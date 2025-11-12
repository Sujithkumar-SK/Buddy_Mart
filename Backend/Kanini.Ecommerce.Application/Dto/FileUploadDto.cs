namespace Kanini.Ecommerce.Application.DTOs;

public class FileUploadDto
{
    public string FileName { get; set; } = string.Empty;
    public long Length { get; set; }
    public Stream Content { get; set; } = null!;
    public string ContentType { get; set; } = string.Empty;
}