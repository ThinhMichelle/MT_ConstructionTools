using Autodesk.Revit.DB;
using Dimension = Autodesk.Revit.DB.Dimension;

namespace MichelleTool
{
    public static class CreateDim
    {
        public static Autodesk.Revit.DB.Dimension CreateDimension(Document doc, Line line, ReferenceArray referenceArray, DimensionType dimensionType = null)
        {
            if (doc == null || line == null || referenceArray == null || referenceArray.Size < 2) return null;

            var view = doc.ActiveView;
            Autodesk.Revit.DB.Dimension dim = null;

            using (Transaction trans = new Transaction(doc, "Create Dimension"))
            {
                if (trans.Start() == TransactionStatus.Started)
                {
                    try
                    {
                        if (doc.IsFamilyDocument)
                        {
                            dim = dimensionType == null
                                ? doc.FamilyCreate.NewDimension(view, line, referenceArray)
                                : doc.FamilyCreate.NewDimension(view, line, referenceArray, dimensionType);
                        }
                        else
                        {
                            dim = dimensionType == null
                                ? doc.Create.NewDimension(view, line, referenceArray)
                                : doc.Create.NewDimension(view, line, referenceArray, dimensionType);
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.RollBack();
                        throw;
                    }
                }
            }
            return dim;
        }
    }
}
