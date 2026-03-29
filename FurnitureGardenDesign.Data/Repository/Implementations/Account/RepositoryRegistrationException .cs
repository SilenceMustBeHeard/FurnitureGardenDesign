using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Repository.Implementations.Account
{
    public class RepositoryRegistrationException : Exception
    {
      

        public RepositoryRegistrationException(string message) : base(message) { }

        public RepositoryRegistrationException() : base()
        {
        }

        public RepositoryRegistrationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

}
