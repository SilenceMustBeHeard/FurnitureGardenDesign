using System;
using System.Collections.Generic;
using System.Text;
using FurnitureGardenDesign.Data.Models;


namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface IOrderService
    { 
        Task<int> GetPendingOrdersCountAsync();

       
        Task<IEnumerable<Order>> GetPendingOrdersAsync();

        // create new order
        Task CreateOrderAsync(string userId, Order model);

    }

}
