﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoundHouseFun.Data.Migrations
{
    public partial class AddActiveToCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Carts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Carts");
        }
    }
}
