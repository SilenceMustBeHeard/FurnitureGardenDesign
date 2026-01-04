using System;
using System.Collections.Generic;
using System.Text;
using FurnitureGardenDesign.Data.Models;


namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface IOrderService
    { 
        Task<int> GetPendingOrdersCountAsync();

       
        Task<IEnumerable<OrderFormViewModel>> GetPendingOrdersAsync();

        // създаване на нова поръчка
        Task CreateOrderAsync(string userId, OrderFormViewModel model);

    }

}
