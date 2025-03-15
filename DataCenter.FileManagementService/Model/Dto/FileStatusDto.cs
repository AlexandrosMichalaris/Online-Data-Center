using System.ComponentModel.DataAnnotations;

namespace FileProcessing.Model;

public class FileStatusDto
{
    [Key]
    public int Id { get; set; }
    
    public string Status { get; set; }
}