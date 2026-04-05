using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Interfaces.Account
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model, ViewDataDictionary? viewData = null);
    }
}
