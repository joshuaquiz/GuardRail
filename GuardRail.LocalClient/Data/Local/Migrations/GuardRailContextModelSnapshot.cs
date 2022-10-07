﻿// <auto-generated />

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace GuardRail.LocalClient.Data.Local.Migrations
{
    [DbContext(typeof(GuardRailContext))]
    partial class GuardRailContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("AccessPointAccessPointGroup", b =>
                {
                    b.Property<int>("AccessPointGroupsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccessPointsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("AccessPointGroupsId", "AccessPointsId");

                    b.HasIndex("AccessPointsId");

                    b.ToTable("AccessPointAccessPointGroup");
                });

            modelBuilder.Entity("AccessPointGroupUser", b =>
                {
                    b.Property<int>("AccessPointGroupsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UsersId")
                        .HasColumnType("INTEGER");

                    b.HasKey("AccessPointGroupsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("AccessPointGroupUser");
                });

            modelBuilder.Entity("AccessPointGroupUserGroup", b =>
                {
                    b.Property<int>("AccessPointGroupsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserGroupsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("AccessPointGroupsId", "UserGroupsId");

                    b.HasIndex("UserGroupsId");

                    b.ToTable("AccessPointGroupUserGroup");
                });

            modelBuilder.Entity("AccessPointUser", b =>
                {
                    b.Property<int>("AccessPointsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UsersId")
                        .HasColumnType("INTEGER");

                    b.HasKey("AccessPointsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("AccessPointUser");
                });

            modelBuilder.Entity("AccessPointUserGroup", b =>
                {
                    b.Property<int>("AccessPointsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserGroupsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("AccessPointsId", "UserGroupsId");

                    b.HasIndex("UserGroupsId");

                    b.ToTable("AccessPointUserGroup");
                });

            modelBuilder.Entity("GuardRail.LocalClient.Data.Models.AccessPoint", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DeviceId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Guid")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AccessPoints");
                });

            modelBuilder.Entity("GuardRail.LocalClient.Data.Models.AccessPointGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("Guid")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AccessPointGroups");
                });

            modelBuilder.Entity("GuardRail.LocalClient.Data.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("Guid")
                        .HasColumnType("TEXT");

                    b.Property<string>("Location")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("GuardRail.LocalClient.Data.Models.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("ByteId")
                        .HasColumnType("BLOB");

                    b.Property<string>("DeviceId")
                        .HasColumnType("TEXT");

                    b.Property<string>("FriendlyName")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Guid")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsConfigured")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("GuardRail.LocalClient.Data.Models.Door", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccessPointId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DeviceId")
                        .HasColumnType("TEXT");

                    b.Property<string>("FriendlyName")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Guid")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsConfigured")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DoorStatus")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AccessPointId")
                        .IsUnique();

                    b.ToTable("Doors");
                });

            modelBuilder.Entity("GuardRail.LocalClient.Data.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("AccountId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Guid")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("GuardRail.LocalClient.Data.Models.UserGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("Guid")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("UserGroups");
                });

            modelBuilder.Entity("UserUserGroup", b =>
                {
                    b.Property<int>("UserGroupsId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UsersId")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserGroupsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("UserUserGroup");
                });

            modelBuilder.Entity("AccessPointAccessPointGroup", b =>
                {
                    b.HasOne("GuardRail.LocalClient.Data.Models.AccessPointGroup", null)
                        .WithMany()
                        .HasForeignKey("AccessPointGroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GuardRail.LocalClient.Data.Models.AccessPoint", null)
                        .WithMany()
                        .HasForeignKey("AccessPointsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AccessPointGroupUser", b =>
                {
                    b.HasOne("GuardRail.LocalClient.Data.Models.AccessPointGroup", null)
                        .WithMany()
                        .HasForeignKey("AccessPointGroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GuardRail.LocalClient.Data.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AccessPointGroupUserGroup", b =>
                {
                    b.HasOne("GuardRail.LocalClient.Data.Models.AccessPointGroup", null)
                        .WithMany()
                        .HasForeignKey("AccessPointGroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GuardRail.LocalClient.Data.Models.UserGroup", null)
                        .WithMany()
                        .HasForeignKey("UserGroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AccessPointUser", b =>
                {
                    b.HasOne("GuardRail.LocalClient.Data.Models.AccessPoint", null)
                        .WithMany()
                        .HasForeignKey("AccessPointsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GuardRail.LocalClient.Data.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AccessPointUserGroup", b =>
                {
                    b.HasOne("GuardRail.LocalClient.Data.Models.AccessPoint", null)
                        .WithMany()
                        .HasForeignKey("AccessPointsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GuardRail.LocalClient.Data.Models.UserGroup", null)
                        .WithMany()
                        .HasForeignKey("UserGroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GuardRail.LocalClient.Data.Models.Device", b =>
                {
                    b.HasOne("GuardRail.LocalClient.Data.Models.User", "User")
                        .WithMany("Devices")
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GuardRail.LocalClient.Data.Models.Door", b =>
                {
                    b.HasOne("GuardRail.LocalClient.Data.Models.AccessPoint", "AccessPoint")
                        .WithOne("Door")
                        .HasForeignKey("GuardRail.LocalClient.Data.Models.Door", "AccessPointId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccessPoint");
                });

            modelBuilder.Entity("GuardRail.LocalClient.Data.Models.User", b =>
                {
                    b.HasOne("GuardRail.LocalClient.Data.Models.Account", "Account")
                        .WithMany("Users")
                        .HasForeignKey("AccountId");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("UserUserGroup", b =>
                {
                    b.HasOne("GuardRail.LocalClient.Data.Models.UserGroup", null)
                        .WithMany()
                        .HasForeignKey("UserGroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GuardRail.LocalClient.Data.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GuardRail.LocalClient.Data.Models.AccessPoint", b =>
                {
                    b.Navigation("Door");
                });

            modelBuilder.Entity("GuardRail.LocalClient.Data.Models.Account", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("GuardRail.LocalClient.Data.Models.User", b =>
                {
                    b.Navigation("Devices");
                });
#pragma warning restore 612, 618
        }
    }
}