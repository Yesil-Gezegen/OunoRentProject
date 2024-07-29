using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.DTO.Blog.Request;

public sealed record UpdateBlogRequest(
    Guid BlogId, Guid SubCategoryId, string Title,
    string LargeImgUrl, string SmallImgUrl,
    string Tags, string Slug,int OrderNumber,
     DateTime Date, Boolean IsActive);
