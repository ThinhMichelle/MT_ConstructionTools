using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichelleTool
{  
    public class ConvertIdType
    {
#if REVIT2026 || REVIT2027
        public static long Convert(object idType)
        {
            return (long)idType;
        }
#else
        public static int Convert(object idType)
        {
            return int.Parse(idType.ToString());
        }
#endif
    }

    public class GetValueId
    {
#if REVIT2026 || REVIT2027
        public static long Get(ElementId id)
        {
            return id.Value;
        }
#else
        public static long Get(ElementId id)
        {
            return id.IntegerValue;
        }
#endif
    }

    public class CheckEqualId
    {
#if REVIT2026
        public static bool Check(ElementId id, BuiltInCategory builtInCategory)
        {
            if (id == null) return false;
            return GetValueId.Get(id) == ConvertIdType.Convert((long)builtInCategory);
        }
        public static bool Check(ElementId id, object idCheck)
        {
            if (id == null) return false;
            return GetValueId.Get(id) == ConvertIdType.Convert(idCheck);
        }
        public static bool Check(ElementId id, ElementId idCheck)
        {
            if (id == null || idCheck == null) return false;
            return GetValueId.Get(id) == GetValueId.Get(idCheck);
        }
        public static bool Check(long id, BuiltInCategory builtInCategory)
        {
            return id == ConvertIdType.Convert((long)builtInCategory);
        }
#else
        public static bool Check(ElementId id, BuiltInCategory builtInCategory)
        {
            if (id == null) return false;
            return GetValueId.Get(id) == ConvertIdType.Convert((long)builtInCategory);
        }
        public static bool Check(ElementId id, object idCheck)
        {
            if (id == null) return false;
            return GetValueId.Get(id) == ConvertIdType.Convert(idCheck);
        }
        public static bool Check(ElementId id, ElementId idCheck)
        {
            if (id == null) return false;
            return GetValueId.Get(id) == GetValueId.Get(idCheck);
        }
        public static bool Check(long id, BuiltInCategory builtInCategory)
        {
            return id == ConvertIdType.Convert((long)builtInCategory);
        }
#endif
    }

    public class GetID
    {
#if REVIT2026 || REVIT2025 || REVIT2024 || REVIT2027
        public static ElementId Get(long id)
        {
            return new ElementId(id);
        }
#else
        public static ElementId Get(long id)
        {
            return new ElementId(int.Parse(id.ToString()));
        }
#endif
    }
}
