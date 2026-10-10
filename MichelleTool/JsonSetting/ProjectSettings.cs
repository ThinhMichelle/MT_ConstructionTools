using Dimension;
using System;
using System.Collections.Generic;
using System.Text;

namespace MichelleTool
{
    public class ProjectSettings
    {
        public int Version { get; set; } = 1;

        public DimensionSettings Dimension { get; set; } = new DimensionSettings();
    }
}
