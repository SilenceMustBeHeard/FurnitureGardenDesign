using Furniture_GardenDesign.Data.Enums;
using FurnitureGardenDesign.Data.Common.Enums;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces;
using FurnitureGardenDesign.Services.Core.Admin.Interfaces;
using FurnitureGardenDesign.Web.ViewModels.DesignVariants;
using Microsoft.EntityFrameworkCore;

namespace FurnitureGardenDesign.Services.Core.Admin.Implementations
{
    public class DesignVariantService : IDesignVariantService
    {

        private readonly IDesignVariantRepository _designVariantRepository;
        private readonly IInboxMessageRepository _inboxMessageRepository;
        private readonly IOrderRepository _orderRepository;

        public DesignVariantService(IDesignVariantRepository designVariantRepository,
            IOrderRepository orderRepository, IInboxMessageRepository inboxMessageRepository)
        {
            _designVariantRepository = designVariantRepository;
            _orderRepository = orderRepository;
            _inboxMessageRepository = inboxMessageRepository;
        }



        // retrieves all design variants associated with a specific order
        public async Task<IEnumerable<DesignVariant>> GetDesignVariantsByOrderIdAsync(Guid orderId)
        {
            return await _designVariantRepository.GetByOrderId(orderId);
        }




        // retrieves a specific design variant by its id, including the associated order details
        public async Task<DesignVariant> GetDesignVariantByIdAsync(Guid id)
        {
            var designVariant = await _designVariantRepository
                .GetAllAttached()
                .Include(dv => dv.Order)
                .FirstOrDefaultAsync(dv => dv.Id == id);

            if (designVariant == null)
                throw new KeyNotFoundException($"Design variant with ID {id} not found.");

            return designVariant;
        }


        // creates a new design variant based on the provided view model
        // saves it to the database, and returns the created entity

        public async Task<DesignVariant> CreateDesignVariantAsync(DesignVariantViewModel model)
        {
            var entity = MapToEntity(model);

            await _designVariantRepository.AddAsync(entity);
            await _designVariantRepository.SaveChangesAsync();

            return entity;
        }



        // updates an existing design variant with new data from the provided view model
        // its used in second step of design process,
        // when designer can update the design variant with 3D model and notes before sending it to the customer for approval
        public async Task UpdateDesignVariantAsync(DesignVariant designVariant)
        {
            var existingDesignVariant = await _designVariantRepository.GetByIdAsync(designVariant.Id);
            if (existingDesignVariant == null)
            {
                throw new KeyNotFoundException($"Design variant with ID {designVariant.Id} not found.");
            }
            existingDesignVariant.Image2DUrl = designVariant.Image2DUrl;
            existingDesignVariant.Model3DUrl = designVariant.Model3DUrl;
            existingDesignVariant.Notes = designVariant.Notes;
            existingDesignVariant.IsApproved = designVariant.IsApproved;
            _designVariantRepository.Update(existingDesignVariant);
            await _designVariantRepository.SaveChangesAsync();
        }




        // sends a design variant proposal to the customer by creating an inbox message and updating the order status
        // this method is called when the designer is ready to send the design variant to the customer for approval
        public async Task SendDesignVariantProposalAsync(Guid designVariantId)
        {
            var designVariant = await _designVariantRepository
                .GetAllAttached()
                .Include(dv => dv.Order)
                .FirstOrDefaultAsync(dv => dv.Id == designVariantId);

            if (designVariant == null)
                throw new KeyNotFoundException("Design variant not found.");

            var recipientId = designVariant.Order.UserId;

         
            await _orderRepository.UpdateStatusAsync(designVariant.OrderId, OrderStatus.DesignProvided);

            var message = new InboxMessage
            {
                Id = Guid.NewGuid(),
                DesignVariantId = designVariantId,
                ReceiverId = recipientId,
                Type = InboxMessageType.DesignSent,
                IsRead = false,
                CreatedOn = DateTime.UtcNow
            };

            await _inboxMessageRepository.AddAsync(message);
            await _inboxMessageRepository.SaveChangesAsync();
        }



        // deletes a design variant by its id, ensuring that it exists before attempting deletion
        public async Task DeleteDesignVariantAsync(Guid id)
        {
            var designVariant = await _designVariantRepository.GetByIdAsync(id);
            if (designVariant == null)
            {
                throw new KeyNotFoundException($"Design variant with ID {id} not found.");
            }
            _designVariantRepository.Delete(designVariant);
            await _designVariantRepository.SaveChangesAsync();



        }

        // helper method to map a DesignVariantViewModel to a DesignVariant entity
        // this method is used when creating a new design variant from the view model
        private DesignVariant MapToEntity(DesignVariantViewModel model)
        {
            return new DesignVariant
            {
                Id = Guid.NewGuid(),
                OrderId = model.OrderId,
                Image2DUrl = model.Image2DUrl,
                Model3DUrl = model.Model3DUrl,
                Notes = model.Notes,
                IsApproved = false,


            };
        }


    }
}