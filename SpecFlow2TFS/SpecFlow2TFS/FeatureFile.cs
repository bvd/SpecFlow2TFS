using Gherkin.Ast;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpecFlow2TFS
{
    public class FeatureFile
    {
        
        public static List<FeatureFile> RetrieveAndIndexFeatureFiles(string baseDir, string baseNameSpace)
        {
            var r = new List<FeatureFile>();
            if (!Directory.Exists(baseDir))
            {
                throw new Exception("no it does not exist " + baseDir);
            }
            DirSearch(r, baseDir, ".feature", baseNameSpace);
            foreach (var ff in r)
            {
                ff.AddScenarios();
            }
            return r;
        }
        
        public static void DirSearch(List<FeatureFile> r, string sDir, string ext, string ns)
        {
            var newFiles = Directory.EnumerateFiles(sDir).Where(x => x.Substring(x.Length - ext.Length) == ext).Select(x => new FeatureFile { NameSpace = ns, FileFullPath = x });
            r.AddRange(newFiles);
            foreach (var d in Directory.EnumerateDirectories(sDir))
            {
                var fn = Path.GetFileName(d);
                ns += "." + char.ToUpper(fn[0]) + fn.Substring(1);
                DirSearch(r, d, ext, ns);
                ns = ns.Substring(0, ns.LastIndexOf("."));
            }
        }

        public string NameSpace { get; set; }

        public string FileFullPath { get; set; }

        public string FeatureClass { get; set; }

        private List<ParsedScenario> _scenarios;

        public List<ParsedScenario> Scenarios{
            get {
                if (null == _scenarios)
                {
                    _scenarios = new List<ParsedScenario>();
                }
                return _scenarios;
            }
        }

        public void AddScenarios()
        {
            var feature = GherkinParser.Parse(FileFullPath);
            FeatureClass = string.Join("", feature.Name.Split(' ').ToList().Select(y => y.Substring(0, 1).ToUpper() + y.Substring(1))) + "Feature";
            Scenarios.AddRange(feature.ScenarioDefinitions.Select(x => new ParsedScenario { 
                Tags = feature.Tags.ToList().Concat(x.Tags.ToList()).Distinct(new TagEqualityComparer()).Select(y => y.Name),
                MethodName = string.Join("", x.Name.Split(' ').ToList().Select(y => y.Substring(0, 1).ToUpper() + y.Substring(1))),
                Description = x.Steps.Select(y => y.Keyword + " " + y.Text).ToList(),
                Examples = RetrieveExamples(x)
            }));
        }

        private List<List<string>> RetrieveExamples(ScenarioDefinition sd)
        {
            if(!(sd is ScenarioOutline))
                return null;
            var so = sd as ScenarioOutline;
            var r = new List<List<string>>();
            r.Add(so.Examples.First().TableHeader.Cells.Select(x => x.Value).ToList());
            var vals = so.Examples.SelectMany(x => x.TableBody);
            foreach (var v in vals)
            {
                r.Add(v.Cells.Select(x => x.Value).ToList());
            }
            return r;
        }

        public static Gherkin.Parser GherkinParser
        {
            get
            {
                if (_parser == null)
                {
                    _parser = new Gherkin.Parser();
                }
                return _parser;
            }
        }

        private static Gherkin.Parser _parser;
    }


}
