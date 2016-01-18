using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecFlow2TFS
{
    public class SpecFlow2TFSConfig
    {
        public const string TFS_URL = "http://192.168.56.101:8080/tfs";
        public const string COLLECTION = @"192.168.56.101\Dream Team";
        public const string PROJECT = "NumberGenerator";
        public const string FEATURE_DIR = @"D:\data\bda20320\Documents\sourcecontrol\DreamTeam.NumberGenerator\NumberGenerator\Portal.Spec";
        public const string BASE_NAMESPACE = "Portal.Spec";
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            // 1. ask for a command
            // DT - delete all test cases that have one or more tags
            // IT - import all test cases that have one or more tags
            // I - import all test cases from source location
            
            // 1. ask for the URL of the TFS team project
            // 2. ask for the directory containing all the (directories with) feature files
            // 3. ask for the DLL containing the test methods

            // try to create and validate handlers for p1, p2 and p3

            // read all the existing test cases from TFS
            // (for import operations only) read the cases from the feature files
            // (for import operations only) read the methods from the DLL

            // ask confirmation from the user for the specific operations

            var TfsConnector = new TfsConnector();
            TfsConnector.LoadTestCases();

            var FeatureFileParser = new FeatureFileParser();
            FeatureFileParser.RetrieveAndIndexFeatureFiles();

            Console.WriteLine("End of Program. Press ENTER to quit.");
            Console.ReadLine();
        }
    }

    public class FeatureFileParser{

        static void DirSearch(string sDir, string ext, List<Tuple<string,string>> files, string ns)
        {
            files.AddRange(Directory.EnumerateFiles(sDir).Where(x => x.Substring(x.Length - ext.Length) == ext).Select(x => new Tuple<string,string>(ns, x)));
            foreach(var d in Directory.EnumerateDirectories(sDir)){
                var fn = Path.GetFileName(d);
                ns += "." + char.ToUpper(fn[0]) + fn.Substring(1);   
                DirSearch(d, ext, files, ns);
                ns = ns.Substring(0, ns.LastIndexOf("."));
            }
        }
        
        public void RetrieveAndIndexFeatureFiles()
        {
            var dirPth = SpecFlow2TFSConfig.FEATURE_DIR;
            if (!Directory.Exists(dirPth)) {
                throw new Exception("no it does not exist " + dirPth);
            }
            var files = new List<Tuple<string, string>>();
            DirSearch(dirPth, ".feature", files, SpecFlow2TFSConfig.BASE_NAMESPACE);

        }

    }

    public class TfsConnector
    {
        public TfsConnector()
        {
            
        }

        public ProjectCollection ProjectCollection;

        public void LoadTestCases()
        {
            var collectionUri = SpecFlow2TFSConfig.TFS_URL + "/" + SpecFlow2TFSConfig.COLLECTION.Substring(SpecFlow2TFSConfig.COLLECTION.LastIndexOf('\\') + 1);
            
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(collectionUri));

            WorkItemStore workItemStore = new WorkItemStore(tpc);

            Project project = null;

            foreach (Project p in workItemStore.Projects)
            {
                if (p.Name == SpecFlow2TFSConfig.PROJECT)
                {
                    project = p;
                    break;
                }
            }

            if (project == null)
            {
                throw new NullReferenceException("no project found for the name " + SpecFlow2TFSConfig.PROJECT);
            }

            // get test management service
            ITestManagementService2 test_service = (ITestManagementService2)tpc.GetService(typeof(ITestManagementService2));
            ITestManagementTeamProject2 test_project = test_service.GetTeamProject(project);

            // get the test cases from the project
            var allTestCasesOfProject = test_project.TestCases.Query("SELECT [Title] FROM WorkItems");

            List<ITestCase> testCases = new List<ITestCase>();
            foreach (ITestCase tc in allTestCasesOfProject)
            {
                testCases.Add(tc);
            }

            // http://blogs.msdn.com/b/densto/archive/2010/03/04/the-test-management-api-part-2-creating-modifying-test-plans.aspx
            // add a new testcase
            var newTestCase = test_project.TestCases.Create();
            newTestCase.Title = "This should be derived from the scenario title";
            newTestCase.WorkItem.Fields["Tags"].Value = "Waaaa";
            newTestCase.Save();


        }

        public class TeamProjectCollection
        {
            public string Name;
            public Guid Id;
            public List<TeamProject> Projects;
            public TeamProjectCollection()
            {
                Projects = new List<TeamProject>();
            }
        }

        public class TeamProject
        {
            public string Name;
            public Guid Id;
        }

        public class TestCase
        {
            public string AutomationStatus;

        }

        public class ProjectItem
        {
            private string Name;
            private WorkItemTypeCollection workItemTypeCollection;
            private string Uri;
            private QueryHierarchy queryHierarchy;

            public ProjectItem(string name, WorkItemTypeCollection workItemTypeCollection, string uri, QueryHierarchy queryHierarchy)
            {
                // TODO: Complete member initialization
                this.Name = name;
                this.workItemTypeCollection = workItemTypeCollection;
                this.Uri = uri;
                this.queryHierarchy = queryHierarchy;
            }
        }
    } 
}
