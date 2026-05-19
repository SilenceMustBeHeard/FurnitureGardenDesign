using System;
using System.Collections.Generic;
using System.Text;

namespace FurnitureGardenDesign.Api.Common;

public class ReviewRequest
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}