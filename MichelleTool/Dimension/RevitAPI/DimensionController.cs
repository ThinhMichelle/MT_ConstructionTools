using Autodesk.Revit.UI;
using MichelleTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Dimension
{
    public class DimensionController : WpfWindowControllerBase
    {
        public static DimensionController Instance = null;
        public DimensionController()
        {
            Instance = this;
        }
    }

}
