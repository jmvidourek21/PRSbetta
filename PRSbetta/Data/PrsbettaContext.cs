using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PRSbetta.Models;

namespace PRSbetta.Data;

public partial class PrsbettaContext : DbContext
{
    public PrsbettaContext()
    {
    }

    public PrsbettaContext(DbContextOptions<PrsbettaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Lineitem> Lineitems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnections");

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<Lineitem>(entity =>
    //        {
    //            entity.HasKey(e => e.Id).HasName("PK__LINEITEM__3214EC278DE3EF6B");

    //            entity.HasOne(d => d.Product).WithMany(p => p.Lineitems)
    //                .OnDelete(DeleteBehavior.ClientSetNull)
    //                .HasConstraintName("FK_LineItem_Product");

    //            entity.HasOne(d => d.Request).WithOne(p => p.Lineitem)
    //                .OnDelete(DeleteBehavior.ClientSetNull)
    //                .HasConstraintName("FK_LineItem_Request");
    //        });

    //        modelBuilder.Entity<Product>(entity =>
    //        {
    //            entity.HasKey(e => e.Id).HasName("PK__PRODUCT__3214EC276D83EC1E");

    //            entity.HasOne(d => d.Vendor).WithOne(p => p.Product)
    //                .OnDelete(DeleteBehavior.ClientSetNull)
    //                .HasConstraintName("FK_Product_Vendor");
    //        });

    //        modelBuilder.Entity<Request>(entity =>
    //        {
    //            entity.HasKey(e => e.Id).HasName("PK__REQUEST__3214EC2762022539");

    //            entity.Property(e => e.Status).HasDefaultValue("NEW");
    //            entity.Property(e => e.Total).HasDefaultValue(0.0m);

    //            entity.HasOne(d => d.User).WithMany(p => p.Requests)
    //                .OnDelete(DeleteBehavior.ClientSetNull)
    //                .HasConstraintName("FK_Request_User");
    //        });

    //        modelBuilder.Entity<User>(entity =>
    //        {
    //            entity.HasKey(e => e.Id).HasName("PK__USER__3214EC277C7A0F71");
    //        });

    //        modelBuilder.Entity<Vendor>(entity =>
    //        {
    //            entity.HasKey(e => e.Id).HasName("PK__VENDOR__3214EC27E9440C2E");
    //        });

    //        OnModelCreatingPartial(modelBuilder);
    //    }

    //    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
