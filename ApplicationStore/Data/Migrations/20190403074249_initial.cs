using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationStore.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    State = table.Column<byte>(nullable: false),
                    ApplicationStoreUserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationCategories_AspNetUsers_ApplicationStoreUserId",
                        column: x => x.ApplicationStoreUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            
            migrationBuilder.CreateTable(
                name: "Platforms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    ApplicationStoreUserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Platforms_AspNetUsers_ApplicationStoreUserId",
                        column: x => x.ApplicationStoreUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    RegisterDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    State = table.Column<byte>(nullable: false),
                    ApplicationCategoryId = table.Column<int>(nullable: false),
                    ApplicationStoreUserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_ApplicationCategories_ApplicationCategoryId",
                        column: x => x.ApplicationCategoryId,
                        principalTable: "ApplicationCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Applications_AspNetUsers_ApplicationStoreUserId",
                        column: x => x.ApplicationStoreUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationPublishs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Version = table.Column<string>(maxLength: 10, nullable: false),
                    PublishDate = table.Column<DateTime>(nullable: false),
                    RegisterDate = table.Column<DateTime>(nullable: false),
                    ChangeList = table.Column<string>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    Size = table.Column<double>(nullable: false),
                    Extension = table.Column<string>(maxLength: 10, nullable: true),
                    PlatformId = table.Column<int>(nullable: false),
                    ApplicationId = table.Column<int>(nullable: false),
                    ApplicationStoreUserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationPublishs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationPublishs_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationPublishs_AspNetUsers_ApplicationStoreUserId",
                        column: x => x.ApplicationStoreUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationPublishs_Platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "Platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationPictures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Size = table.Column<double>(nullable: false),
                    Data = table.Column<byte[]>(nullable: false),
                    RegisterDate = table.Column<DateTime>(nullable: false),
                    IsDefaultIcon = table.Column<bool>(nullable: false),
                    ApplicationPublishId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationPictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationPictures_ApplicationPublishs_ApplicationPublishId",
                        column: x => x.ApplicationPublishId,
                        principalTable: "ApplicationPublishs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 100, nullable: false),
                    RegisterDate = table.Column<DateTime>(nullable: false),
                    ApplicationPublishId = table.Column<int>(nullable: false),
                    ApplicationStoreUserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_ApplicationPublishs_ApplicationPublishId",
                        column: x => x.ApplicationPublishId,
                        principalTable: "ApplicationPublishs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_ApplicationStoreUserId",
                        column: x => x.ApplicationStoreUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DownloadApplications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RegisterDate = table.Column<DateTime>(nullable: false),
                    ApplicationPublishId = table.Column<int>(nullable: false),
                    ApplicationStoreUserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DownloadApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DownloadApplications_ApplicationPublishs_ApplicationPublishId",
                        column: x => x.ApplicationPublishId,
                        principalTable: "ApplicationPublishs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DownloadApplications_AspNetUsers_ApplicationStoreUserId",
                        column: x => x.ApplicationStoreUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FavorieApplications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RegisterDate = table.Column<DateTime>(nullable: false),
                    ApplicationPublishId = table.Column<int>(nullable: false),
                    ApplicationStoreUserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavorieApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavorieApplications_ApplicationPublishs_ApplicationPublishId",
                        column: x => x.ApplicationPublishId,
                        principalTable: "ApplicationPublishs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavorieApplications_AspNetUsers_ApplicationStoreUserId",
                        column: x => x.ApplicationStoreUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentLikes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsLike = table.Column<bool>(nullable: false),
                    Count = table.Column<double>(nullable: false),
                    CommentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentLikes_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationCategories_ApplicationStoreUserId",
                table: "ApplicationCategories",
                column: "ApplicationStoreUserId");

            migrationBuilder.CreateIndex(
                name: "ApplicationCategoryCodeIndex",
                table: "ApplicationCategories",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ApplicationCategoryTitleIndex",
                table: "ApplicationCategories",
                column: "Title",
                unique: true,
                filter: "[Title] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationPictures_ApplicationPublishId",
                table: "ApplicationPictures",
                column: "ApplicationPublishId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationPublishs_ApplicationId",
                table: "ApplicationPublishs",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationPublishs_ApplicationStoreUserId",
                table: "ApplicationPublishs",
                column: "ApplicationStoreUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationPublishs_PlatformId",
                table: "ApplicationPublishs",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicationCategoryId",
                table: "Applications",
                column: "ApplicationCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ApplicationStoreUserId",
                table: "Applications",
                column: "ApplicationStoreUserId");

            migrationBuilder.CreateIndex(
                name: "ApplicationCodeIndex",
                table: "Applications",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ApplicationTitleIndex",
                table: "Applications",
                column: "Title",
                unique: true,
                filter: "[Title] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CommentLikes_CommentId",
                table: "CommentLikes",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ApplicationPublishId",
                table: "Comments",
                column: "ApplicationPublishId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ApplicationStoreUserId",
                table: "Comments",
                column: "ApplicationStoreUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DownloadApplications_ApplicationPublishId",
                table: "DownloadApplications",
                column: "ApplicationPublishId");

            migrationBuilder.CreateIndex(
                name: "IX_DownloadApplications_ApplicationStoreUserId",
                table: "DownloadApplications",
                column: "ApplicationStoreUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FavorieApplications_ApplicationPublishId",
                table: "FavorieApplications",
                column: "ApplicationPublishId");

            migrationBuilder.CreateIndex(
                name: "IX_FavorieApplications_ApplicationStoreUserId",
                table: "FavorieApplications",
                column: "ApplicationStoreUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Platforms_ApplicationStoreUserId",
                table: "Platforms",
                column: "ApplicationStoreUserId");

            migrationBuilder.CreateIndex(
               name: "PlatformTitleIndex",
               table: "Platforms",
               column: "Title",
               unique: true,
               filter: "[Title] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationPictures");
            
            migrationBuilder.DropTable(
                name: "CommentLikes");

            migrationBuilder.DropTable(
                name: "DownloadApplications");

            migrationBuilder.DropTable(
                name: "FavorieApplications");
            
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "ApplicationPublishs");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Platforms");

            migrationBuilder.DropTable(
                name: "ApplicationCategories");
        }
    }
}
