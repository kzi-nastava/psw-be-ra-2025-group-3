using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Explorer.Tours.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tours");

            migrationBuilder.CreateTable(
                name: "AwardEvents",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    VotingStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    VotingEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AwardEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bundles",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TourIds = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bundles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TourId = table.Column<long>(type: "bigint", nullable: true),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Diaries",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: true),
                    TouristId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Equipment",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Facilities",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupTourSessions",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    TourName = table.Column<string>(type: "text", nullable: false),
                    ClubId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    StarterId = table.Column<long>(type: "bigint", nullable: false),
                    IsHighlighted = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupTourSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monuments",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecipientId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    RelatedEntityId = table.Column<long>(type: "bigint", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    TouristId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourIds = table.Column<string>(type: "jsonb", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourExecutions",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartLatitude = table.Column<double>(type: "double precision", nullable: false),
                    StartLongitude = table.Column<double>(type: "double precision", nullable: false),
                    CompletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AbandonTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastActivity = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CompletedKeyPoints = table.Column<string>(type: "jsonb", nullable: false),
                    ProgressPercentage = table.Column<double>(type: "double precision", nullable: false),
                    GroupSessionId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourExecutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourProblems",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResolvedByTouristComment = table.Column<string>(type: "text", nullable: true),
                    IsHighlighted = table.Column<bool>(type: "boolean", nullable: false),
                    AdminDeadline = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourProblems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourReviews",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ProgressPercentage = table.Column<double>(type: "double precision", nullable: false),
                    IsEdited = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourReviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    DistanceInKm = table.Column<double>(type: "double precision", nullable: false),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Tags = table.Column<string>(type: "jsonb", nullable: false),
                    TourDurations = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourWishlists",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourWishlists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupTourSessionParticipants",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SessionId = table.Column<long>(type: "bigint", nullable: false),
                    TouristId = table.Column<long>(type: "bigint", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LeftAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TourExecutionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupTourSessionParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupTourSessionParticipants_GroupTourSessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "tours",
                        principalTable: "GroupTourSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AuthorId = table.Column<long>(type: "bigint", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    AuthorType = table.Column<int>(type: "integer", nullable: false),
                    TourProblemId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_TourProblems_TourProblemId",
                        column: x => x.TourProblemId,
                        principalSchema: "tours",
                        principalTable: "TourProblems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewImages",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourReviewId = table.Column<long>(type: "bigint", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewImages_TourReviews_TourReviewId",
                        column: x => x.TourReviewId,
                        principalSchema: "tours",
                        principalTable: "TourReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeyPoints",
                schema: "tours",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TourId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Secret = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    EncounterId = table.Column<long>(type: "bigint", nullable: true),
                    IsEncounterMandatory = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyPoints_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "tours",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourEquipment",
                schema: "tours",
                columns: table => new
                {
                    EquipmentId = table.Column<long>(type: "bigint", nullable: false),
                    TourId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourEquipment", x => new { x.EquipmentId, x.TourId });
                    table.ForeignKey(
                        name: "FK_TourEquipment_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalSchema: "tours",
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TourEquipment_Tours_TourId",
                        column: x => x.TourId,
                        principalSchema: "tours",
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AwardEvents_Year",
                schema: "tours",
                table: "AwardEvents",
                column: "Year",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bundles_AuthorId",
                schema: "tours",
                table: "Bundles",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_AuthorId",
                schema: "tours",
                table: "Coupons",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                schema: "tours",
                table: "Coupons",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_TourId",
                schema: "tours",
                table: "Coupons",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Diaries_TouristId",
                schema: "tours",
                table: "Diaries",
                column: "TouristId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupTourSessionParticipants_SessionId",
                schema: "tours",
                table: "GroupTourSessionParticipants",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyPoints_TourId",
                schema: "tours",
                table: "KeyPoints",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TourProblemId",
                schema: "tours",
                table: "Messages",
                column: "TourProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RecipientId_IsRead",
                schema: "tours",
                table: "Notifications",
                columns: new[] { "RecipientId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewImages_TourReviewId",
                schema: "tours",
                table: "ReviewImages",
                column: "TourReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_AuthorId",
                schema: "tours",
                table: "Sales",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_TourEquipment_TourId",
                schema: "tours",
                table: "TourEquipment",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourExecutions_TouristId_TourId_Status",
                schema: "tours",
                table: "TourExecutions",
                columns: new[] { "TouristId", "TourId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TourReviews_TourId",
                schema: "tours",
                table: "TourReviews",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_TourReviews_TourId_TouristId",
                schema: "tours",
                table: "TourReviews",
                columns: new[] { "TourId", "TouristId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tours_AuthorId",
                schema: "tours",
                table: "Tours",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_TourWishlists_TouristId",
                schema: "tours",
                table: "TourWishlists",
                column: "TouristId");

            migrationBuilder.CreateIndex(
                name: "IX_TourWishlists_TouristId_TourId",
                schema: "tours",
                table: "TourWishlists",
                columns: new[] { "TouristId", "TourId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AwardEvents",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Bundles",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Coupons",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Diaries",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Facilities",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "GroupTourSessionParticipants",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "KeyPoints",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Messages",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Monuments",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Positions",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "ReviewImages",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Sales",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "TourEquipment",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "TourExecutions",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "TourWishlists",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "GroupTourSessions",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "TourProblems",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "TourReviews",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Equipment",
                schema: "tours");

            migrationBuilder.DropTable(
                name: "Tours",
                schema: "tours");
        }
    }
}
