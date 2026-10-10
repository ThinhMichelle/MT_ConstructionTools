using System;
using System.Collections.Generic;
using System.Text;

namespace Dimension
{
    public class DimensionSettings
    {
        /// <summary>
        /// UniqueId của DimensionType được chọn
        /// </summary>
        public string DimensionTypeUniqueId { get; set; }

        public bool IsGrid { get; set; }

        public bool IsStructuralColumn { get; set; }

        public bool IsStructuralFraming { get; set; }

        public bool IsStructuralFoundation { get; set; }

        public bool IsWall { get; set; }
    }
}
