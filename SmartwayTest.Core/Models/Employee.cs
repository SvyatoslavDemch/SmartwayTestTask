using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartwayTest.Core.Interfaces;

namespace SmartwayTest.Core.Models
{
    [Table("Employees")]
    public class Employee : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public Passport Passport { get; set; } = new Passport();
        public Department Department { get; set; } = new Department();
        public int? DepartmentId { get; set; }
    }
}
