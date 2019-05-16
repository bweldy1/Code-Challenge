using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;

namespace challenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id, bool recurseReports = false)
        { 
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id, recurseReports);                
            }

            return null;
        }

        public ReportingStructure GetReportingStructure(string id)
        {
            var employee = this.GetById(id, true);

            var reportStructure = new ReportingStructure() { Employee = employee };

            //enter recursion to calculate all the reports
            reportStructure.NumberOfReports = CountReportsRecursive(employee);

            return reportStructure;
        }


        private int CountReportsRecursive(Employee employee)
        {
            if (employee == null)
            {
                //why are you sending me nulls?  c'mon man!
                throw new ArgumentNullException("employee");
            }
            var reportCount = 0;
            if (employee.DirectReports != null && employee.DirectReports.Count > 0)
            {
                //loop through all our reports and recurse each one
                foreach (var report in employee.DirectReports)
                {
                    //up the count
                    reportCount++;

                    reportCount += CountReportsRecursive(report);
                }
            }
            return reportCount;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                //get the supervisor so we can update the direct report later
                var supervisor = _employeeRepository.GetSupervisor(originalEmployee.EmployeeId);

                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;

                    //fix the supervisor direct report link now that we have a new object                    
                    if(supervisor != null)
                    {
                        supervisor.DirectReports.Remove(originalEmployee);
                        supervisor.DirectReports.Add(newEmployee);
                    }
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }
    }
}
