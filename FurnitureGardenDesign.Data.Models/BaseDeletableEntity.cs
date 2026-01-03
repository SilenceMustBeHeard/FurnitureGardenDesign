using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Data.Models
{
         // helper class for flagging for deletion
        public abstract class BaseDeletableEntity   
        {
            public bool IsDeleted { get; set; }
            public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        }

    
}
