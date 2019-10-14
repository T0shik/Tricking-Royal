﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TrickingRoyal.Database;

namespace TrickingRoyal.Database.Migrations.App
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Battles.Domain.Models.SubComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CommentId");

                    b.Property<string>("DisplayName");

                    b.Property<string>("Message");

                    b.Property<string>("Picture");

                    b.Property<string>("TaggedUser");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.ToTable("SubComments");
                });

            modelBuilder.Entity("Battles.Models.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DisplayName");

                    b.Property<int>("MatchId");

                    b.Property<string>("Message");

                    b.Property<string>("Picture");

                    b.HasKey("Id");

                    b.HasIndex("MatchId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Battles.Models.Decision", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EvaluationId");

                    b.Property<string>("UserId");

                    b.Property<int>("Vote");

                    b.Property<int>("Weight");

                    b.HasKey("Id");

                    b.HasIndex("EvaluationId");

                    b.HasIndex("UserId");

                    b.ToTable("Decisions");
                });

            modelBuilder.Entity("Battles.Models.Evaluation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Complete");

                    b.Property<DateTime>("Created");

                    b.Property<int>("EvaluationType");

                    b.Property<DateTime>("Expiry");

                    b.Property<int>("MatchId");

                    b.Property<string>("Reason");

                    b.HasKey("Id");

                    b.HasIndex("MatchId");

                    b.ToTable("Evaluations");
                });

            modelBuilder.Entity("Battles.Models.Like", b =>
                {
                    b.Property<int>("MatchId");

                    b.Property<string>("UserId");

                    b.HasKey("MatchId", "UserId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("Battles.Models.Match", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Chain");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Finished");

                    b.Property<DateTime>("LastUpdate");

                    b.Property<int>("Mode");

                    b.Property<int>("Round");

                    b.Property<int>("Status");

                    b.Property<int>("Surface");

                    b.Property<string>("Turn");

                    b.Property<int>("TurnDays");

                    b.Property<int>("TurnType");

                    b.HasKey("Id");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("Battles.Models.MatchUser", b =>
                {
                    b.Property<int>("MatchId");

                    b.Property<string>("UserId");

                    b.Property<bool>("CanFlag");

                    b.Property<bool>("CanGo");

                    b.Property<bool>("CanLockIn");

                    b.Property<bool>("CanPass");

                    b.Property<bool>("CanUpdate");

                    b.Property<int>("Index");

                    b.Property<int>("Points");

                    b.Property<bool>("Ready");

                    b.Property<int>("Role");

                    b.Property<bool>("Winner");

                    b.HasKey("MatchId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("MatchUser");
                });

            modelBuilder.Entity("Battles.Models.NotificationConfiguration", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<int>("ConfigurationType");

                    b.Property<string>("NotificationId");

                    b.Property<string>("UserInformationId");

                    b.HasKey("Id");

                    b.HasIndex("UserInformationId");

                    b.ToTable("NotificationConfigurations");
                });

            modelBuilder.Entity("Battles.Models.NotificationMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Message");

                    b.Property<string>("Navigation");

                    b.Property<bool>("New");

                    b.Property<DateTime>("TimeStamp");

                    b.Property<int>("Type");

                    b.Property<string>("UserInformationId");

                    b.HasKey("Id");

                    b.HasIndex("UserInformationId");

                    b.ToTable("NotificationMessages");
                });

            modelBuilder.Entity("Battles.Models.UserInformation", b =>
                {
                    b.Property<string>("Id");

                    b.Property<bool>("Activated");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<string>("DisplayName");

                    b.Property<int>("Draw");

                    b.Property<int>("Experience");

                    b.Property<string>("Facebook");

                    b.Property<int>("Flags");

                    b.Property<string>("Gym");

                    b.Property<int>("Hosting");

                    b.Property<int>("HostingLimit");

                    b.Property<string>("Information");

                    b.Property<string>("Instagram");

                    b.Property<int>("Joined");

                    b.Property<int>("JoinedLimit");

                    b.Property<int>("Level");

                    b.Property<int>("LevelUpPoints");

                    b.Property<int>("Loss");

                    b.Property<string>("Picture");

                    b.Property<int>("Reputation");

                    b.Property<int>("Skill");

                    b.Property<int>("Style");

                    b.Property<int>("VotingPower");

                    b.Property<int>("Win");

                    b.Property<string>("Youtube");

                    b.HasKey("Id");

                    b.ToTable("UserInformation");
                });

            modelBuilder.Entity("Battles.Models.Video", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Empty");

                    b.Property<int>("MatchId");

                    b.Property<string>("ThumbPath");

                    b.Property<string>("UserId");

                    b.Property<int>("UserIndex");

                    b.Property<int>("VideoIndex");

                    b.Property<string>("VideoPath");

                    b.HasKey("Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("UserId");

                    b.ToTable("Videos");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("TrickingRoyal.Database.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Battles.Domain.Models.SubComment", b =>
                {
                    b.HasOne("Battles.Models.Comment", "Comment")
                        .WithMany("SubComments")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Battles.Models.Comment", b =>
                {
                    b.HasOne("Battles.Models.Match", "Match")
                        .WithMany("Comments")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Battles.Models.Decision", b =>
                {
                    b.HasOne("Battles.Models.Evaluation", "Evaluation")
                        .WithMany("Decisions")
                        .HasForeignKey("EvaluationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Battles.Models.UserInformation", "User")
                        .WithMany("Decisions")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Battles.Models.Evaluation", b =>
                {
                    b.HasOne("Battles.Models.Match", "Match")
                        .WithMany()
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Battles.Models.Like", b =>
                {
                    b.HasOne("Battles.Models.Match", "Match")
                        .WithMany("Likes")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Battles.Models.MatchUser", b =>
                {
                    b.HasOne("Battles.Models.Match", "Match")
                        .WithMany("MatchUsers")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Battles.Models.UserInformation", "User")
                        .WithMany("UserMatches")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Battles.Models.NotificationConfiguration", b =>
                {
                    b.HasOne("Battles.Models.UserInformation")
                        .WithMany("NotificationConfigurations")
                        .HasForeignKey("UserInformationId");
                });

            modelBuilder.Entity("Battles.Models.NotificationMessage", b =>
                {
                    b.HasOne("Battles.Models.UserInformation")
                        .WithMany("Notifications")
                        .HasForeignKey("UserInformationId");
                });

            modelBuilder.Entity("Battles.Models.Video", b =>
                {
                    b.HasOne("Battles.Models.Match", "Match")
                        .WithMany("Videos")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Battles.Models.UserInformation", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("TrickingRoyal.Database.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("TrickingRoyal.Database.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TrickingRoyal.Database.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("TrickingRoyal.Database.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
