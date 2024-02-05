using FC.Codeflix.Catalog.Api.Models.Genre;
using FC.Codeflix.Catalog.Api.Models.Response;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Create.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Delete.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Get.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.List.DTO;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.Update.DTO;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GenresController : ControllerBase
{
    private readonly ILogger<GenresController> _logger;
    private readonly IMediator _mediator;
    
    public GenresController(ILogger<GenresController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseList<GenreOutputModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        CancellationToken cancellationToken,
        [FromQuery] int? page,
        [FromQuery(Name = "per_page")] int? perPage,
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] SearchOrder? dir)
    {
        var input = new ListGenresInput()
        {
            Page = page ?? 0,
            PerPage = perPage ?? 15,
            Search = search ?? "",
            Sort = sort ?? "",
            Dir = dir ?? SearchOrder.Asc
        };

        var output = await _mediator.Send(input, cancellationToken);

        return Ok(new ApiResponseList<GenreOutputModel>(output));
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GenreOutputModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var output = await _mediator.Send(new GetGenreInput(id), cancellationToken);

        return Ok(new ApiResponse<GenreOutputModel>(output));
    }
    
    [HttpPost()]
    [ProducesResponseType(typeof(ApiResponse<GenreOutputModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGenre([FromBody] CreateGenreInput input, CancellationToken cancellationToken)
    {
        var output = await _mediator.Send(input, cancellationToken);

        return CreatedAtAction(nameof(GetById), new {id = output.Id},new ApiResponse<GenreOutputModel>(output));
    }
    
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<GenreOutputModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateGenre([FromRoute] Guid id,[FromBody] UpdateGenreApiInput apiInput, CancellationToken cancellationToken)
    {
        var output = await _mediator.Send(new UpdateGenreInput(id, apiInput.Name, apiInput.IsActive, apiInput.CategoriesIds), cancellationToken);

        return Ok(new ApiResponse<GenreOutputModel>(output));
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteGenreInput(id), cancellationToken);

        return NoContent();
    }
}