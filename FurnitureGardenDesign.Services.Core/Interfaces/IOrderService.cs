using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Web.ViewModels;
using FurnitureGardenDesign.Web.ViewModels.Orders;
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


        Task<DetailsOrderViewModel?> GetByIdAsync(Guid id);
        Task<bool> RejectOrderAsync(Guid id);

    }

}
