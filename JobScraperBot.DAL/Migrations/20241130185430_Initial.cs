using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JobScraperBot.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HiddenVacancies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HiddenVacancies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobKinds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KindName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobKinds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageIntervals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Interval = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageIntervals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StackName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    MessageIntervalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time", nullable: false),
                    NextUpdate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_MessageIntervals_MessageIntervalId",
                        column: x => x.MessageIntervalId,
                        principalTable: "MessageIntervals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobKindId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionSettings_Grades_GradeId",
                        column: x => x.GradeId,
                        principalTable: "Grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionSettings_JobKinds_JobKindId",
                        column: x => x.JobKindId,
                        principalTable: "JobKinds",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubscriptionSettings_Stacks_StackId",
                        column: x => x.StackId,
                        principalTable: "Stacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionSettings_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Grades",
                columns: new[] { "Id", "GradeName" },
                values: new object[,]
                {
                    { new Guid("2ae095ee-0a25-448b-aa4c-3f36d6bfd690"), "junior" },
                    { new Guid("4732ea9c-5213-44d7-bf6c-1d3b3ebb0541"), "middle" },
                    { new Guid("5ca50086-db24-42d3-8f1c-fd99dd08f9b0"), "team_lead" },
                    { new Guid("64dafd11-1362-4f4e-b30c-676f85da3d9f"), "senior" },
                    { new Guid("65a8789f-75af-4844-b988-54a719affc99"), "head_chief" },
                    { new Guid("b029e897-181d-4d1a-8d88-dae9c6e32842"), "trainee_intern" }
                });

            migrationBuilder.InsertData(
                table: "JobKinds",
                columns: new[] { "Id", "KindName" },
                values: new object[,]
                {
                    { new Guid("aceceff4-c31c-4bcb-9477-e34c20904059"), "remote" },
                    { new Guid("b6c47d47-8d77-46fc-9451-32d3eb1003d3"), "office_or_remote" },
                    { new Guid("c5b3ab08-2e77-49a8-a6be-dce363847c22"), "office" }
                });

            migrationBuilder.InsertData(
                table: "MessageIntervals",
                columns: new[] { "Id", "Interval" },
                values: new object[,]
                {
                    { new Guid("6a502243-794c-4db9-918a-71aae13713e8"), "daily" },
                    { new Guid("75bb9554-4bd8-4bc5-8e36-8a8bf6c93dae"), "once_in_two_days" },
                    { new Guid("7b467d34-83d2-4e6c-8748-e884ad0c76c1"), "weekly" }
                });

            migrationBuilder.InsertData(
                table: "Stacks",
                columns: new[] { "Id", "StackName" },
                values: new object[,]
                {
                    { new Guid("2b085c17-ec37-4214-91c7-f6806ff40864"), ".net" },
                    { new Guid("2e5f45c5-0758-4396-9744-6d93bdbf7466"), "full_stack" },
                    { new Guid("65afd9dc-73cc-476e-b460-a1547313323c"), "java" },
                    { new Guid("9d439620-e7e4-4e2d-b868-b90e0fe66d58"), "python" },
                    { new Guid("ebcb84e1-e67c-4d9e-bdea-7ec59e9c8f2e"), "front_end" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_MessageIntervalId",
                table: "Subscriptions",
                column: "MessageIntervalId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionSettings_GradeId",
                table: "SubscriptionSettings",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionSettings_JobKindId",
                table: "SubscriptionSettings",
                column: "JobKindId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionSettings_StackId",
                table: "SubscriptionSettings",
                column: "StackId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionSettings_SubscriptionId",
                table: "SubscriptionSettings",
                column: "SubscriptionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HiddenVacancies");

            migrationBuilder.DropTable(
                name: "SubscriptionSettings");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "JobKinds");

            migrationBuilder.DropTable(
                name: "Stacks");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "MessageIntervals");
        }
    }
}
