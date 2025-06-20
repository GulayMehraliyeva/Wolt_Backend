using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface IRestaurantRepository : IBaseRepository<Restaurant>
    {
        Task<List<Restaurant>> GetAllPaginated(int page, int take);
        Task<IEnumerable<Restaurant>> SearchByNameAsync(string query);
        Task<Restaurant> GetByIdWithIncludesAsync(int id, Func<IQueryable<Restaurant>, IQueryable<Restaurant>> include);

    }
}
