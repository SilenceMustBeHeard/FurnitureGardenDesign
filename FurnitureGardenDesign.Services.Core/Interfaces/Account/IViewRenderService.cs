using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FurnitureGardenDesign.Services.Core.Interfaces.Account
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model, ViewDataDictionary? viewData = null);
    }
}