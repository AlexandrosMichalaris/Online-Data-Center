using System.ComponentModel.DataAnnotations;

namespace FileProcessing.Model;

public class FileStatusEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Status { get; set; }
}