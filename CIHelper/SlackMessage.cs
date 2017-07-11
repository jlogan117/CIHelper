using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIHelper
{
    public class SlackMessage
    {
        public string stage { get; set; }
        public string passed { get; set; }
        public string failed { get; set; }

        public SlackMessage(string stage, string passed, string failed)
        {
            this.stage = stage;
            this.passed = passed;
            this.failed = failed;
        }
    }
}
