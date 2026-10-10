using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dimension
{
    public class ItemDim
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public ItemDim(long id, string name)
        {
            Id = id;
            Name = name;
        }
        public override string ToString()
        {
            return Name;
        }
    }

}
