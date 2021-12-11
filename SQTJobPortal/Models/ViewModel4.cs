using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQTJobPortal.Models
{
    public class ViewModel4
    {

      
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Profession { get; set; }
        public string Education { get; set; }
        public string MotivationLetter { get; set; }
        public Nullable<int> JobId { get; set; }
        public Nullable<int> JobSeekerId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> ApplyDate { get; set; }
        public Nullable<int> CvId { get; set; }


        public virtual Job Job { get; set; }
        public virtual User User { get; set; }
        public virtual FileDetails FileDetails { get; set; }
    }
}
