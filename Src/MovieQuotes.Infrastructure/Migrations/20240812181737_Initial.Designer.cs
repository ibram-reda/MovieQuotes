﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MovieQuotes.Infrastructure;

#nullable disable

namespace MovieQuotes.Infrastructure.Migrations
{
    [DbContext(typeof(MovieQuotesDbContext))]
    [Migration("20240812181737_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MovieQuotes.Domain.Models.Movie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasMaxLength(700)
                        .HasColumnType("nvarchar(700)");

                    b.Property<string>("LocalPath")
                        .IsRequired()
                        .HasMaxLength(700)
                        .HasColumnType("nvarchar(700)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.HasKey("Id");

                    b.HasAlternateKey("Title");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("MovieQuotes.Domain.Models.SubtitlePhrase", b =>
                {
                    b.Property<int>("MovieId")
                        .HasColumnType("int");

                    b.Property<int>("Sequence")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("EndTime")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("time");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(700)
                        .HasColumnType("nvarchar(700)");

                    b.Property<string>("VideoClipPath")
                        .HasMaxLength(700)
                        .HasColumnType("nvarchar(700)");

                    b.HasKey("MovieId", "Sequence");

                    b.HasIndex("Text");

                    b.ToTable("SubtitlePhrases");
                });

            modelBuilder.Entity("MovieQuotes.Domain.Models.SubtitlePhrase", b =>
                {
                    b.HasOne("MovieQuotes.Domain.Models.Movie", "Movie")
                        .WithMany("Subtitles")
                        .HasForeignKey("MovieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Movie");
                });

            modelBuilder.Entity("MovieQuotes.Domain.Models.Movie", b =>
                {
                    b.Navigation("Subtitles");
                });
#pragma warning restore 612, 618
        }
    }
}
