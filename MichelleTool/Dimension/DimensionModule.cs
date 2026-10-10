using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dimension
{
    public class DimensionModule
    {
        public static List<BuiltInCategory> builtInCategories = new List<BuiltInCategory>()
        { BuiltInCategory.OST_Grids, BuiltInCategory.OST_StructuralFraming, BuiltInCategory.OST_Walls , BuiltInCategory.OST_StructuralFoundation};
    }
}
