﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoundHouseFun.Data.Migrations
{
    public partial class AddedPriceToSong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Songs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Songs");
        }
    }
}
