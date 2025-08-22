using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelReview2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HotelReview2",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Property = table.Column<string>(type: "text", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelReview2", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HotelReview2Fts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    HotelReview2Id = table.Column<string>(type: "text", nullable: false),
                    TextA = table.Column<string>(type: "text", nullable: false),
                    VectorEn = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "english")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "TextA" }),
                    VectorFr = table.Column<NpgsqlTsVector>(type: "tsvector", nullable: false)
                        .Annotation("Npgsql:TsVectorConfig", "french")
                        .Annotation("Npgsql:TsVectorProperties", new[] { "TextA" })
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelReview2Fts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelReview2Fts_HotelReview2_HotelReview2Id",
                        column: x => x.HotelReview2Id,
                        principalTable: "HotelReview2",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HotelReview2Fts_HotelReview2Id",
                table: "HotelReview2Fts",
                column: "HotelReview2Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HotelReview2Fts_VectorEn",
                table: "HotelReview2Fts",
                column: "VectorEn")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_HotelReview2Fts_VectorFr",
                table: "HotelReview2Fts",
                column: "VectorFr")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HotelReview2Fts");

            migrationBuilder.DropTable(
                name: "HotelReview2");
        }
    }
}
