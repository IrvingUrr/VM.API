﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using VM.Data.Models;

namespace VM.Data.Models.Configurations
{
    public partial class UserPurchaseConfiguration : IEntityTypeConfiguration<UserPurchase>
    {
        public void Configure(EntityTypeBuilder<UserPurchase> entity)
        {
            entity.ToTable("UserPurchase");

            entity.Property(e => e.Amount).HasColumnType("decimal(15, 3)");

            entity.Property(e => e.CurrencyIsoCode)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.PurchaseDate).HasColumnType("datetime");

            entity.HasOne(d => d.User)
                .WithMany(p => p.UserPurchases)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPurchase_UserId");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<UserPurchase> entity);
    }
}
