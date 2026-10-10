using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;
using Category = Autodesk.Revit.DB.Category;
using Color = Autodesk.Revit.DB.Color;
using Document = Autodesk.Revit.DB.Document;
using Parameter = Autodesk.Revit.DB.Parameter;
using Transform = Autodesk.Revit.DB.Transform;
using View = Autodesk.Revit.DB.View;

namespace MichelleTool
{
    public static class Common
    {
       
        /// <summary>
        /// get parameter string value by its name
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static string GetParameterStringValueByName(Element elem, string paramName)
        {
            if (elem == null || elem.IsValidObject == false)
                return string.Empty;

            Parameter param = elem.LookupParameter(paramName);
            return param?.AsString();
        }
        public static Line MoveLine(Line originalLine, XYZ direct)
        {
            XYZ newStart = originalLine.GetEndPoint(0) + direct;
            XYZ newEnd = originalLine.GetEndPoint(1) + direct;
            return Line.CreateBound(newStart, newEnd);
        }
        public static XYZ ProjectPointToLine(Line line, XYZ point)
        {
            XYZ A = line.GetEndPoint(0);
            XYZ B = line.GetEndPoint(1);

            XYZ AB = B - A;
            XYZ AP = point - A;

            double ab2 = AB.DotProduct(AB);

            // Line có chiều dài = 0
            if (ab2 < 1e-9)
                return A;

            // Tham số chiếu
            double t = AP.DotProduct(AB) / ab2;

            // Giới hạn vào đoạn [A, B]
            t = Math.Max(0.0, Math.Min(1.0, t));

            // Điểm chiếu
            return A + t * AB;
        }
        /// <summary>
        /// get parameter bool value by its name
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        /// 

        public static bool GetParameterBoolValueByName(Element elem, string paramName)
        {
            if (elem == null || elem.IsValidObject == false)
                return false;

            Parameter param = elem.LookupParameter(paramName);
            if (param != null && param.StorageType == StorageType.Integer)
            {
                return param.AsInteger() == 1;
            }

            return false;
        }
        public static bool IsParallel(XYZ dir1, XYZ dir2, double tolerance = 1e-6)
        {
            if (dir1 == null || dir2 == null)
                return false;

            if (dir1.IsZeroLength() || dir2.IsZeroLength())
                return false;

            XYZ cross = dir1.Normalize().CrossProduct(dir2.Normalize());

            return cross.GetLength() < tolerance;
        }
        /// <summary>
        /// set parameter value by its name
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetParameterValueByName(Element elem, string paramName, object value)
        {
            if (elem == null || elem.IsValidObject == false)
                return false;

            Parameter param = elem.LookupParameter(paramName);
            return SetParameterValue(param, value);
        }

        /// <summary>
        /// Set value to parameter based on its storage type
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetParameterValue(Parameter param, object value)
        {
            if (param != null
                && !param.IsReadOnly
                && value != null)
            {
                try
                {
                    switch (param.StorageType)
                    {
                        case StorageType.Integer:
                            param.Set((int)value);
                            break;

                        case StorageType.Double:
                            param.Set((double)value);
                            break;

                        case StorageType.String:
                            param.Set((string)value);
                            break;

                        case StorageType.ElementId:
                            param.Set((ElementId)value);
                            break;
                    }
                    return true;
                }
                catch (Exception) { }
            }
            return false;
        }     
        public static BasePoint GetBasePoint(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc)
           .OfCategory(BuiltInCategory.OST_ProjectBasePoint)
           .WhereElementIsNotElementType();

            BasePoint projectBasePoint = null;

            foreach (Element element in collector)
            {
                projectBasePoint = element as BasePoint;
                if (projectBasePoint != null)
                    break;
            }

            return projectBasePoint;
        }
        public static XYZ ProjectPointZ0(this XYZ Pt)
        {
            return new XYZ(Pt.X, Pt.Y, 0);
        }
        public static List<Solid> GetSolidsofInstance(FamilyInstance familyInstance)
        {
            var solids = new List<Solid>();
            Options geomOptions = new Options
            {
                ComputeReferences = true,
                DetailLevel = ViewDetailLevel.Fine
            };
            // Lấy hình học của FamilyInstance
            var geometry = familyInstance.get_Geometry(geomOptions);
            if (geometry == null) return solids;
            foreach (GeometryObject geomObj in geometry)
            {
                // Nếu là GeometryInstance thì truy cập bên trong nó
                if (geomObj is GeometryInstance geomInstance)
                {
                    GeometryElement instanceGeometry = geomInstance.GetInstanceGeometry();

                    foreach (GeometryObject instObj in instanceGeometry)
                    {
                        if (instObj is Solid solid && solid.Volume > 0)
                        {
                            solids.Add(solid);
                        }
                    }
                }
                // Nếu là Solid (trường hợp Family đơn giản)
                else if (geomObj is Solid directSolid && directSolid.Volume > 0)
                {
                    solids.Add(directSolid);
                }
            }
            return solids;
        }

