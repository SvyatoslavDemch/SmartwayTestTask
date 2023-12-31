﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartwayTest.Core.Interfaces;

namespace SmartwayTest.Core.Models
{
    [Table("Passports")]
    public class Passport : IEntity
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Number { get; set; }
        public int EmployeeId { get; set; }
    }
}
