using Autodesk.Revit.UI;
using MichelleTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Tag
{
    public class TagController : WpfWindowControllerBase
    {
        public static TagController Instance = null;
        public TagController()
        {
            Instance = this;
        }
    }

}
