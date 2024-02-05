using FC.Codeflix.Catalog.Api.Models.Category;
using FC.Codeflix.Catalog.Api.Models.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Create.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Delete.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Get.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Category.List.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Category.Update.DTO;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ILogger<CategoriesController> _logger;
    private readonly IMediator _mediator;

    public CategoriesController(ILogger<CategoriesController> logger, 
                                IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CategoryOutputModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryInput input, CancellationToken cancellationToken)
    {
        var output = await _mediator.Send(input, cancellationToken);

        return CreatedAtAction(nameof(Create), new { output.Id }, new ApiResponse<CategoryOutputModel>(output));
    }
    
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryOutputModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update([FromRoute] Guid id,[FromBody] UpdateCategoryApiInput apiInput, CancellationToken cancellationToken)
    {
        var input = new UpdateCategoryInput(id, apiInput.Name, apiInput.Description, apiInput.IsActive);
        var output = await _mediator.Send(input, cancellationToken);

        return Ok(new ApiResponse<CategoryOutputModel>(output));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryOutputModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var output = await _mediator.Send(new GetCategoryInput(id), cancellationToken);

        return Ok(new ApiResponse<CategoryOutputModel>(output));
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseList<CategoryOutputModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        CancellationToken cancellationToken,
        [FromQuery] int? page,
        [FromQuery(Name = "per_page")] int? perPage,
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] SearchOrder? dir)
    {
        var input = new ListCategoriesInput
        {
            Page = page ?? 0,
            PerPage = perPage ?? 15,
            Search = search ?? "",
            Sort = sort ?? "",
            Dir = dir ?? SearchOrder.Asc
        };

        var output = await _mediator.Send(input, cancellationToken);

        return Ok(new ApiResponseList<CategoryOutputModel>(output));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
         _ = await _mediator.Send(new DeleteCategoryInput(id), cancellationToken);

        return NoContent();
    }
}