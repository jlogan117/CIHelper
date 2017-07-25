using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIHelper
{
    public class ApiReporter
    {
        public string pipeline { get; set; }
        public string stage { get; set; }
        public int passed { get; set; }
        public int failed { get; set; }
        public List<string> stageErrors { get; set; }
        public int duration { get; set; }
        public int buildNumber { get; set; }

        public int reportStage()
        {
            return 1;
        }
    }
}
