using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MichelleTool
{
    public static class DimensionExtension
    {
        /// <summary>
        /// Kiểm tra xem dimension có bị lỗi hay không
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public static bool CheckDimensionIsValid(this Autodesk.Revit.DB.Dimension dim)
        {
            if (dim == null) return false;
            if (dim.IsValidObject == false ) return false;
            if (string.IsNullOrEmpty(dim.ValueString) == true && dim.Segments.Size > 0)
            {
                foreach (DimensionSegment segment in dim.Segments)
                {
                    if (segment.ValueString == "0" || segment.ValueString.Contains("-")) return false;
                }
            }
            else if (string.IsNullOrEmpty(dim.ValueString) == false)
            {
                if (dim.ValueString == "0" || dim.ValueString.Contains("-")) return false;
            }
            return true;
        }
    }
}
