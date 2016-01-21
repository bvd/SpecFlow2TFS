
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
            var features = FeatureFile.RetrieveAndIndexFeatureFiles(SpecFlow2TFSConfig.FEATURE_DIR, SpecFlow2TFSConfig.BASE_NAMESPACE);
            var tests = new List<ImportTestCase>();
            foreach(var ff in features){
                foreach(var s in ff.Scenarios){
                    tests.Add(new ImportTestCase{
                        FeatureClass = ff.FeatureClass,
                        NameSpace = ff.NameSpace,
                        Description = s.Description,
                        Examples = s.Examples,
                        MethodName = s.MethodName,
                        Tags = s.Tags
                    });
                }
            }
            
            TfsConnector.CreateOrUpdateTestCases(tests);



            Console.WriteLine("End of Program. Press ENTER to quit.");
            Console.ReadLine();
        }
    }
}
