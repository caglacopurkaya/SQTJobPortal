﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SQTJobPortal.Models
{
   
    public class ViewModel2
    {
        public User companies { get; set; }
        public Category categories { get; set; }
        public Professions professions { get; set; }
        public User users { get; set; }
        public int UserId;
        public Job jobs;

        public List<JobRequest> requests { get; set; }
        public List<User> userss { get; set; }
        public List<Skills> skills{ get; set; }
        
        public JobRequest request { get; set; }

        public FileDetails files { get; set; }

        

    }
}