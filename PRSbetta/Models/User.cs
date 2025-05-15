﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace PRSbetta.Models;

[Table("USER")]
[Index("Username", Name = "uname", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string FirstName { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string LastName { get; set; } = null!;

    [StringLength(12)]
    [Unicode(false)]
    public string? PhoneNumber { get; set; } = null!;

    [StringLength(75)]
    [Unicode(false)]
    public string? Email { get; set; } = null!;

    public bool? Reviewer { get; set; }

    public bool? Admin { get; set; }

    //[InverseProperty("User")]
    //[JsonIgnore]
    //public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
