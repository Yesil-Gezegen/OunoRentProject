using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.DTO.SubCategory.Request;

public sealed record UpdateSubCategoryRequest(Guid SubCategoryId, string Name, string Description, string Icon, int OrderNumber);

