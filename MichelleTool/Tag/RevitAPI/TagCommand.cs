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

namespace Tag
{
    [Transaction(TransactionMode.Manual)]
    public class TagCommand : IExternalCommand
    {
        private TagController _wpfWindowController = new TagController();
        /// <summary>
        /// External command mainline
        /// </summary>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                _wpfWindowController.ShowFixWindow<TagRevitHandler>((ExEvent) => { return new TagUserControl(ExEvent, commandData.Application); }, "", WindowFit.FIX, null);
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
