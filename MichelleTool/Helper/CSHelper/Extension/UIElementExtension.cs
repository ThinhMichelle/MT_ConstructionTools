using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MichelleTool
{
    public static class UIElementExtension
    {
        private static Action EmptyDelegate = delegate () { };
        public static void Refresh(this UIElement uiElement)
        {
            ////Task t = new Task(() => {

            uiElement.Dispatcher.BeginInvoke(DispatcherPriority.Normal, EmptyDelegate);
            //});
            //t.Start();

            // uiElement.Dispatcher.BeginInvoke( EmptyDelegate , DispatcherPriority.ContextIdle);
            // uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
        public static void Show2(this Window uIElement)
        {
            uIElement.Dispatcher.Invoke(() =>
            {
                try
                {
                    uIElement.Show();
                }
                catch (Exception)
                {
                }
            });
        }
    }
}
