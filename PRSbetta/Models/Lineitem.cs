using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PRSbetta.Models;

[Table("LINEITEM")]
[Index("RequestId", "ProductId", Name = "req_pdt", IsUnique = true)]
public partial class Lineitem
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("RequestID")]
    public int RequestId { get; set; }

    [Column("ProductID")]
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    
    public virtual Product? Product { get; set; } = null!;

   
    public virtual Request? Request { get; set; } = null!;
}
