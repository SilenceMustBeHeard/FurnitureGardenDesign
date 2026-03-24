using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Web.ViewModels;
using FurnitureGardenDesign.Web.ViewModels.Admin.Order;
using FurnitureGardenDesign.Web.ViewModels.Orders;
using System;
using System.Collections.Generic;
using System.Text;


namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface IOrderService
    {
        
        // get count of pending orders for admin
        Task<int> GetPendingOrdersCountAsync();

        // get all pending orders for admin
        Task<IEnumerable<AdminOrderListViewModel>> GetPendingOrdersAsync();

        // create new order
        Task CreateOrderAsync(string userId, OrderFormViewModel model);

        // get order details by id for admin
        Task<DetailsOrderViewModel?> GetByIdAsync(Guid id);


        // reject order by id for admin
        Task<bool> RejectOrderAsync(Guid id);

    }

}
