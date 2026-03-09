using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Web.ViewModels.Account
{
        public class AccountMenuViewModel
        {
            public bool IsAuthenticated { get; set; }
            public string? Email { get; set; }
            public int UnreadMessagesCount { get; set; }
        }
    
}
 
    
