using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Interfaces
{
    public interface IOrderRepository
       : IRepository<Order, Guid>, IRepositoryAsync<Order, Guid>
    {
        Task<int> CountPendingAsync();
    }
}
