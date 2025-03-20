using System.ComponentModel.DataAnnotations;

namespace FileProcessing.Model;

public class FileStatusEntity
{
    [Key]
    public int Id { get; set; }
    
    public string Status { get; set; }
}