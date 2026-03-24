using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations.Interactions.Account
{
    public class RepositoryRegistrationException : Exception
    {
        public RepositoryRegistrationException(string message) : base(message) { }
    }

}
