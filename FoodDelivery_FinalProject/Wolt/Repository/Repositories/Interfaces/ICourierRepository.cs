using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface ICourierRepository : IBaseRepository<Courier>
    {
        Task<Courier> GetByUserIdAsync(string userId);
        Task<Courier> GetByIdWithUserAsync(int id);

    }
}
