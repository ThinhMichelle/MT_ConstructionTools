using Autodesk.Revit.UI;
using MichelleTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tag
{
    /// <summary>
    /// Interaction logic for TagUserControl.xaml
    /// </summary>
    public partial class TagUserControl : UserControlBase
    {
        public TagUserControl(ExternalEvent external, UIApplication uiapp)
        {
            InitializeComponent();
            DataContext = new TagViewModel(uiapp);
        }
    }
}
