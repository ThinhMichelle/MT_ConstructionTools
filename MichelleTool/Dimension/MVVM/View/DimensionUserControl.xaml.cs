using Autodesk.Revit.UI;
using Dimension;
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

namespace Dimension
{
    /// <summary>
    /// Interaction logic for Project1UserControl.xaml
    /// </summary>
    public partial class DimensionUserControl : UserControlBase
    {
        public static DimensionUserControl Instance;
        public DimensionViewModel ViewModel;
        public ExternalEvent _event;
        public DimensionUserControl(ExternalEvent external, UIApplication uiapp)
        {
            InitializeComponent();
            ViewModel = new DimensionViewModel(uiapp);
            DataContext = ViewModel;
            Instance = this;
            _event = external;
        }
    }
}
