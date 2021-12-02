using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQTJobPortal.Models
{
    public class ViewModel
    {
        public User companies { get; set; }

        public Category categories { get; set; }
        public Job jobs { get; set; }

        public JobRequest request { get; set; }


        public User user { get; set; }


    }
}