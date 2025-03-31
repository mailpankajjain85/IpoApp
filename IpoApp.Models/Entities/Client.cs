// Models/Client.cs
using System.ComponentModel.DataAnnotations;

public class Client
{
    public Guid ClientID { get; set; } = Guid.NewGuid();
    public string OrgShortCode { get; set; }
    public string ClientShortCode { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// DTOs/Client/ClientCreateRequest.cs
public class ClientCreateRequest
{
    [Required, StringLength(50)]
    public string OrgShortCode { get; set; }

    [Required, StringLength(20)]
    public string ClientShortCode { get; set; }

    [Required, StringLength(255)]
    public string FullName { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [Required, Phone]
    public string Mobile { get; set; }
}

// DTOs/Client/ClientResponse.cs
public class ClientResponse
{
    public Guid ClientID { get; set; }
    public string ClientShortCode { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}