using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MichelleTool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Xml.Linq;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace Dimension
{
    [Transaction(TransactionMode.Manual)]
    public class DimensionRevitHandler : IExternalEventHandler
    {
        private bool isRunFirstTime = true;
        private const string Found_sideA = "MB_a";
        private const string Found_sideB = "MB_b";
        private const string BEAM_PREFIX = "RC_";

        DimensionViewModel viewModel;

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;


            if (isRunFirstTime)
            {

                isRunFirstTime = false;
            }
            TransactionGroup tsg = new TransactionGroup(doc, "Create Dimension");

            try
            {
                viewModel = DimensionUserControl.Instance.ViewModel;
                //TODO: Write Revit API code here
                tsg.Start();
                MainProcess(doc);
            }
            catch (Exception exception)
            {

                System.Windows.MessageBox.Show("Some error has occured. \n" + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                tsg.RollBack();
                DimensionController.Instance.Window.Show();

            }
            finally
            {
                SaveJson(doc, viewModel);
                tsg.Assimilate();
                DimensionController.Instance.Window.Show();

            }


        }

        private void MainProcess(Document doc)
        {
            if (viewModel.SelectedElements == null || !viewModel.SelectedElements.Any()) return;
            if (viewModel.RefPoints == null || !viewModel.RefPoints.Any()) return;

            var lineDim = CreateLineByNearestAxis(viewModel.RefPoints[0], viewModel.RefPoints[1], 10, out XYZ center, out XYZ selectedAxis);
            var refGrids = new List<Reference>();
            var refTotals = new List<Reference>();
            Line lineDimGrid = null;
            Line lineDimhead = null;
            var referenceParts = GetReferenceTotals(lineDim,ref refGrids, ref refTotals);

            var combineArray = DimensionUtils.CombineReferences(referenceParts, doc.ActiveView, lineDim);
            if (combineArray == null) return;
            if (combineArray.Size < 2) return;

            var typeDim = doc.GetElement(GetID.Get(viewModel.SelectedDim.Id)) as DimensionType;
            // get direct to place next dim
            var firstEle = viewModel.SelectedElements.First();
            var box = firstEle.get_BoundingBox(doc.ActiveView);
            var centerPt = (box.Min + box.Max) * 0.5;
            var refPt = Common.ProjectPointToLine(lineDim.Extend(), centerPt);
            var direct = refPt.ProjectPointZ0() - centerPt.ProjectPointZ0();
            // dim part
            var dimPart = CreateDim.CreateDimension(doc, lineDim, combineArray, typeDim);
            var paraDistance = typeDim.get_Parameter(BuiltInParameter.DIM_STYLE_DIM_LINE_SNAP_DIST);
            var distance = paraDistance.AsDouble() * doc.ActiveView.Scale;
            direct = direct.Normalize() * distance;

            // dim grid 
            lineDimGrid = Common.MoveLine(lineDim, direct);
            combineArray = DimensionUtils.CombineReferences(refGrids, doc.ActiveView, lineDim);
            if (combineArray.Size == 2)
            {
                var dimGrids = CreateDim.CreateDimension(doc, lineDimGrid, combineArray, typeDim);
            }
            else if (combineArray.Size > 2)
            {
                var dimGrids = CreateDim.CreateDimension(doc, lineDimGrid, combineArray, typeDim);

                // dim header
                lineDimhead = Common.MoveLine(lineDim, direct * 2);
                combineArray = DimensionUtils.CombineReferences(refTotals, doc.ActiveView, lineDim);
                var dimHeads = CreateDim.CreateDimension(doc, lineDimhead, combineArray, typeDim);

            }
        }
        public static XYZ GetBoundingBoxDirection(Element element)
        {
            BoundingBoxXYZ bbox = element.get_BoundingBox(null);

            if (bbox == null)
                return XYZ.Zero;

            double deltaX = bbox.Max.X - bbox.Min.X;
            double deltaY = bbox.Max.Y - bbox.Min.Y;

            return deltaX >= deltaY ? XYZ.BasisY : XYZ.BasisX;
        }
        public static (Grid grid1, Grid grid2) GetOuterGrids(List<Grid> grids)
        {
            if (grids == null || grids.Count < 2)
                throw new ArgumentException("List phải có ít nhất 2 Grid.");

            Grid grid1 = null;
            Grid grid2 = null;
            double maxDistance = double.MinValue;

            for (int i = 0; i < grids.Count - 1; i++)
            {
                Curve curve1 = grids[i].Curve;
                XYZ p1 = (curve1.GetEndPoint(0) + curve1.GetEndPoint(1)) / 2;

                for (int j = i + 1; j < grids.Count; j++)
                {
                    Curve curve2 = grids[j].Curve;
                    XYZ p2 = (curve2.GetEndPoint(0) + curve2.GetEndPoint(1)) / 2;

                    double distance = p1.DistanceTo(p2);

                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        grid1 = grids[i];
                        grid2 = grids[j];
                    }
                }
            }

            return (grid1, grid2);
        }
        public List<Reference> GetReferenceTotals(Line lineDim,ref List<Reference> referenceGrids, ref List<Reference> referenceHeads)
        {
            var referenceTotals = new List<Reference>();


            if (viewModel.IsStructuralColumn)
            {
                var columns = viewModel.SelectedElements.Where(e => e.Category != null && (BuiltInCategory)GetValueId.Get(e.Category.Id) == BuiltInCategory.OST_StructuralColumns).Cast<FamilyInstance>();
                if (columns.Any())
                {
                    foreach (var column in columns)
                    {
                        var refLR = column.GetRefCenterLR();
                        var refFB = column.GetRefCenterFB();

                        if (refLR != null) referenceTotals.Add(refLR);
                        if (refFB != null) referenceTotals.Add(refFB);
                    }
                }
            }

            if (viewModel.IsStructuralFraming)
            {
                var framings = viewModel.SelectedElements.Where(e => e.Category != null && (BuiltInCategory)GetValueId.Get(e.Category.Id) == BuiltInCategory.OST_StructuralFraming).Cast<FamilyInstance>();

                foreach (var framing in framings)
                {
                    var refLR = framing.GetRefCenterLR();
                    var refFB = framing.GetRefCenterFB();

                    if (refLR != null) referenceTotals.Add(refLR);
                    if (refFB != null) referenceTotals.Add(refFB);
                }
            }

            if (viewModel.IsStructuralFoundation)
            {
                var foundations = viewModel.SelectedElements.Where(e => e.Category != null && (BuiltInCategory)GetValueId.Get(e.Category.Id) == BuiltInCategory.OST_StructuralFoundation).Where(k => k is FamilyInstance).Cast<FamilyInstance>();

                foreach (var found in foundations)
                {
                    var direct = GetBoundingBoxDirection(found);
                    var isAdd = Common.IsParallel(lineDim.Direction, direct);
                    if (!isAdd) continue;
                    var refA = found.GetReferencesByName(Found_sideA);
                    var refB = found.GetReferencesByName(Found_sideB);
                    if (refA != null) referenceTotals.Add(refA);
                    if (refB != null) referenceTotals.Add(refB);
                }
            }
            if (viewModel.Isgrid)
            {
                var grids = viewModel.SelectedElements.Where(e => e.Category != null && (BuiltInCategory)GetValueId.Get(e.Category.Id) == BuiltInCategory.OST_Grids);
                if (grids.Any())
                {
                    referenceTotals.AddRange(grids.Select(g => new Reference(g)));
                    referenceGrids.AddRange(grids.Select(g => new Reference(g)));
                    if (grids.Count() > 2)
                    {
                        var gridOutSide = GetOuterGrids(grids.Cast<Grid>().ToList());
                        referenceHeads.Add(new Reference(gridOutSide.grid1));
                        referenceHeads.Add(new Reference(gridOutSide.grid2));
                    }
                }
            }
            return referenceTotals;

        }

        public void SaveJson(Document doc, DimensionViewModel viewModel)
        {
            DimensionSettings settings = new DimensionSettings
            {
                DimensionTypeUniqueId = viewModel.SelectedDim?.Id.ToString(),

                IsGrid = viewModel.Isgrid,

                IsStructuralColumn = viewModel.IsStructuralColumn,

                IsStructuralFraming = viewModel.IsStructuralFraming,

                IsStructuralFoundation = viewModel.IsStructuralFoundation,

                IsWall = viewModel.IsWall
            };
            ProjectSettingsManager.Save(doc, CurrentModule.keyDimentionJson, settings);
        }
        public static Line CreateLineByNearestAxis(XYZ p1, XYZ p2, double length, out XYZ center, out XYZ selectedAxis)
        {
            // 1. Tạo line giữa 2 điểm
            Line sourceLine = Line.CreateBound(p1, p2);

            // Vector đơn vị của line
            XYZ direction = (p2 - p1).Normalize();

            // 2. Trục X và Y
            XYZ axisX = XYZ.BasisX;
            XYZ axisY = XYZ.BasisY;

            // Tính góc giữa line và Ox
            double angleX = direction.AngleTo(axisX);

            // Tính góc giữa line và Oy
            double angleY = direction.AngleTo(axisY);

            // Vì direction có thể ngược chiều trục,
            // lấy góc nhỏ nhất giữa +axis và -axis
            angleX = Math.Min(angleX, Math.PI - angleX);
            angleY = Math.Min(angleY, Math.PI - angleY);

            // 3. Chọn trục có góc nhỏ hơn
            selectedAxis = angleX <= angleY
                ? axisX
                : axisY;

            // 4. Lấy center của line
            center = (p1 + p2) * 0.5;

            // 5. Tạo line mới từ center,
            // dài "length", chia đều 2 phía
            XYZ halfVector = selectedAxis * (length / 2.0);

            XYZ start = center - halfVector;
            XYZ end = center + halfVector;

            return Line.CreateBound(start, end);
        }

        public string GetName()
        {
            return "Revit Addin";
        }
    }
    public enum TypeDirect
    {
        X, Y
    }
   
}
