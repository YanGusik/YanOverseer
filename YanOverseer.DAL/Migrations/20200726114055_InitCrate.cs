﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace YanOverseer.DAL.Migrations
{
    public partial class InitCrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildSettings",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    ModeratorRoleName = table.Column<string>(nullable: true),
                    AutoRoleName = table.Column<string>(nullable: true),
                    CreateMessageChannel = table.Column<ulong>(nullable: false),
                    UpdateMessageChannel = table.Column<ulong>(nullable: false),
                    DeleteMessageChannel = table.Column<ulong>(nullable: false),
                    AutoRole = table.Column<bool>(nullable: false),
                    AutoWelcomeMessage = table.Column<bool>(nullable: false),
                    AutoLogCreateMessage = table.Column<bool>(nullable: false),
                    AutoLogUpdateMessage = table.Column<bool>(nullable: false),
                    AutoLogDeleteMessage = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DiscordId = table.Column<ulong>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false),
                    CountTextMessage = table.Column<int>(nullable: false),
                    CountMessageWithImage = table.Column<int>(nullable: false),
                    CountMessageWithUrl = table.Column<int>(nullable: false),
                    Alias = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    ProfileId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ProfileId",
                table: "Messages",
                column: "ProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuildSettings");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
