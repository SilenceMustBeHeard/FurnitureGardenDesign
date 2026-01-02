using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Models
{
   
        public abstract class BaseDeletableEntity
        {
            public bool IsDeleted { get; set; }
            public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        }

    
}
