using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace Project1
{
    [Transaction(TransactionMode.Manual)]
    public class Project1RevitHandler : IExternalEventHandler
    {
        private bool isRunFirstTime = true;
        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            if (isRunFirstTime)
            {

                isRunFirstTime = false;
            }
            try
            {

                //TODO: Write Revit API code here



            }
            catch (Exception exception)
            {

                System.Windows.MessageBox.Show("Some error has occured. \n" + exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        public string GetName()
        {
            return "Revit Addin";
        }
    }
}
