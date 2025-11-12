using System.ComponentModel.DataAnnotations;

namespace Kanini.Ecommerce.Application.DTOs;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
    public bool IsActive { get; set; }
    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public DateTime CreatedOn { get; set; }
    public List<CategoryDto> SubCategories { get; set; } = new();
}

public class CategoryCreateRequestDto
{
    [Required(ErrorMessage = "Category name is required")]
    [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    public int? ParentCategoryId { get; set; }

    [MaxLength(500, ErrorMessage = "Image path cannot exceed 500 characters")]
    public string? ImagePath { get; set; }
}

public class CategoryUpdateRequestDto
{
    [Required(ErrorMessage = "Category name is required")]
    [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    public int? ParentCategoryId { get; set; }

    [MaxLength(500, ErrorMessage = "Image path cannot exceed 500 characters")]
    public string? ImagePath { get; set; }

    public bool IsActive { get; set; } = true;
}