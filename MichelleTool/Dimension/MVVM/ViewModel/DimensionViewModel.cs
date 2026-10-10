using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MichelleTool;
using MichelleTool.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using View = Autodesk.Revit.DB.View;

namespace Dimension
{
    public class DimensionViewModel : ViewModelValidateBase
    {
        private int _Value;
        private ObservableCollection<ItemDim> _itemDims = new ObservableCollection<ItemDim>();
        private ItemDim _SelectedDim;
        private bool _Isgrid;
        private bool _IsStructuralColumn;
        private bool _IsStructuralFraming;
        private bool _IsStructuralFoundation;
        private bool _IsWall;


        public int Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ItemDim> ItemDims
        {
            get { return _itemDims; }
            set
            {
                _itemDims = value;
                OnPropertyChanged();
            }
        }

        public ItemDim SelectedDim
        {
            get { return _SelectedDim; }
            set
            {
                _SelectedDim = value;
                OnPropertyChanged();
            }
        }

        public bool Isgrid
        {
            get { return _Isgrid; }
            set
            {
                _Isgrid = value;
                OnPropertyChanged();
            }
        }

        public bool IsStructuralColumn
        {
            get { return _IsStructuralColumn; }
            set
            {
                _IsStructuralColumn = value;
                OnPropertyChanged();
            }
        }

        public bool IsStructuralFraming
        {
            get { return _IsStructuralFraming; }
            set
            {
                _IsStructuralFraming = value;
                OnPropertyChanged();
            }
        }

        public bool IsStructuralFoundation
        {
            get { return _IsStructuralFoundation; }
            set
            {
                _IsStructuralFoundation = value;
                OnPropertyChanged();
            }
        }

        public bool IsWall
        {
            get { return _IsWall; }
            set
            {
                _IsWall = value;
                OnPropertyChanged();
            }
        }

        public ICommand Loaded { get; set; }
        public ICommand OKCmd { get; set; }
        public ICommand CancelCmd { get; set; }

        public List<Element> SelectedElements { get; set; } = new List<Element>();
        public List<XYZ> RefPoints { get; set; } = new List<XYZ>();
        public UIDocument _uidoc { get; set; }
        public Document _doc { get; set; }
        public View _view;

        public DimensionViewModel(UIApplication uiapp)
        {
            _uidoc = uiapp.ActiveUIDocument;
            _doc = _uidoc.Document;
            _view = _uidoc.ActiveView;

            // get DimensionTypes
            GetDimensionTypes();

            // load Json
            var dimJson = LoadJson(_doc);
            if (dimJson != null)
            {
                Isgrid = dimJson.IsGrid;
                IsStructuralColumn = dimJson.IsStructuralColumn;
                IsStructuralFraming = dimJson.IsStructuralFraming;
                IsStructuralFoundation = dimJson.IsStructuralFoundation;
                IsWall = dimJson.IsWall;
                SelectedDim = ItemDims.FirstOrDefault(x => x.Id == long.Parse(dimJson.DimensionTypeUniqueId));
                if (SelectedDim == null) SelectedDim = ItemDims.FirstOrDefault();
            }
            else
            {
                SelectedDim = ItemDims.FirstOrDefault();
                Isgrid = true;
                IsStructuralColumn = false;
                IsStructuralFraming = false;
                IsStructuralFoundation = false;
                IsWall = false;
                Value = 0;
            }
           
           

            // Initialize commands
            Loaded = new RelayCommand<object>((q) =>
            {
                return true;
            }, (q) =>
            {
                Debug.WriteLine("Da chay vo day!");
            });

            OKCmd = new RelayCommand<object>(q =>
            {
                return ErrorCount == 0;
            }, (q) =>
            {
                DimensionController.Instance.Window.Hide();
                GetElementtoDim();
                DimensionUserControl.Instance._event.Raise();
            });

            CancelCmd = new RelayCommand<object>(q =>
            {
                return true;
            }, (q) =>
            {
                DimensionController.Instance.Window.Close();
            });
        }

        private void GetDimensionTypes()
        {
            ItemDims.Clear();

            var types = new FilteredElementCollector(_doc)
                .OfClass(typeof(DimensionType))
                .Cast<DimensionType>()
                .Where(k=> k.StyleType == DimensionStyleType.Linear)
                .OrderBy(x => x.Name);

            foreach (var type in types)
            {
                ItemDims.Add(new ItemDim(GetValueId.Get(type.Id), type.Name));
            }
        }
        private DimensionSettings LoadJson(Document doc)
        {
            return ProjectSettingsManager.Load<DimensionSettings>(doc, CurrentModule.keyDimentionJson);
        }
        public void GetElementtoDim()
        {
            SelectedElements = new List<Element>();
            try
            {
                SelectElements();
                SelectPoint();
            }   
            catch (Exception ex)
            {
                return;
            }
        }

        public void SelectElements()
        {
            try
            {
                var references = _uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element,
                   new SelectionFilterCatgories(DimensionModule.builtInCategories),
                   "Select Element to Dimension");

                SelectedElements = references.Select(k => _doc.GetElement(k)).ToList();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return;
            }
        }
        public void SelectPoint()
        {
            try
            {
                XYZ point1 = _uidoc.Selection.PickPoint(
                    Autodesk.Revit.UI.Selection.ObjectSnapTypes.Endpoints |
                    Autodesk.Revit.UI.Selection.ObjectSnapTypes.Intersections,
                    "Select first point");

                XYZ point2 = _uidoc.Selection.PickPoint(
                    Autodesk.Revit.UI.Selection.ObjectSnapTypes.Endpoints |
                    Autodesk.Revit.UI.Selection.ObjectSnapTypes.Intersections,
                    "Select second point");

                RefPoints = new List<XYZ>
                            {
                                point1,
                                point2
                            };
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return; 
                // Người dùng nhấn ESC
            }
        }
    }
}
