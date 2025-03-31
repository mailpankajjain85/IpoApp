// Models/User.cs
using System.ComponentModel.DataAnnotations;

public class User
{
    public Guid UserID { get; set; } = Guid.NewGuid();
    public Guid? ClientID { get; set; }
    public string OrgShortCode { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public string Role { get; set; } // 'SUPERADMIN', 'ADMIN', 'STAFF', 'CLIENT'
    public string Phone { get; set; }
    public bool IsClient { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime LastLogin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// DTOs/User/UserCreateRequest.cs
public class UserCreateRequest
{
    [Required]
    public string OrgShortCode { get; set; }

    [Required, StringLength(50)]
    public string Username { get; set; }

    [Required, StringLength(100)]
    public string FullName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, StringLength(100, MinimumLength = 8)]
    public string Password { get; set; }
    public string Phone { get; set; }
    public Guid? ClientID { get; set; } // Required only for CLIENT role

    [Required]
    [RegularExpression("^(SUPERADMIN|ADMIN|STAFF|CLIENT)$")]
    public string Role { get; set; }
}

// DTOs/User/UserResponse.cs
public class UserResponse
{
    public Guid UserID { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime LastLogin { get; set; }
    public Guid? ClientID { get; set; }
    public string ClientShortCode { get; set; } // Added from join
}