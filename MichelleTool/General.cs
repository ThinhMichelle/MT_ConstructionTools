using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MichelleTool
{
    public static class General
    {
        public static List<Reference> GetReferencesByType(this FamilyInstance element, FamilyInstanceReferenceType? reftype = null)
        {
            List<Reference> refs = new List<Reference>();
            var type = reftype ?? FamilyInstanceReferenceType.CenterLeftRight;
            var references = element.GetReferences(type);
            refs.AddRange(references);
            return refs;
        }

        public static Reference GetRefCenterLR(this FamilyInstance element)
        {
            List<Reference> refs = new List<Reference>();
            return element.GetReferences(FamilyInstanceReferenceType.CenterLeftRight).FirstOrDefault(); 
        }

        public static Reference GetRefCenterFB(this FamilyInstance element)
        {
            List<Reference> refs = new List<Reference>();
            return element.GetReferences(FamilyInstanceReferenceType.CenterFrontBack).FirstOrDefault();
        }

        public static Reference GetReferencesByName(this FamilyInstance element, string name)
        {
            return element.GetReferenceByName(name);
        }
    }
}