        public static void MoveLine1(this Line originalLine, XYZ direct)
        {
            XYZ newStart = originalLine.GetEndPoint(0) + direct;
            XYZ newEnd = originalLine.GetEndPoint(1) + direct;
            originalLine = Line.CreateBound(newStart, newEnd);
        }
        public static XYZ MovePoint(this XYZ originalPoint, XYZ direct)
        {
            return originalPoint + direct;
        }

        public static BasePoint GetProjectBasePoint(Document doc)
        {
            BasePoint projectBasePoint = new FilteredElementCollector(doc)
            .OfClass(typeof(BasePoint))
            .Cast<BasePoint>()
            .FirstOrDefault(bp => bp.IsShared == false);

            return projectBasePoint;
        }
        public static List<XYZ> GetPointofSolid(Solid solid)
        {
            var points = new List<XYZ>();
            foreach (Edge edge in solid.Edges)
            {
                points.AddRange(edge.Tessellate());
            }
            return points;
        }

        public static List<Face> GetFacesPerallelZ(Solid solid)
        {
            var lisfaces = new List<Face>();
            foreach (var face in solid.Faces)
            {
                if (face is PlanarFace planarFace)
                {
                    if (planarFace.Reference == null) continue;
                    if (planarFace.FaceNormal.Round(2).IsAlmostEqualTo(XYZ.BasisZ) || planarFace.FaceNormal.Round(2).IsAlmostEqualTo(XYZ.BasisZ.Negate())) lisfaces.Add(planarFace);
                }
            }
            return lisfaces;
        }
       
        public static List<DimensionType> GetDimensionStyle(Document doc)
        {
            // Lọc tất cả các DimensionType trong dự án
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(DimensionType));

