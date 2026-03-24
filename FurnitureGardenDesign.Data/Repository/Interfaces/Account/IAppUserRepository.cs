using FurnitureGardenDesign.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Interfaces.Account
{
   

    public interface IAppUserRepository
        : IRepository<AppUser, string>, IRepositoryAsync<AppUser, string>

    {
    }

}
