using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SQTJobPortal.Models
{
    public class ViewModel3
    {
        [Required]
     
        public string Title { get; set; }
        public string Description { get; set; }
        public string Experience { get; set; }
        public Nullable<System.DateTime> PostedDate { get; set; }
        public Nullable<System.DateTime> DueToApply { get; set; }
        public string Salary { get; set; }
        public string JobType { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<int> ProfessionId { get; set; }
        public int UserId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CompanyEmail { get; set; }

      

    }
}