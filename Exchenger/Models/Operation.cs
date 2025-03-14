using System;
using System.Collections.Generic;

namespace Exchenger.Models;

public partial class Operation
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int DepositedСurrencyId { get; set; }

    public int ReceivedCurrencyId { get; set; }

    public int AmountOfDeposited { get; set; }

    public int AmountOfReceived { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Currency DepositedСurrency { get; set; } = null!;

    public virtual Currency ReceivedCurrency { get; set; } = null!;
}
