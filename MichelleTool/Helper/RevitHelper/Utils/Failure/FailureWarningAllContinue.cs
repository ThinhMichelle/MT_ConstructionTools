using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichelleTool
{
    public class FailureWarningAllContinue : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            IList<FailureMessageAccessor> fmas = failuresAccessor.GetFailureMessages();
            if (fmas.Count == 0) return FailureProcessingResult.Continue;
            foreach (FailureMessageAccessor fma in fmas)
            {
                FailureSeverity failureSeverity = fma.GetSeverity();
                if (failureSeverity == FailureSeverity.Warning)
                {
                    failuresAccessor.DeleteWarning(fma);
                    continue;
                }
                else if (failureSeverity == FailureSeverity.Error)
                {
                    if (failuresAccessor.IsElementsDeletionPermitted(fma.GetFailingElementIds().ToList()))
                    {
                        failuresAccessor.DeleteElements(fma.GetFailingElementIds().ToList());
                    }
                }
                return FailureProcessingResult.ProceedWithRollBack;
            }
            return FailureProcessingResult.Continue;
        }

    }
}