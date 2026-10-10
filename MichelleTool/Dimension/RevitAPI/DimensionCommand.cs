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

namespace Dimension
{
    [Transaction(TransactionMode.Manual)]
    public class DimensionCommand : IExternalCommand
    {
        private DimensionController _wpfWindowController = new DimensionController();
        /// <summary>
        /// External command mainline
        /// </summary>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                _wpfWindowController.ShowFixWindow<DimensionRevitHandler>((ExEvent) => { return new DimensionUserControl(ExEvent, commandData.Application); }, "", WindowFit.HEGHT_CONTENT, null);
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
