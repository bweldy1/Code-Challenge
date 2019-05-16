using System;
using System.Collections.Generic;
using System.Linq;
using challenge.Models;
using System.Threading.Tasks;

namespace challenge.Repositories
{
    public interface ICompensationRepository
    {
        Compensation GetCompensationByEmployeeId(string id);
        Compensation Add(Compensation compensation);
        Task SaveAsync();
    }
}
