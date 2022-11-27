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
    [Migration("20221113015606_DrinkIngredient")]
    partial class DrinkIngredient
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BaristaHome.Models.Announcement", b =>
                {
                    b.Property<int>("AnnouncementId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AnnouncementId"), 1L, 1);

                    b.Property<string>("AnnouncementText")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.HasKey("AnnouncementId");

                    b.HasIndex("StoreId");

                    b.ToTable("Announcements");
                });

            modelBuilder.Entity("BaristaHome.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"), 1L, 1);

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("ChecklistId")
                        .HasColumnType("int");

                    b.HasKey("CategoryId");

                    b.HasIndex("ChecklistId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("BaristaHome.Models.CategoryTask", b =>
                {
                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("StoreTaskId")
                        .HasColumnType("int");

                    b.Property<bool>("IsFinished")
                        .HasColumnType("bit");

                    b.HasKey("CategoryId", "StoreTaskId");

                    b.HasIndex("StoreTaskId");

                    b.ToTable("CategoryTask");
                });

            modelBuilder.Entity("BaristaHome.Models.Checklist", b =>
                {
                    b.Property<int>("ChecklistId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ChecklistId"), 1L, 1);

                    b.Property<string>("ChecklistTitle")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.HasKey("ChecklistId");

                    b.HasIndex("StoreId");

                    b.ToTable("Checklist");
                });

            modelBuilder.Entity("BaristaHome.Models.Drink", b =>
                {
                    b.Property<int>("DrinkId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DrinkId"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<byte[]>("DrinkImageData")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("DrinkName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("DrinkVideo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Instructions")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.HasKey("DrinkId");

                    b.HasIndex("StoreId");

                    b.ToTable("Drink");
                });

            modelBuilder.Entity("BaristaHome.Models.DrinkIngredient", b =>
                {
                    b.Property<int>("DrinkId")
                        .HasColumnType("int");

                    b.Property<int>("IngredientId")
                        .HasColumnType("int");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("unit")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.HasKey("DrinkId", "IngredientId");

                    b.HasIndex("IngredientId");

                    b.ToTable("DrinkIngredient");
                });

            modelBuilder.Entity("BaristaHome.Models.DrinkTag", b =>
                {
                    b.Property<int>("DrinkId")
                        .HasColumnType("int");

                    b.Property<int>("TagId")
                        .HasColumnType("int");

                    b.HasKey("DrinkId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("DrinkTag");
                });

            modelBuilder.Entity("BaristaHome.Models.Feedback", b =>
                {
                    b.Property<int>("FeedbackId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FeedbackId"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("FeedbackId");

                    b.HasIndex("StoreId");

                    b.HasIndex("UserId");

                    b.ToTable("Feedback");
                });

            modelBuilder.Entity("BaristaHome.Models.Ingredient", b =>
                {
                    b.Property<int>("IngredientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IngredientId"), 1L, 1);

                    b.Property<string>("IngredientName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("IngredientId");

                    b.ToTable("Ingredient");
                });

            modelBuilder.Entity("BaristaHome.Models.InventoryItem", b =>
                {
                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.Property<decimal>("PricePerUnit")
                        .HasPrecision(16, 2)
                        .HasColumnType("decimal(16,2)");

                    b.Property<decimal>("Quantity")
                        .HasPrecision(16, 2)
                        .HasColumnType("decimal(16,2)");

                    b.HasKey("ItemId", "StoreId");

                    b.HasIndex("StoreId");

                    b.ToTable("InventoryItem");
                });

            modelBuilder.Entity("BaristaHome.Models.Item", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ItemId"), 1L, 1);

                    b.Property<string>("ItemName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("UnitId")
                        .HasColumnType("int");

                    b.HasKey("ItemId");

                    b.HasIndex("UnitId");

                    b.ToTable("Item");
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

            modelBuilder.Entity("BaristaHome.Models.Sale", b =>
                {
                    b.Property<int>("SaleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SaleId"), 1L, 1);

                    b.Property<int>("InventoryItemItemId")
                        .HasColumnType("int");

                    b.Property<int>("InventoryItemStoreId")
                        .HasColumnType("int");

                    b.Property<decimal>("Profit")
                        .HasPrecision(16, 2)
                        .HasColumnType("decimal(16,2)");

                    b.Property<DateTime>("TimeSold")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("UnitsSold")
                        .HasPrecision(16, 2)
                        .HasColumnType("decimal(16,2)");

                    b.HasKey("SaleId");

                    b.HasIndex("InventoryItemItemId", "InventoryItemStoreId");

                    b.ToTable("Sale");
                });

            modelBuilder.Entity("BaristaHome.Models.Shift", b =>
                {
                    b.Property<int>("ShiftId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ShiftId"), 1L, 1);

                    b.Property<DateTime>("EndShift")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartShift")
                        .HasColumnType("datetime2");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("ShiftId");

                    b.HasIndex("StoreId");

                    b.HasIndex("UserId");

                    b.ToTable("Shift");
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
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<string>("StoreName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("StoreId");

                    b.ToTable("Store");
                });

            modelBuilder.Entity("BaristaHome.Models.StoreTask", b =>
                {
                    b.Property<int>("StoreTaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StoreTaskId"), 1L, 1);

                    b.Property<string>("TaskName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("StoreTaskId");

                    b.ToTable("StoreTask");
                });

            modelBuilder.Entity("BaristaHome.Models.StoreTimer", b =>
                {
                    b.Property<int>("StoreTimerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StoreTimerId"), 1L, 1);

                    b.Property<int>("DurationMin")
                        .HasColumnType("int");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.Property<string>("TimerName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("StoreTimerId");

                    b.HasIndex("StoreId");

                    b.ToTable("StoreTimer");
                });

            modelBuilder.Entity("BaristaHome.Models.Tag", b =>
                {
                    b.Property<int>("TagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TagId"), 1L, 1);

                    b.Property<string>("TagName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("TagId");

                    b.ToTable("Tag");
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

                    b.Property<string>("UserDescription")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("UserImage")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<byte[]>("UserImageData")
                        .HasColumnType("varbinary(max)");

                    b.Property<decimal?>("Wage")
                        .HasPrecision(16, 2)
                        .HasColumnType("decimal(16,2)");

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

            modelBuilder.Entity("BaristaHome.Models.Announcement", b =>
                {
                    b.HasOne("BaristaHome.Models.Store", "Store")
                        .WithMany("Announcement")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Store");
                });

            modelBuilder.Entity("BaristaHome.Models.Category", b =>
                {
                    b.HasOne("BaristaHome.Models.Checklist", "Checklist")
                        .WithMany()
                        .HasForeignKey("ChecklistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Checklist");
                });

            modelBuilder.Entity("BaristaHome.Models.CategoryTask", b =>
                {
                    b.HasOne("BaristaHome.Models.Category", "Category")
                        .WithMany("CategoryTasks")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BaristaHome.Models.StoreTask", "StoreTask")
                        .WithMany("CategoryTasks")
                        .HasForeignKey("StoreTaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("StoreTask");
                });

            modelBuilder.Entity("BaristaHome.Models.Checklist", b =>
                {
                    b.HasOne("BaristaHome.Models.Store", "Store")
                        .WithMany("Checklists")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Store");
                });

            modelBuilder.Entity("BaristaHome.Models.Drink", b =>
                {
                    b.HasOne("BaristaHome.Models.Store", "Store")
                        .WithMany("Drinks")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Store");
                });

            modelBuilder.Entity("BaristaHome.Models.DrinkIngredient", b =>
                {
                    b.HasOne("BaristaHome.Models.Drink", "Drink")
                        .WithMany("DrinkIngredients")
                        .HasForeignKey("DrinkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BaristaHome.Models.Ingredient", "Ingredient")
                        .WithMany("DrinkIngredients")
                        .HasForeignKey("IngredientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Drink");

                    b.Navigation("Ingredient");
                });

            modelBuilder.Entity("BaristaHome.Models.DrinkTag", b =>
                {
                    b.HasOne("BaristaHome.Models.Drink", "Drink")
                        .WithMany("DrinkTags")
                        .HasForeignKey("DrinkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BaristaHome.Models.Tag", "Tag")
                        .WithMany("DrinkTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Drink");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("BaristaHome.Models.Feedback", b =>
                {
                    b.HasOne("BaristaHome.Models.Store", "Store")
                        .WithMany("Feedbacks")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BaristaHome.Models.User", "User")
                        .WithMany("Feedbacks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Store");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BaristaHome.Models.InventoryItem", b =>
                {
                    b.HasOne("BaristaHome.Models.Item", "Item")
                        .WithMany("InventoryItems")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BaristaHome.Models.Store", "Store")
                        .WithMany("InventoryItems")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("Store");
                });

            modelBuilder.Entity("BaristaHome.Models.Item", b =>
                {
                    b.HasOne("BaristaHome.Models.Unit", "Unit")
                        .WithMany("Item")
                        .HasForeignKey("UnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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

            modelBuilder.Entity("BaristaHome.Models.Sale", b =>
                {
                    b.HasOne("BaristaHome.Models.InventoryItem", "InventoryItem")
                        .WithMany("Sale")
                        .HasForeignKey("InventoryItemItemId", "InventoryItemStoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("InventoryItem");
                });

            modelBuilder.Entity("BaristaHome.Models.Shift", b =>
                {
                    b.HasOne("BaristaHome.Models.Store", "Store")
                        .WithMany("Shifts")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BaristaHome.Models.User", "User")
                        .WithMany("Shifts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Store");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BaristaHome.Models.StoreTimer", b =>
                {
                    b.HasOne("BaristaHome.Models.Store", "Store")
                        .WithMany("StoreTimers")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Store");
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

            modelBuilder.Entity("BaristaHome.Models.Category", b =>
                {
                    b.Navigation("CategoryTasks");
                });

            modelBuilder.Entity("BaristaHome.Models.Drink", b =>
                {
                    b.Navigation("DrinkIngredients");

                    b.Navigation("DrinkTags");
                });

            modelBuilder.Entity("BaristaHome.Models.Ingredient", b =>
                {
                    b.Navigation("DrinkIngredients");
                });

            modelBuilder.Entity("BaristaHome.Models.InventoryItem", b =>
                {
                    b.Navigation("Sale");
                });

            modelBuilder.Entity("BaristaHome.Models.Item", b =>
                {
                    b.Navigation("InventoryItems");
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
                    b.Navigation("Announcement");

                    b.Navigation("Checklists");

                    b.Navigation("Drinks");

                    b.Navigation("Feedbacks");

                    b.Navigation("InventoryItems");

                    b.Navigation("Shifts");

                    b.Navigation("StoreTimers");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("BaristaHome.Models.StoreTask", b =>
                {
                    b.Navigation("CategoryTasks");
                });

            modelBuilder.Entity("BaristaHome.Models.Tag", b =>
                {
                    b.Navigation("DrinkTags");
                });

            modelBuilder.Entity("BaristaHome.Models.Unit", b =>
                {
                    b.Navigation("Item");
                });

            modelBuilder.Entity("BaristaHome.Models.User", b =>
                {
                    b.Navigation("Feedbacks");

                    b.Navigation("Payrolls");

                    b.Navigation("Shifts");

                    b.Navigation("UserShiftStatuses");
                });
#pragma warning restore 612, 618
        }
    }
}
