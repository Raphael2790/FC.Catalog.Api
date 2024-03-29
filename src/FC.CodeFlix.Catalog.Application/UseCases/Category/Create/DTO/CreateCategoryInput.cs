﻿using FC.CodeFlix.Catalog.Application.UseCases.Category.Common;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.Create.DTO;

public class CreateCategoryInput : IRequest<CategoryOutputModel>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }

    public CreateCategoryInput(string name, string? description = null, bool isActive = true)
    {
        Name = name;
        Description = description ?? "";
        IsActive = isActive;
    }
}
