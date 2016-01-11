using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecFlow2TFS
{
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
            TfsConnector.ConnectToProject();
            TfsConnector.LoadTestCases();

            Console.WriteLine("End of Program. Press ENTER to quit.");
            Console.ReadLine();
        }
    }

    public class TfsConnector
    {
        public string url;

        public TfsConfigurationServer server;

        public List<TeamProjectCollection> projectCollections;

        public Guid connectedProject;
        public Guid connectedProjectCollection;

        public string TfsTeamProjectUrl;

        public string collectionUri = "";
        
        public TfsConnector()
        {
            projectCollections = new List<TeamProjectCollection>();

            Console.WriteLine("What is the TFS team project URL?");
            TfsTeamProjectUrl = Console.ReadLine();
            Console.WriteLine("connecting to " + TfsTeamProjectUrl);

            // Connect to Team Foundation Server
            //     Server is the name of the server that is running the application tier for Team Foundation.
            //     Port is the port that Team Foundation uses. The default port is 8080.
            //     VDir is the virtual path to the Team Foundation application. The default path is tfs.


            Uri tfsUri = new Uri(TfsTeamProjectUrl);

            server = TfsConfigurationServerFactory.GetConfigurationServer(tfsUri);

            // Get the catalog of team project collections
            ReadOnlyCollection<CatalogNode> collectionNodes = server.CatalogNode.QueryChildren(
                new[] { CatalogResourceTypes.ProjectCollection },
                false, CatalogQueryOptions.None);

            // List the team project collections
            foreach (CatalogNode collectionNode in collectionNodes)
            {
                // Use the InstanceId property to get the team project collection
                Guid collectionId = new Guid(collectionNode.Resource.Properties["InstanceId"]);
                TfsTeamProjectCollection teamProjectCollection = server.GetTeamProjectCollection(collectionId);

                // Print the name of the team project collection
                Console.WriteLine("Collection: " + teamProjectCollection.Name);

                var myTpc = new TeamProjectCollection { 
                    Id = teamProjectCollection.InstanceId, 
                    Name = teamProjectCollection.Name
                };

                // Get a catalog of team projects for the collection
                ReadOnlyCollection<CatalogNode> projectNodes = collectionNode.QueryChildren(
                    new[] { CatalogResourceTypes.TeamProject },
                    false, CatalogQueryOptions.None);

                // List the team projects in the collection
                foreach (CatalogNode projectNode in projectNodes)
                {
                    Console.WriteLine(" Team Project: " + projectNode.Resource.DisplayName);
                    myTpc.Projects.Add(new TeamProject { 
                        Id = projectNode.Resource.Identifier,
                        Name = projectNode.Resource.DisplayName
                    });
                }

                projectCollections.Add(myTpc);
            }
        }

        public void ConnectToProject()
        {
            Console.WriteLine("What is the ProjectCollection name?");
            var ProjectCollectionName = Console.ReadLine();
            Console.WriteLine("What is the Project name?");
            var ProjectName = Console.ReadLine();

            var projectCollection = projectCollections
                .Where(x => x.Name == ProjectCollectionName);

            var projectGuid = projectCollection.SelectMany(x => x.Projects)
                .Where(x => x.Name == ProjectName)
                .Select(x => x.Id);

            if (0 == projectGuid.Count())
            {
                Console.WriteLine("that project does not exist.");
                return;
            }

            Console.WriteLine("you selected project with guid " + projectGuid.Single());
            connectedProject = projectGuid.Single();
            connectedProjectCollection = projectCollection.Single().Id;

            var name = projectCollection.Single().Name;

            collectionUri = TfsTeamProjectUrl + "/" + name.Substring(name.LastIndexOf('\\') + 1);
        }

        public void LoadTestCases()
        {
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(collectionUri));

            WorkItemStore workItemStore = new WorkItemStore(tpc);
            // Run a query.
            WorkItemCollection queryResults = workItemStore.Query(
               "Select [Title], [Tags], [Description] " +
               "From WorkItems " +
               "Where [Work Item Type] = 'Test Case' " +
               "Order By [State] Asc, [Changed Date] Desc");
            
            var tc = queryResults[0];
            var f = tc.Fields;



            foreach (Field fld in f)
            {
                string name = fld.Name;
                object val = fld.Value;
            }
            
            /*
            
            Dictionary<string, object> fld = new Dictionary<string, object>();

            foreach (var fldName in tc.Fields.)
            {
                fld = fldName.
            }*/
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
    } 
}
