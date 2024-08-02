using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Shared.DTO.SubCategory.Request;

public sealed record CreateSubCategoryRequest(
    string Name,
    string Description,
    IFormFile Icon,
    int OrderNumber);