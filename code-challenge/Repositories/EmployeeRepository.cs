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
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRepository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }
        
        public Employee GetById(string id, bool recurseReports)
        {
            var employee = _employeeContext.Employees.FirstOrDefault(e => e.EmployeeId == id);
            if(recurseReports && employee != null)
            {
                employee = GetEmployeeRecursive(employee);
            }

            return employee;           
        }

        public Employee GetSupervisor(string id)
        {
            //for a given employee, find its immediate supervisor
            //since we don't store a reference table, search through all the employees
            foreach(var e in _employeeContext.Employees)
            {
                if (e.DirectReports != null)
                {
                    foreach (var d in e.DirectReports)
                    {
                        if (d.EmployeeId == id)
                        {
                            //get the full list of reports
                            return GetEmployeeRecursive(e);
                        }
                    }
                }
            }

            return null;
        }

        private Employee GetEmployeeRecursive(Employee employee)
        {
            _employeeContext.Entry(employee).Collection(r => r.DirectReports).Load();

            foreach(var report in employee.DirectReports)
            {
                GetEmployeeRecursive(report);
            }

            return employee;
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
