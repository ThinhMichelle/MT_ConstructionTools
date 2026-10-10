using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;

namespace MichelleTool
{
    public class SelectionFilterCatgory : ISelectionFilter
    {
        private int _id;

        public SelectionFilterCatgory(Category category)
        {
            //_id = category.Id.IntegerValue;
            _id = (int)GetValueId.Get(category.Id);
        }

        public SelectionFilterCatgory(BuiltInCategory builtInCategory)
        {
            _id = (int)builtInCategory;
        }

        public bool AllowElement(Autodesk.Revit.DB.Element elem)
        {
            return elem.Category != null && GetValueId.Get(elem.Category.Id).Equals(_id) && elem.GetTypeId() != ElementId.InvalidElementId;
        }

        public bool AllowReference(Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.XYZ position)
        {
            return true;
        }
    }
    public class SelectionFilterCatgories : ISelectionFilter
    {
        private List<long> _ids;

        public SelectionFilterCatgories(List<Category> categories)
        {
            //_id = category.Id.IntegerValue;
            _ids = categories.Select(c => GetValueId.Get(c.Id)).ToList();
        }

        public SelectionFilterCatgories(List<BuiltInCategory> builtInCategories)
        {
            _ids = builtInCategories.Select(b => (long)b).ToList();
        }

        public bool AllowElement(Autodesk.Revit.DB.Element elem)
        {
            return elem.Category != null && _ids.Any(id => id == GetValueId.Get(elem.Category.Id)) && elem.GetTypeId() != ElementId.InvalidElementId;
        }

        public bool AllowReference(Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.XYZ position)
        {
            return true;
        }
    }
    public class SelectionFilterInLinkCAD : ISelectionFilter
    {
        private Document _doc;
        private ImportInstance _importInstance;
        public SelectionFilterInLinkCAD(Document doc, ImportInstance importInstance)
        {
            _doc = doc;
            _importInstance = importInstance;
        }
        public bool AllowElement(Element elem)
        {
            if (_importInstance == null ) return false;
            ImportInstance ims = elem as ImportInstance;
            if (ims != null)
            {
                return CheckEqualId.Check(ims.Id, _importInstance.Id);
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            ElementId eleId = reference.ElementId;
            ImportInstance importinstance = _doc.GetElement(eleId) as ImportInstance;
            return (importinstance == null && CheckEqualId.Check(importinstance.Id, _importInstance.Id)) ? false : true;
        }
    }
    public class SelectionFilterCADLink : ISelectionFilter
    {

        public SelectionFilterCADLink()
        {
        }

        //public bool AllowElement(Element elem)
        //{
        //    if (elem is ImportInstance importInstance)
        //    {
        //        return importInstance.IsLinked && elem.Document.GetElement(importInstance.GetTypeId()) is CADLinkType;
        //    }
        //    return false;
        //}
        public bool AllowElement(Element elem)
        {
            if (elem is ImportInstance importInstance)
            {
                return elem.Document.GetElement(importInstance.GetTypeId()) is CADLinkType;
            }
            return false;
        }

        public bool AllowReference(Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.XYZ position)
        {
            return true;
        }
    }
    public class SelectionFilterCatgoryAndName : ISelectionFilter
    {
        private int _id;
        private List<string> _names;

        public SelectionFilterCatgoryAndName(Category category, List<string> names)
        {
            _id = (int)GetValueId.Get(category.Id);
            _names = names;
        }

        public SelectionFilterCatgoryAndName(BuiltInCategory builtInCategory, List<string> names)
        {
            _id = (int)builtInCategory;
            _names = names;
        }

        public bool AllowElement(Autodesk.Revit.DB.Element elem)
        {
            return elem.Category != null && GetValueId.Get(elem.Category.Id).Equals(_id) && elem.GetTypeId() != ElementId.InvalidElementId && _names.Contains(elem.Name);
        }

        public bool AllowReference(Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.XYZ position)
        {
            return true;
        }
    }

    public class SelectionFilterCatgoriesAndNames : ISelectionFilter
    {
        private List<int> _id;
        private List<string> _names;

        public SelectionFilterCatgoriesAndNames(List<Category> categories, List<string> names)
        {
            _id = categories.Select(k => (int)GetValueId.Get(k.Id)).ToList();
            _names = names;
        }

        public SelectionFilterCatgoriesAndNames(List<BuiltInCategory> builtInCategories, List<string> names)
        {
            _id = builtInCategories.Select(k => (int)k).ToList();
            _names = names;
        }

        public bool AllowElement(Autodesk.Revit.DB.Element elem)
        {
            if (elem != null)
            {
                if (elem.Category == null) return false;
                if (!_id.Contains((int)GetValueId.Get(elem.Category.Id))/* || elem.GetTypeId() == ElementId.InvalidElementId*/) return false;
                if (elem is FamilyInstance)
                {
                    return _names.Contains(elem.Name);
                }
                //else
                //{
                //    if (_id.Contains(elem.Category.Id.IntegerValue))
                //    {
                //        var store = CurrentModule.ElementStore.GetValueSchema(elem, CurrentModule.LINE2DPANEL_SCHEMA_KEY);
                //        return store != "";
                //    }
                //}
            }
            return false;
        }

        public bool AllowReference(Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.XYZ position)
        {
            return true;
        }
    }

    public class SelectionFilterCatgoryAndContainsName : ISelectionFilter
    {
        private int _id;
        private List<string> _names;

        public SelectionFilterCatgoryAndContainsName(Category category, List<string> names)
        {
            _id = (int)GetValueId.Get(category.Id);
            _names = names;
        }

        public SelectionFilterCatgoryAndContainsName(BuiltInCategory builtInCategory, List<string> names)
        {
            _id = (int)builtInCategory;
            _names = names;
        }

        public bool AllowElement(Autodesk.Revit.DB.Element elem)
        {
            return elem.Category != null && GetValueId.Get(elem.Category.Id).Equals(_id) && elem.GetTypeId() != ElementId.InvalidElementId && _names.Any(k => elem.Name.Contains(k));
        }

        public bool AllowReference(Autodesk.Revit.DB.Reference reference, Autodesk.Revit.DB.XYZ position)
        {
            return true;
        }
    }

    public class SelectionFilterCategories : ISelectionFilter
    {
        private readonly List<BuiltInCategory> _categories;

        public SelectionFilterCategories(List<BuiltInCategory> categories)
        {
            _categories = categories;
        }

        public bool AllowElement(Element elem)
        {
            if (elem.Category == null) return false;

            return _categories.Contains((BuiltInCategory)GetValueId.Get(elem.Category.Id));
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}