using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyList.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    DoorsTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    StartTimes = table.Column<List<TimeOnly>>(type: "time without time zone[]", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    Venue = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Artists = table.Column<List<string>>(type: "text[]", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    Recommended = table.Column<bool>(type: "boolean", nullable: false),
                    WillSellOut = table.Column<bool>(type: "boolean", nullable: false),
                    U21DrinkTix = table.Column<bool>(type: "boolean", nullable: false),
                    AllAges = table.Column<bool>(type: "boolean", nullable: false),
                    AgeMeta = table.Column<string>(type: "text", nullable: true),
                    PitWarning = table.Column<bool>(type: "boolean", nullable: false),
                    NoInsOuts = table.Column<bool>(type: "boolean", nullable: false),
                    SoldOut = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shows", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shows");
        }
    }
}
