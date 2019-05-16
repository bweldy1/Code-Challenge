using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using challenge.Data;

namespace challenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<ICompensationRepository> _logger;

        public CompensationRepository(ILogger<ICompensationRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Compensation Add(Compensation compensation)
        {
            //check for duplicate
            //if(GetCompensationByEmployeeId(compensation.EmployeeId) != null)
            //{
            //    throw new ArgumentException($"Only one compensation record allowed per employee.  Duplicate compensation found for employee ID: {compensation.EmployeeId}");
            //}

            _employeeContext.Compensations.Add(compensation);
            return compensation;
        }

        public Compensation GetCompensationByEmployeeId(string id)
        {
            var comp = _employeeContext.Compensations.Where(c => c.EmployeeId == id).Include(e => e.Employee).FirstOrDefault();
            return comp;            
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }
    }
}
