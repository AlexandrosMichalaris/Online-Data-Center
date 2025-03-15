using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageService.Model.Dto;

// Model for Hangfire Job table
[Table("Job", Schema = "Hangfire")]
public class HangfireJobDto
{
    [Key]
    public long Id { get; set; }
}