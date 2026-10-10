using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tag
{
    public class TagAvailable : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            try
            {
                var doc = applicationData?.ActiveUIDocument?.Document;
                return doc != null ? true : false;
            }
            catch (Exception)
            { }
            return false;
        }
    }
}
