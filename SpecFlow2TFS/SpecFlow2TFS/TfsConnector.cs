

namespace SpecFlow2TFS
{
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Framework.Client;
    using Microsoft.TeamFoundation.Framework.Common;
    using Microsoft.TeamFoundation.TestManagement.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Web;
    
    public class TfsConnector
    {
        public TfsConnector()
        {

        }

        public ProjectCollection ProjectCollection;

        private ITestManagementTeamProject2 GetTestProject()
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
            return test_project;
        }

        internal void CreateOrUpdateTestCases(IEnumerable<ImportTestCase> tests)
        {
            var test_project = GetTestProject();
            
            // get the test cases from the project
            var allTestCasesOfProject = test_project.TestCases.Query("SELECT [Title] FROM WorkItems");

            List<ITestCase> testCases = new List<ITestCase>();
            foreach (ITestCase tc in allTestCasesOfProject)
            {
                
                testCases.Add(tc);
            }




            // http://blogs.msdn.com/b/densto/archive/2010/03/04/the-test-management-api-part-2-creating-modifying-test-plans.aspx
            foreach (var t in tests)
            {
                
                
                var newTestCase = test_project.TestCases.Create();
                newTestCase.Title = t.MethodName;
                newTestCase.WorkItem.Fields["Tags"].Value = string.Join(",", t.Tags.Select(x => x.Substring(1)));
                newTestCase.Description = "";
                foreach (var s in t.Description)
                {
                    newTestCase.Description += HttpUtility.HtmlEncode(s) + "<br/>";
                }
                newTestCase.Description += "<br/>";

                var tab = "<table class=w3-table-all><tbody>";
                var row = "<tr>";
                var rowHeadCell = "<th>";
                var rowHeadCellEnd = "</th>";
                var rowEnd = "</tr>";
                var rowCell = "<td>";
                var rowCellEnd = "</td>";
                var tabEnd = "</tbody></table>";

                string h = null;

                if (t.Examples != null)
                {
                    newTestCase.Description += tab + row;
                    foreach (var e in t.Examples)
                    {
                        if (h == null)
                        {
                            foreach (var eh in e)
                            {
                                h += rowHeadCell + HttpUtility.HtmlEncode(eh) + rowHeadCellEnd;
                            }
                            newTestCase.Description += h + rowEnd;
                        }
                        else
                        {
                            newTestCase.Description += row;
                            foreach (var ed in e)
                            {
                                newTestCase.Description += rowCell + HttpUtility.HtmlEncode(ed) + rowCellEnd;
                            }
                            newTestCase.Description += rowEnd;
                        }
                    }
                    newTestCase.Description += tabEnd;
                }

                ITmiTestImplementation imp = new TetsImplementation { 
                    Storage = SpecFlow2TFSConfig.LIB,
                    TestId = Guid.NewGuid(),
                    TestName = t.NameSpace + t.MethodName,
                    TestType = "Unit Test"
                };

                newTestCase.Implementation = imp;

                newTestCase.WorkItem.Fields["Microsoft.VSTS.TCM.AutomationStatus"].Value = "Automated";
                newTestCase.WorkItem.Fields["Microsoft.VSTS.TCM.AutomatedTestName"].Value = imp.TestName;
                newTestCase.WorkItem.Fields["Microsoft.VSTS.TCM.AutomatedTestStorage"].Value = imp.Storage;
                newTestCase.WorkItem.Fields["Microsoft.VSTS.TCM.AutomatedTestId"].Value = imp.TestId.ToString();
                newTestCase.WorkItem.Fields["Microsoft.VSTS.TCM.AutomatedTestType"].Value = imp.TestType;

                ArrayList ValidationResult = newTestCase.WorkItem.Validate();

                newTestCase.Save();
            }
        }

        internal string ReadDescriptionTemplate(){
            var loc = Assembly.GetExecutingAssembly().Location;
            var dir = Path.GetDirectoryName(loc);
            var tplLoc = dir + @"\TestItemDescription.html";
            try
            {   
                using (StreamReader sr = new StreamReader(tplLoc))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                throw new Exception("The file " + tplLoc + " could not be read:");
            }
        }
    } 
}
