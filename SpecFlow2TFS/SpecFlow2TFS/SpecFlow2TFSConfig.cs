using System;
using System.Collections.Generic;
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
        public const string LIB = "Portal.Spec.dll";
    }
}
