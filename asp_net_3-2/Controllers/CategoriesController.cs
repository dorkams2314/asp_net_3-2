using Microsoft.AspNetCore.Authorization;
using asp_net_3_2.Security;
using asp_net_3_2.DTOs.Categories;
using asp_net_3_2.Services;
using Microsoft.AspNetCore.Mvc;

namespace asp_net_3_2.Controllers;

[Authorize(Policy = Policies.Admin)]
[Route("api/categories")]
public sealed class CategoriesController(IService<CategoryDto, CategoryWriteDto, CategoryWriteDto, PatchCategoryDto> service)
    : CrudController<CategoryDto, CategoryWriteDto, CategoryWriteDto, PatchCategoryDto>(service)
{
}
