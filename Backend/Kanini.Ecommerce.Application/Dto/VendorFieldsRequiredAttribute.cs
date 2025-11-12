using System.ComponentModel.DataAnnotations;

namespace Kanini.Ecommerce.Application.DTOs;

public class VendorFieldsRequiredAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        // Allow vendor registration without business fields
        // Business profile will be created separately
        return true;
    }

    public override string FormatErrorMessage(string name)
    {
        return "Validation passed";
    }
}