using Gherkin.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecFlow2TFS
{
    public class TagEqualityComparer : IEqualityComparer<Tag>
    {
        public bool Equals(Tag a, Tag b)
        {
            if (a == null && b == null)
                return true;
            else if (a == null | b == null)
                return false;
            else return a.Name == b.Name;
        }

        public int GetHashCode(Tag t)
        {
            return t.Name.GetHashCode();
        }
    }
}
