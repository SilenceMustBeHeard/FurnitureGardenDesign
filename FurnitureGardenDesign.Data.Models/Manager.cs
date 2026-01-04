using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Models
{
    public class Manager : BaseDeletableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;
    }
}
