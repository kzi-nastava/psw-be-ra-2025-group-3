using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Explorer.Stakeholders.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "stakeholders");

            migrationBuilder.CreateTable(
                name: "Accounts",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppRatings",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRatings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClubJoinRequests",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    ClubId = table.Column<long>(type: "bigint", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubJoinRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meetups",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    TourId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meetups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Preferences",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    WalkingRating = table.Column<int>(type: "integer", nullable: false),
                    BicycleRating = table.Column<int>(type: "integer", nullable: false),
                    CarRating = table.Column<int>(type: "integer", nullable: false),
                    BoatRating = table.Column<int>(type: "integer", nullable: false),
                    Tags = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TouristRankRewards",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    GoldRewardClaimed = table.Column<bool>(type: "boolean", nullable: false),
                    PlatinumRewardClaimed = table.Column<bool>(type: "boolean", nullable: false),
                    DiamondRewardClaimed = table.Column<bool>(type: "boolean", nullable: false),
                    VistaRewardClaimed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TouristRankRewards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    BalanceAc = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    AmountAc = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ReferenceType = table.Column<string>(type: "text", nullable: true),
                    ReferenceId = table.Column<long>(type: "bigint", nullable: true),
                    InitiatorPersonId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WelcomeBonuses",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    BonusType = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WelcomeBonuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "People",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    ProfilePictureUrl = table.Column<string>(type: "text", nullable: true),
                    Biography = table.Column<string>(type: "text", nullable: true),
                    Quote = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                    table.ForeignKey(
                        name: "FK_People_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "stakeholders",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tourists",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<long>(type: "bigint", nullable: false),
                    EquipmentIds = table.Column<string>(type: "text", nullable: true),
                    XP = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tourists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tourists_People_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "stakeholders",
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Achievements",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<int>(type: "integer", nullable: false),
                    AwardedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Achievements_Tourists_TouristId",
                        column: x => x.TouristId,
                        principalSchema: "stakeholders",
                        principalTable: "Tourists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "XpEvents",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SourceEntityId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XpEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_XpEvents_Tourists_TouristId",
                        column: x => x.TouristId,
                        principalSchema: "stakeholders",
                        principalTable: "Tourists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClubImages",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClubId = table.Column<long>(type: "bigint", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClubImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clubs",
                schema: "stakeholders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<long>(type: "bigint", nullable: false),
                    FeaturedImageId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    MemberIds = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clubs_ClubImages_FeaturedImageId",
                        column: x => x.FeaturedImageId,
                        principalSchema: "stakeholders",
                        principalTable: "ClubImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_TouristId_AwardedAtUtc",
                schema: "stakeholders",
                table: "Achievements",
                columns: new[] { "TouristId", "AwardedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_TouristId_Code",
                schema: "stakeholders",
                table: "Achievements",
                columns: new[] { "TouristId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClubImages_ClubId",
                schema: "stakeholders",
                table: "ClubImages",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_ClubJoinRequests_TouristId_ClubId",
                schema: "stakeholders",
                table: "ClubJoinRequests",
                columns: new[] { "TouristId", "ClubId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clubs_FeaturedImageId",
                schema: "stakeholders",
                table: "Clubs",
                column: "FeaturedImageId");

            migrationBuilder.CreateIndex(
                name: "IX_People_UserId",
                schema: "stakeholders",
                table: "People",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Preferences_TouristId",
                schema: "stakeholders",
                table: "Preferences",
                column: "TouristId");

            migrationBuilder.CreateIndex(
                name: "IX_TouristRankRewards_TouristId",
                schema: "stakeholders",
                table: "TouristRankRewards",
                column: "TouristId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tourists_PersonId",
                schema: "stakeholders",
                table: "Tourists",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                schema: "stakeholders",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_PersonId",
                schema: "stakeholders",
                table: "Wallets",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_PersonId_CreatedAtUtc",
                schema: "stakeholders",
                table: "WalletTransactions",
                columns: new[] { "PersonId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_WelcomeBonuses_PersonId",
                schema: "stakeholders",
                table: "WelcomeBonuses",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_XpEvents_TouristId_CreatedAtUtc",
                schema: "stakeholders",
                table: "XpEvents",
                columns: new[] { "TouristId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_XpEvents_TouristId_Type_SourceEntityId",
                schema: "stakeholders",
                table: "XpEvents",
                columns: new[] { "TouristId", "Type", "SourceEntityId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClubImages_Clubs_ClubId",
                schema: "stakeholders",
                table: "ClubImages",
                column: "ClubId",
                principalSchema: "stakeholders",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClubImages_Clubs_ClubId",
                schema: "stakeholders",
                table: "ClubImages");

            migrationBuilder.DropTable(
                name: "Accounts",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "Achievements",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "AppRatings",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "ClubJoinRequests",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "Meetups",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "Preferences",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "TouristRankRewards",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "Wallets",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "WalletTransactions",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "WelcomeBonuses",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "XpEvents",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "Tourists",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "People",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "Clubs",
                schema: "stakeholders");

            migrationBuilder.DropTable(
                name: "ClubImages",
                schema: "stakeholders");
        }
    }
}
