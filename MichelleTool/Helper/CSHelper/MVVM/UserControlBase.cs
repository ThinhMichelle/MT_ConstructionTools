using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace MichelleTool
{
    public class UserControlBase : UserControlValidateBase
    {
        public UserControlBase()
        {
            var _ = new Microsoft.Xaml.Behaviors.DefaultTriggerAttribute(typeof(Trigger), typeof(Microsoft.Xaml.Behaviors.TriggerBase), null);
            this.Loaded += UserControlBase_Loaded;
        }

        private void UserControlBase_Loaded(object sender, RoutedEventArgs e)
        {
            base.OnLoad(sender, e);
            ControlsUtils.Translate(this);
        }
    }
}
