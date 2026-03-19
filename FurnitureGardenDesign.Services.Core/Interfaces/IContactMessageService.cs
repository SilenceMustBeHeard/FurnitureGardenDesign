using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Interfaces
{
    public interface IContactMessageService
    {
        Task CreateMessageAsync(ContactMessage message);
    }
}
