﻿// <auto-generated />
using System;
using Huby.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Huby.Migrations
{
    [DbContext(typeof(ApplicationDatabase))]
    [Migration("20201129150623_AddReaction")]
    partial class AddReaction
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Huby.Data.Hub", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(12)")
                        .HasMaxLength(12);

                    b.Property<int>("BannerColor")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsNSFW")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("character varying(12)");

                    b.Property<long>("SubscribersCount")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("OwnerId");

                    b.ToTable("Hubs");
                });

            modelBuilder.Entity("Huby.Data.Moderator", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(12)")
                        .HasMaxLength(12);

                    b.Property<bool>("CanDeletePosts")
                        .HasColumnType("boolean");

                    b.Property<bool>("CanEdit")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("HubId")
                        .IsRequired()
                        .HasColumnType("character varying(12)");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("character varying(12)");

                    b.HasKey("Id");

                    b.HasIndex("HubId");

                    b.HasIndex("UserId");

                    b.ToTable("Moderators");
                });

            modelBuilder.Entity("Huby.Data.Post", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(12)")
                        .HasMaxLength(12);

                    b.Property<long>("CommentsCount")
                        .HasColumnType("bigint");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ContentType")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("Dislikes")
                        .HasColumnType("bigint");

                    b.Property<string>("HubId")
                        .IsRequired()
                        .HasColumnType("character varying(12)");

                    b.Property<bool>("IsNSFW")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("Likes")
                        .HasColumnType("bigint");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("character varying(12)");

                    b.Property<string>("PostType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("HubId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("PostType");

                    b.ToTable("Posts");

                    b.HasDiscriminator<string>("PostType").HasValue("Post");
                });

            modelBuilder.Entity("Huby.Data.Reaction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(12)")
                        .HasMaxLength(12);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("PostId")
                        .IsRequired()
                        .HasColumnType("character varying(12)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("character varying(12)");

                    b.Property<int>("Value")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("Reaction");
                });

            modelBuilder.Entity("Huby.Data.Subscription", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(12)")
                        .HasMaxLength(12);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("HubId")
                        .IsRequired()
                        .HasColumnType("character varying(12)");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("character varying(12)");

                    b.HasKey("Id");

                    b.HasIndex("HubId");

                    b.HasIndex("UserId");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("Huby.Data.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("character varying(12)")
                        .HasMaxLength(12);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Huby.Data.Comment", b =>
                {
                    b.HasBaseType("Huby.Data.Post");

                    b.Property<string>("ParentId")
                        .IsRequired()
                        .HasColumnType("character varying(12)");

                    b.HasIndex("ParentId");

                    b.HasDiscriminator().HasValue("Comment");
                });

            modelBuilder.Entity("Huby.Data.Topic", b =>
                {
                    b.HasBaseType("Huby.Data.Post");

                    b.Property<bool>("Locked")
                        .HasColumnType("boolean");

                    b.Property<string>("Stickied")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Tags")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasDiscriminator().HasValue("Topic");
                });

            modelBuilder.Entity("Huby.Data.Hub", b =>
                {
                    b.HasOne("Huby.Data.User", "Owner")
                        .WithMany("Hubs")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Huby.Data.Moderator", b =>
                {
                    b.HasOne("Huby.Data.Hub", "Hub")
                        .WithMany("Moderators")
                        .HasForeignKey("HubId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Huby.Data.User", "User")
                        .WithMany("Moderations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Huby.Data.Post", b =>
                {
                    b.HasOne("Huby.Data.Hub", "Hub")
                        .WithMany("Topics")
                        .HasForeignKey("HubId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Huby.Data.User", "Owner")
                        .WithMany("Posts")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Huby.Data.Reaction", b =>
                {
                    b.HasOne("Huby.Data.Post", "Post")
                        .WithMany("Reactions")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Huby.Data.User", "User")
                        .WithMany("Reactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Huby.Data.Subscription", b =>
                {
                    b.HasOne("Huby.Data.Hub", "Hub")
                        .WithMany("Subscriptions")
                        .HasForeignKey("HubId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Huby.Data.User", "User")
                        .WithMany("Subscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Huby.Data.Comment", b =>
                {
                    b.HasOne("Huby.Data.Post", "Parent")
                        .WithMany("Comments")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
