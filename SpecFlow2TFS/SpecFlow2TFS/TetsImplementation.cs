using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecFlow2TFS
{
    public class TetsImplementation : ITmiTestImplementation
    {

        private string _storage;
        public string Storage
        {
            get
            {
                return _storage;
            }
            set
            {
                _storage = value;
            }
        }

        private Guid _testId;
        public Guid TestId
        {
            get
            {
                return _testId;
            }
            set
            {
                _testId = value;
            }
        }

        private string _testName;
        public string TestName
        {
            get
            {
                return _testName;
            }
            set
            {
                _testName = value;
            }
        }

        private string _testType;
        public string TestType
        {
            get
            {
                return _testType;
            }
            set
            {
                _testType = value;
            }
        }

        private Guid _testTypeId;
        public Guid TestTypeId
        {
            get
            {
                return _testTypeId;
            }
            set
            {
                _testTypeId = value;
            }
        }

        public string DisplayText
        {
            get { return _testName; }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
