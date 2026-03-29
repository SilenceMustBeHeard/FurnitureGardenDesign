using System;
using System.Collections.Generic;
using System.Text;
using FurnitureGardenDesign.Data.Models;
using FurnitureGardenDesign.Data.Repository.Interfaces.Account;


namespace FurnitureGardenDesign.Data.Repository.Implementations.Account
{
    public class AppUserRepository
        : BaseRepository<AppUser, string>, IAppUserRepository
    {
        public AppUserRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }

}
