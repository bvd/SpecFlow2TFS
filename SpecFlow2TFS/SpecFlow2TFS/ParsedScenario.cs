using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecFlow2TFS
{
    public class ParsedScenario
    {
        public IEnumerable<string> Tags { get; set; }

        public IEnumerable<string> Description { get; set; }

        public string MethodName { get; set; }

        public List<List<string>> Examples { get; set; }
    }
}
