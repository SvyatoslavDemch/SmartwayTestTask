﻿using SmartwayTest.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartwayTest.Core.Interfaces
{
    public interface ICompanyRepository
    {
        Task<int> CreateComapny(Company company);

        Task<Company> GetCompanyById(int companyId);
        
        Task<List<Employee>> GetEmployeesByCompanyId(int companyId);

        Task DeleteCompany(int companyId);

        Task UpdateComapny(Company company);
    }
}
