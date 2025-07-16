using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using VitaCore.Models;

public sealed class AppUser : IdentityUser
{
    
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    
    public DateTime? Birthday { get; set; }

    public DateTime? LastLoginTime { get; set; }

    public PatientModel? Patient { get; set; }

    
}