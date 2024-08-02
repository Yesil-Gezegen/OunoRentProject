using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Shared.DTO.Blog.Request;

public sealed record UpdateBlogRequest(
    Guid BlogId,
    Guid SubCategoryId,
    string Title,
    string Tags,
    string Slug,
    int OrderNumber,
    DateTime Date,
    Boolean IsActive,
    IFormFile? LargeImage,
    IFormFile? SmallImage);