using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace PRSbetta.Models;

[Table("VENDOR")]
[Index("Code", Name = "Vcode", IsUnique = true)]
public partial class Vendor
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? Address { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? City { get; set; } = null!;

    [StringLength(2)]
    [Unicode(false)]
    public string? State { get; set; } = null!;

    [StringLength(5)]
    [Unicode(false)]
    public string? Zip { get; set; } = null!;

    [StringLength(12)]
    [Unicode(false)]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    //[InverseProperty("Vendor")]
    //[JsonIgnore]
    //public virtual ICollection <Product>? Product { get; set; } = new List<Product> ();
}
