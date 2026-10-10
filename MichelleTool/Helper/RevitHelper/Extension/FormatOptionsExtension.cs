using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichelleTool
{
    public static class FormatOptionsExtension
    {
#if !(REVIT2019 || REVIT2020)
        public static ForgeTypeId GetDisplayUnitType(this FormatOptions formatOptions)
        {
            return formatOptions.GetUnitTypeId();
        }
        public static ForgeTypeId GetUnitSymbol(this FormatOptions formatOptions)
        {
            return formatOptions.GetValidSymbols().FirstOrDefault();
        }
#else
    public static DisplayUnitType GetDisplayUnitType(this FormatOptions formatOptions)
        {
            return formatOptions.DisplayUnits;
        }
        public static DisplayUnitType GetUnitSymbol(this FormatOptions formatOptions)
        {
            return formatOptions.DisplayUnits;
        }
       
#endif
    }
}
