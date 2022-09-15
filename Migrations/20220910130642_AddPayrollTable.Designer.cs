﻿// <auto-generated />
using System;
using BaristaHome.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BaristaHome.Migrations
{
    [DbContext(typeof(BaristaHomeContext))]
    [Migration("20220910130642_AddPayrollTable")]
    partial class AddPayrollTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BaristaHome.Models.InventoryItem", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ItemId"), 1L, 1);

                    b.Property<string>("ItemName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<decimal>("PricePerUnit")
                        .HasPrecision(16, 2)
                        .HasColumnType("decimal(16,2)");

                    b.Property<decimal>("Quantity")
                        .HasPrecision(16, 2)
                        .HasColumnType("decimal(16,2)");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.Property<int>("UnitId")
                        .HasColumnType("int");

                    b.HasKey("ItemId");

                    b.HasIndex("StoreId");

                    b.HasIndex("UnitId");

                    b.ToTable("InventoryItem");
                });

            modelBuilder.Entity("BaristaHome.Models.Payroll", b =>
                {
                    b.Property<int>("PayrollId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PayrollId"), 1L, 1);

                    b.Property<decimal>("Amount")
                        .HasPrecision(16, 2)
                        .HasColumnType("decimal(16,2)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("Date");

                    b.Property<decimal>("Hours")
                        .HasPrecision(16, 2)
                        .HasColumnType("decimal(16,2)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("PayrollId");

                    b.HasIndex("UserId");

                    b.ToTable("Payroll");
                });

            modelBuilder.Entity("BaristaHome.Models.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleId"), 1L, 1);

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("RoleId");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("BaristaHome.Models.ShiftStatus", b =>
                {
                    b.Property<int>("ShiftStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ShiftStatusId"), 1L, 1);

                    b.Property<string>("ShiftStatusName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("ShiftStatusId");

                    b.ToTable("ShiftStatus");
                });

            modelBuilder.Entity("BaristaHome.Models.Store", b =>
                {
                    b.Property<int>("StoreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StoreId"), 1L, 1);

                    b.Property<string>("StoreInviteCode")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<string>("StoreName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.HasKey("StoreId");

                    b.ToTable("Store");
                });

            modelBuilder.Entity("BaristaHome.Models.Unit", b =>
                {
                    b.Property<int>("UnitId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UnitId"), 1L, 1);

                    b.Property<string>("UnitName")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.HasKey("UnitId");

                    b.ToTable("Unit");
                });

            modelBuilder.Entity("BaristaHome.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"), 1L, 1);

                    b.Property<string>("Color")
                        .HasMaxLength(7)
                        .HasColumnType("nvarchar(7)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("InviteCode")
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RoleId")
                        .HasColumnType("int");

                    b.Property<int?>("StoreId")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.HasIndex("RoleId");

                    b.HasIndex("StoreId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("BaristaHome.Models.UserShiftStatus", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("ShiftStatusId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId", "ShiftStatusId", "Time");

                    b.HasIndex("ShiftStatusId");

                    b.ToTable("UserShiftStatus");
                });

            modelBuilder.Entity("BaristaHome.Models.InventoryItem", b =>
                {
                    b.HasOne("BaristaHome.Models.Store", "Store")
                        .WithMany("InventoryItems")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BaristaHome.Models.Unit", "Unit")
                        .WithMany("InventoryItems")
                        .HasForeignKey("UnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Store");

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("BaristaHome.Models.Payroll", b =>
                {
                    b.HasOne("BaristaHome.Models.User", "User")
                        .WithMany("Payrolls")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BaristaHome.Models.User", b =>
                {
                    b.HasOne("BaristaHome.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");

                    b.HasOne("BaristaHome.Models.Store", "Store")
                        .WithMany("Users")
                        .HasForeignKey("StoreId");

                    b.Navigation("Role");

                    b.Navigation("Store");
                });

            modelBuilder.Entity("BaristaHome.Models.UserShiftStatus", b =>
                {
                    b.HasOne("BaristaHome.Models.ShiftStatus", "ShiftStatus")
                        .WithMany("UserShiftStatuses")
                        .HasForeignKey("ShiftStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BaristaHome.Models.User", "User")
                        .WithMany("UserShiftStatuses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShiftStatus");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BaristaHome.Models.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("BaristaHome.Models.ShiftStatus", b =>
                {
                    b.Navigation("UserShiftStatuses");
                });

            modelBuilder.Entity("BaristaHome.Models.Store", b =>
                {
                    b.Navigation("InventoryItems");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("BaristaHome.Models.Unit", b =>
                {
                    b.Navigation("InventoryItems");
                });

            modelBuilder.Entity("BaristaHome.Models.User", b =>
                {
                    b.Navigation("Payrolls");

                    b.Navigation("UserShiftStatuses");
                });
#pragma warning restore 612, 618
        }
    }
}