            // Lọc DimensionType có FamilyName là "Linear Dimension Style"
            return collector
                .Cast<DimensionType>()
                .Where(dimType => dimType.StyleType == DimensionStyleType.Linear).ToList();
        }
        public static List<TextNoteType> GetTextNoteType(Document doc)
        {
            // Lọc tất cả các DimensionType trong dự án
            FilteredElementCollector collector = new FilteredElementCollector(doc)
                .OfClass(typeof(TextNoteType));

            // Lọc DimensionType có FamilyName là "Linear Dimension Style"
            return collector
                .Cast<TextNoteType>()
                .ToList();
        }
        public static List<Reference> GetReferenceofFamilyInstance(FamilyInstance familyinstance)
        {
            var reflefright = familyinstance.GetReferences(FamilyInstanceReferenceType.CenterLeftRight);
            var reffrontback = familyinstance.GetReferences(FamilyInstanceReferenceType.CenterFrontBack);
            var refs = new List<Reference>();
            refs.AddRange(reffrontback);
            refs.AddRange(reflefright);
            return refs;
        }
        public static Reference GetReferenceLeftRightofFamilyInstance(FamilyInstance familyinstance)
        {
            return familyinstance.GetReferences(FamilyInstanceReferenceType.CenterLeftRight).FirstOrDefault();
        }
        public static Reference GetReferenceFrontBackofFamilyInstance(FamilyInstance familyinstance)
        {
            return familyinstance.GetReferences(FamilyInstanceReferenceType.CenterFrontBack).FirstOrDefault();
        }

        public static double AngleBetween(XYZ v1, XYZ v2)
        {
            // Kiểm tra vector null hoặc độ dài 0
            if (v1 == null || v2 == null) return double.NaN;
            if (v1.IsAlmostEqualTo(XYZ.Zero) || v2.IsAlmostEqualTo(XYZ.Zero)) return double.NaN;
            double dot = v1.Normalize().DotProduct(v2.Normalize());
            dot = Math.Max(-1.0, Math.Min(1.0, dot)); // tránh lỗi sai số > 1 hoặc < -1

            double angle = Math.Acos(dot);
            return angle;
        }     
        /// <summary>
        /// Lấy tất cả Annotation Symbol Types trong document (Generic Annotation)
        /// </summary>
        public static List<FamilySymbol> GetAllAnnotationSymbols(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc)
               .OfClass(typeof(FamilySymbol))
       .OfCategory(BuiltInCategory.OST_GenericAnnotation);

            return collector
      .Cast<FamilySymbol>()
      .Where(fs => GetValueId.Get(fs.Family.FamilyCategory.Id) == (int)BuiltInCategory.OST_GenericAnnotation)
       .ToList();
        }

       
        public static void DrawCurve3Dview(Document doc, Curve curve, int viewId, Color newcolor)
        {
            var view = doc.GetElement(new ElementId(viewId)) as View;
            DebugUtils.DrawCurve(view, curve, newcolor);
            //Transaction tx = new Transaction(doc);
            //tx.Start("Draw Curve");
            //DebugUtils.DrawCurve(view, curve, newcolor);
            //tx.Commit();
        }
        public static void DrawCurve2DView(Document doc, Curve curve, int viewId, Color newcolor)
        {
            var view = doc.GetElement(new ElementId(viewId)) as View;
            //DebugUtils.DrawDetailCurve(view, curve, newcolor);
            Transaction tx = new Transaction(doc);
            tx.Start("Draw Curve");
            DebugUtils.DrawCurve(view, curve, newcolor);
            tx.Commit();
        }

        public static FamilySymbol GetFamilySymbolByName(Document doc,string familyName, string typeName)
        {
            if (string.IsNullOrEmpty(familyName) || string.IsNullOrEmpty(typeName))
            {
                return null;
            }
            try
            {
                // Lọc tất cả FamilySymbol trong document
                var familySymbol = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .FirstOrDefault(fs =>
                fs.Family != null &&
                fs.Family.Name.Equals(familyName, System.StringComparison.OrdinalIgnoreCase) &&
                fs.Name.Equals(typeName, System.StringComparison.OrdinalIgnoreCase));
                if (familySymbol == null)
                {
                    return new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .FirstOrDefault(fs =>
                fs.Family != null &&
                fs.Family.Name.Equals(familyName, System.StringComparison.OrdinalIgnoreCase) &&
                fs.Name.Contains(typeName, System.StringComparison.OrdinalIgnoreCase));
                }
                return familySymbol;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        public static FamilySymbol GetFamilySymbolByName2(Document doc, string familyName, string typeName)
        {
            if (string.IsNullOrEmpty(familyName) || string.IsNullOrEmpty(typeName))
            {
                return null;
            }
            try
            {

                switch (typeName)
                {
                    case "102":
                        typeName = "100";
                        break;
                    case "92":
                        typeName = "90";
                        break;
                    case "77":
                        typeName = "75";
                        break;
                    case "67":
                        typeName = "65";
                        break;
                    default:
                        break;
                }
                // Lọc tất cả FamilySymbol trong document
                var familySymbol = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .FirstOrDefault(fs =>
                fs.Family != null &&
                fs.Family.Name.Equals(familyName, System.StringComparison.OrdinalIgnoreCase) &&
                fs.Name.Contains(typeName, System.StringComparison.OrdinalIgnoreCase));
                if (familySymbol == null)
                {
                    return new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .FirstOrDefault(fs =>
                fs.Family != null &&
                fs.Family.Name.Equals(familyName, System.StringComparison.OrdinalIgnoreCase) &&
                fs.Name.Contains(typeName, System.StringComparison.OrdinalIgnoreCase));
                }
                return familySymbol;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }
        public static List<FamilySymbol> GetFamilySymbolsByName(Document doc, string familyName)
        {
            if (string.IsNullOrEmpty(familyName))
            {
                return null;
            }
            try
            {
                // Lọc tất cả FamilySymbol trong document
                return new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .Where(fs =>
                fs.Family != null &&
                fs.Family.Name.Equals(familyName, System.StringComparison.OrdinalIgnoreCase)).ToList();             
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }
        public static XYZ OffsetPoint(XYZ basePoint, XYZ direction, double distance)
        {
            if (basePoint == null || direction == null) return basePoint;

            XYZ normalizedDirection = direction.Normalize();
            return basePoint + (normalizedDirection * distance);
        }
        public static XYZ GetExteriorWallDirection(Wall wall)
        {
            // Lấy đường location của tường
            LocationCurve locationCurve = wall.Location as LocationCurve;
            if (locationCurve == null)
                throw new Exception($"Wall ID {wall.Id} không có LocationCurve.");

            Curve curve = locationCurve.Curve;
            XYZ direction;

            if (curve is Line line)
            {
                // Nếu là line thì lấy vector tiếp tuyến
                direction = line.Direction.Normalize();
            }
            else
            {
                // Nếu là curve (arc, spline, v.v...) thì lấy vector từ điểm đầu đến cuối
                direction = (curve.GetEndPoint(1) - curve.GetEndPoint(0)).Normalize();
            }

            // Vector Z chuẩn
            XYZ up = XYZ.BasisZ;

            // Hướng mặt ngoài được tính = Z × direction (tích có hướng)
            XYZ exteriorDir = up.CrossProduct(direction);

            // Nếu tường bị lật thì đảo hướng
            if (wall.Flipped)
                exteriorDir = -exteriorDir;

            return exteriorDir.Normalize();
        }

    }
}