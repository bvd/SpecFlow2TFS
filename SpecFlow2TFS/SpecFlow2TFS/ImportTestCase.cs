using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecFlow2TFS
{
    public class ImportTestCase
    {
        public string FeatureClass { get; set; }

        public string NameSpace { get; set; }

        public IEnumerable<string> Description { get; set; }

        public List<List<string>> Examples { get; set; }

        public string MethodName { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}
