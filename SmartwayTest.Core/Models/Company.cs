using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartwayTest.Core.Interfaces;

namespace SmartwayTest.Core.Models
{
    [Table("Companies")]
    public class Company : IEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Department>? Departments{ get; set; } = new List<Department>();
        public List<Employee>? Employees { get; set; } = new List<Employee>();  
    }
}
