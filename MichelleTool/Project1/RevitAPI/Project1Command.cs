#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using MichelleTool;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace Project1
{
    [Transaction(TransactionMode.Manual)]
    public class Project1Command : IExternalCommand
    {
        private Project1Controller _wpfWindowController = new Project1Controller();
        /// <summary>
        /// External command mainline
        /// </summary>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                _wpfWindowController.ShowFixWindow<Project1RevitHandler>((ExEvent) => { return new Project1UserControl(ExEvent, commandData.Application); }, "", WindowFit.FIX, null);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
