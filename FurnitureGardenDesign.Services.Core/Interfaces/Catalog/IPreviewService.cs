using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Services.Core.Interfaces.Catalog
{
    public interface IPreviewService
    {
        Task<(bool Success, string? ImageUrl, string? Message)> FetchWebpageImageAsync(string url);

    }
}
