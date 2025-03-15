using System.ComponentModel.DataAnnotations;

namespace StorageService.Model.Domain;

public enum FileStatus
{
    Pending,
    Failed,
    Completed,
}