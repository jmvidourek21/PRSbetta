using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using PRSbetta.Data;



namespace PRSbetta.Models;

[Table("REQUEST")]
public partial class Request
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? RequestNumber { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Description { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? Justification { get; set; } = null!;

    [Column(TypeName ="Date")]
    public DateTime DateNeeded { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string? DeliveryMode { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? Status { get; set; } = "NEW";

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Total { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime SubmittedDate { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? ReasonForRejection { get; set; }

   //[InverseProperty("Request")]
    [JsonIgnore]
    public virtual ICollection<Lineitem>? Lineitem { get; set; } = new List<Lineitem>();

    //[ForeignKey("UserId")]
    //[InverseProperty("Requests")]
    //public virtual User? User { get; set; } = null!;
}



