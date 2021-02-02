using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.Models
{
    public class EmployeeVM
    {
        public String Id { get; set; }
        public String UserName { get; set; }
        public String Email { get; set; }
        public String PhoneNumber { get; set; }
        public String firstName { get; set; }
        public String lastName { get; set; }
        public String taxId { get; set; }
        public DateTime dateOfBirth { get; set; }
        public DateTime dateJoin { get; set; }
        public DateTime dateCreated { get; set; }
    }
}
