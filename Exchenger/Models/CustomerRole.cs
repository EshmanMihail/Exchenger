using System;
using System.Collections.Generic;

namespace Exchenger.Models;

public partial class CustomerRole
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
