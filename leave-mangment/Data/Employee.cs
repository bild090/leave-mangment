using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_mangment.Data
{
    public class Employee : IdentityUser
    {
        public String firstName { get; set; }
        public String lastName { get; set; }
        public String taxId { get; set; }
        public DateTime dateOfBirth { get; set; }
        public DateTime dateJoin { get; set; }
        public DateTime dateCreated { get; set; }
    }
}
