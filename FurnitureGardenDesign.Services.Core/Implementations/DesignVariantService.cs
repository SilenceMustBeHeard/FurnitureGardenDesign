using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Implementations
{
    public class DesignVariantService : IDesignVariantService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IDesignVariantRepository _designVariantRepository;
        public DesignVariantService(
            IOrderRepository orderRepository,
            IDesignVariantRepository designVariantRepository)
        {
            _orderRepository = orderRepository;
            _designVariantRepository = designVariantRepository;
        }









        
        public Task AddDesignVariantAsync(Guid orderId, string imageUrl, string? notes)
        {
            throw new NotImplementedException();
        }

        public Task ApproveDesignVariantAsync(Guid designVariantId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDesignVariantAsync(Guid designVariantId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CatalogDesign>> GetAllActiveAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DesignVariant?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
