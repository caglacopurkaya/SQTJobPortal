//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SQTJobPortal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FileDetails
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FileDetails()
        {
            this.JobRequest = new HashSet<JobRequest>();
        }
    
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Nullable<int> UserId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JobRequest> JobRequest { get; set; }
        public virtual User User { get; set; }
    }
}
