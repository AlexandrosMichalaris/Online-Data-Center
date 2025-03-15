using System.ComponentModel.DataAnnotations;

namespace StorageService.Model.Dto;

public class HangfireStateDto
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }  // Foreign key to FileRecords

    [Required]
    public long JobId { get; set; } // Foreign key to Hangfire Job table
    
    [Required]
    public string FileName { get; set; }
    
    public string Reason { get; set; }
    
    [Required]
    public DateTimeOffset CreateDate { get; set; }
}