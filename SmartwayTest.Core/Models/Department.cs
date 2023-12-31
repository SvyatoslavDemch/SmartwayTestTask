﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using SmartwayTest.Core.Interfaces;

namespace SmartwayTest.Core.Models
{
    [Table("Departments")]
    public class Department : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int CompanyId { get; set; }
        public ICollection<Employee>? Employees { get; set; }
    }
}
