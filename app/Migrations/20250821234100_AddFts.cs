using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class AddFts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "VectorEn",
                table: "HotelReview",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "Title", "Text" });

            migrationBuilder.CreateIndex(
                name: "IX_HotelReview_VectorEn",
                table: "HotelReview",
                column: "VectorEn")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HotelReview_VectorEn",
                table: "HotelReview");

            migrationBuilder.DropColumn(
                name: "VectorEn",
                table: "HotelReview");
        }
    }
}
