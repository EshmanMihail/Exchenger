using System;
using System.Collections.Generic;

namespace Exchenger.Models;

public partial class Customer
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int RoleId { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<Operation> Operations { get; set; } = new List<Operation>();

    public virtual CustomerRole Role { get; set; } = null!;
}
