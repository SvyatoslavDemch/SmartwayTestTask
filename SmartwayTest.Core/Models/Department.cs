using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace SmartwayTest.Core.Models
{
    [Table("Departments")]
    public class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentPhone { get; set; }
        public int CompanyId { get; set; }
        public ICollection<Employee>? Employees { get; set; }
    }
}
