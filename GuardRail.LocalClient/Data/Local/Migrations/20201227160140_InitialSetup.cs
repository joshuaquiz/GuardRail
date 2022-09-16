using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GuardRail.LocalClient.Data.Local.Migrations
{
    public partial class InitialSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "AccessPointGroups",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>("TEXT", nullable: false),
                    Name = table.Column<string>("TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPointGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "AccessPoints",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>("TEXT", nullable: false),
                    DeviceId = table.Column<string>("TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "Accounts",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>("TEXT", nullable: false),
                    Name = table.Column<string>("TEXT", nullable: true),
                    Location = table.Column<string>("TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "UserGroups",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Guid = table.Column<Guid>("TEXT", nullable: false),
                    Name = table.Column<string>("TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                "AccessPointAccessPointGroup",
                table => new
                {
                    AccessPointGroupsId = table.Column<int>("INTEGER", nullable: false),
                    AccessPointsId = table.Column<int>("INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPointAccessPointGroup", x => new { x.AccessPointGroupsId, x.AccessPointsId });
                    table.ForeignKey(
                        "FK_AccessPointAccessPointGroup_AccessPointGroups_AccessPointGroupsId",
                        x => x.AccessPointGroupsId,
                        "AccessPointGroups",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_AccessPointAccessPointGroup_AccessPoints_AccessPointsId",
                        x => x.AccessPointsId,
                        "AccessPoints",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Doors",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccessPointId = table.Column<int>("INTEGER", nullable: false),
                    Guid = table.Column<Guid>("TEXT", nullable: false),
                    DeviceId = table.Column<string>("TEXT", nullable: true),
                    FriendlyName = table.Column<string>("TEXT", nullable: true),
                    LockedStatus = table.Column<int>("INTEGER", nullable: false),
                    IsConfigured = table.Column<bool>("INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doors", x => x.Id);
                    table.ForeignKey(
                        "FK_Doors_AccessPoints_AccessPointId",
                        x => x.AccessPointId,
                        "AccessPoints",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Users",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountId = table.Column<int>("INTEGER", nullable: true),
                    Guid = table.Column<Guid>("TEXT", nullable: false),
                    Username = table.Column<string>("TEXT", nullable: true),
                    Password = table.Column<string>("TEXT", nullable: true),
                    FirstName = table.Column<string>("TEXT", nullable: true),
                    LastName = table.Column<string>("TEXT", nullable: true),
                    Phone = table.Column<string>("TEXT", nullable: true),
                    Email = table.Column<string>("TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        "FK_Users_Accounts_AccountId",
                        x => x.AccountId,
                        "Accounts",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "AccessPointGroupUserGroup",
                table => new
                {
                    AccessPointGroupsId = table.Column<int>("INTEGER", nullable: false),
                    UserGroupsId = table.Column<int>("INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPointGroupUserGroup", x => new { x.AccessPointGroupsId, x.UserGroupsId });
                    table.ForeignKey(
                        "FK_AccessPointGroupUserGroup_AccessPointGroups_AccessPointGroupsId",
                        x => x.AccessPointGroupsId,
                        "AccessPointGroups",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_AccessPointGroupUserGroup_UserGroups_UserGroupsId",
                        x => x.UserGroupsId,
                        "UserGroups",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AccessPointUserGroup",
                table => new
                {
                    AccessPointsId = table.Column<int>("INTEGER", nullable: false),
                    UserGroupsId = table.Column<int>("INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPointUserGroup", x => new { x.AccessPointsId, x.UserGroupsId });
                    table.ForeignKey(
                        "FK_AccessPointUserGroup_AccessPoints_AccessPointsId",
                        x => x.AccessPointsId,
                        "AccessPoints",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_AccessPointUserGroup_UserGroups_UserGroupsId",
                        x => x.UserGroupsId,
                        "UserGroups",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AccessPointGroupUser",
                table => new
                {
                    AccessPointGroupsId = table.Column<int>("INTEGER", nullable: false),
                    UsersId = table.Column<int>("INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPointGroupUser", x => new { x.AccessPointGroupsId, x.UsersId });
                    table.ForeignKey(
                        "FK_AccessPointGroupUser_AccessPointGroups_AccessPointGroupsId",
                        x => x.AccessPointGroupsId,
                        "AccessPointGroups",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_AccessPointGroupUser_Users_UsersId",
                        x => x.UsersId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AccessPointUser",
                table => new
                {
                    AccessPointsId = table.Column<int>("INTEGER", nullable: false),
                    UsersId = table.Column<int>("INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessPointUser", x => new { x.AccessPointsId, x.UsersId });
                    table.ForeignKey(
                        "FK_AccessPointUser_AccessPoints_AccessPointsId",
                        x => x.AccessPointsId,
                        "AccessPoints",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_AccessPointUser_Users_UsersId",
                        x => x.UsersId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Devices",
                table => new
                {
                    Id = table.Column<int>("INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>("INTEGER", nullable: true),
                    Guid = table.Column<Guid>("TEXT", nullable: false),
                    DeviceId = table.Column<string>("TEXT", nullable: true),
                    FriendlyName = table.Column<string>("TEXT", nullable: true),
                    ByteId = table.Column<byte[]>("BLOB", nullable: true),
                    IsConfigured = table.Column<bool>("INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        "FK_Devices_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "UserUserGroup",
                table => new
                {
                    UserGroupsId = table.Column<int>("INTEGER", nullable: false),
                    UsersId = table.Column<int>("INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUserGroup", x => new { x.UserGroupsId, x.UsersId });
                    table.ForeignKey(
                        "FK_UserUserGroup_UserGroups_UserGroupsId",
                        x => x.UserGroupsId,
                        "UserGroups",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_UserUserGroup_Users_UsersId",
                        x => x.UsersId,
                        "Users",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_AccessPointAccessPointGroup_AccessPointsId",
                "AccessPointAccessPointGroup",
                "AccessPointsId");

            migrationBuilder.CreateIndex(
                "IX_AccessPointGroupUser_UsersId",
                "AccessPointGroupUser",
                "UsersId");

            migrationBuilder.CreateIndex(
                "IX_AccessPointGroupUserGroup_UserGroupsId",
                "AccessPointGroupUserGroup",
                "UserGroupsId");

            migrationBuilder.CreateIndex(
                "IX_AccessPointUser_UsersId",
                "AccessPointUser",
                "UsersId");

            migrationBuilder.CreateIndex(
                "IX_AccessPointUserGroup_UserGroupsId",
                "AccessPointUserGroup",
                "UserGroupsId");

            migrationBuilder.CreateIndex(
                "IX_Devices_UserId",
                "Devices",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_Doors_AccessPointId",
                "Doors",
                "AccessPointId",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Users_AccountId",
                "Users",
                "AccountId");

            migrationBuilder.CreateIndex(
                "IX_UserUserGroup_UsersId",
                "UserUserGroup",
                "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "AccessPointAccessPointGroup");

            migrationBuilder.DropTable(
                "AccessPointGroupUser");

            migrationBuilder.DropTable(
                "AccessPointGroupUserGroup");

            migrationBuilder.DropTable(
                "AccessPointUser");

            migrationBuilder.DropTable(
                "AccessPointUserGroup");

            migrationBuilder.DropTable(
                "Devices");

            migrationBuilder.DropTable(
                "Doors");

            migrationBuilder.DropTable(
                "UserUserGroup");

            migrationBuilder.DropTable(
                "AccessPointGroups");

            migrationBuilder.DropTable(
                "AccessPoints");

            migrationBuilder.DropTable(
                "UserGroups");

            migrationBuilder.DropTable(
                "Users");

            migrationBuilder.DropTable(
                "Accounts");
        }
    }
}