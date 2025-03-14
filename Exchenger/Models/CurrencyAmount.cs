using System;
using System.Collections.Generic;

namespace Exchenger.Models;

public partial class CurrencyAmount
{
    public int Id { get; set; }

    public int CurrencyId { get; set; }

    public int? Amount { get; set; }

    public virtual Currency Currency { get; set; } = null!;
}
