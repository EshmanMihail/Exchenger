using System;
using System.Collections.Generic;

namespace Exchenger.Models;

public partial class Currency
{
    public int CurId { get; set; }

    public string CurName { get; set; } = null!;

    public string? CurAbbreviation { get; set; }

    public int? CurScale { get; set; }

    public int? CurPeriodicity { get; set; }

    public double? CurOfficialRate { get; set; }

    public virtual ICollection<CurrencyAmount> CurrencyAmounts { get; set; } = new List<CurrencyAmount>();

    public virtual ICollection<Operation> OperationDepositedСurrencies { get; set; } = new List<Operation>();

    public virtual ICollection<Operation> OperationReceivedCurrencies { get; set; } = new List<Operation>();
}
