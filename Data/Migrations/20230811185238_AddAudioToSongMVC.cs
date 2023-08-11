using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoundHouseFun.Data.Migrations
{
    public partial class AddAudioToSongMVC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Audio",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Audio",
                table: "Songs");
        }
    }
}
