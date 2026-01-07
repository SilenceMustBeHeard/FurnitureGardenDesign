using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;


namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface IOrderService
    { 
        Task<int> GetPendingOrdersCountAsync();

       
        Task<IEnumerable<Order>> GetPendingOrdersAsync();

        // create new order
        Task CreateOrderAsync(string userId, OrderFormViewModel model);

    }

}
