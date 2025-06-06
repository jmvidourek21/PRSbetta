﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PRSbetta.Models;

[Table("PRODUCT")]
[Index("VendorId", "PartNumber", Name = "vendor_part", IsUnique = true)]
public partial class Product
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("VendorID")]
    public int VendorId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string PartNumber { get; set; } = null!;

    [StringLength(150)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Unit { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? PhotoPath { get; set; }

    //[InverseProperty("Product")]
    //public virtual ICollection<Lineitem> Lineitems { get; set; } = new List<Lineitem>();

    
    public virtual Vendor? Vendor { get; set; } = null!;
}
