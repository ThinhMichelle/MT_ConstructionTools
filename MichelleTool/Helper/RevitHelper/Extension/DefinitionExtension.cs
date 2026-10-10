using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichelleTool
{
    public static class DefinitionExtension
    {
#if (REVIT2020 || REVIT2021)
        public static ParameterType GetParameterType(this Definition def)
        {
            return def.ParameterType;
        }
#else
        public static ForgeTypeId GetParameterType(this Definition def)
        {
            return def.GetDataType();
        }
#endif
#if (REVIT2020 || REVIT2021)
        public static BuiltInParameterGroup GetParameterGroup(this Definition def)
        {
            return def.ParameterGroup;
        }
#else
        public static ForgeTypeId GetParameterGroup(this Definition def)
        {
            return def.GetGroupTypeId();
        }
#endif
    }
}
