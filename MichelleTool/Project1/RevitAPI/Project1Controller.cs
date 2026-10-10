using Autodesk.Revit.UI;
using MichelleTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Project1
{
    public class Project1Controller : WpfWindowControllerBase
    {
        public static Project1Controller Instance = null;
        public Project1Controller()
        {
            Instance = this;
        }
    }

}
