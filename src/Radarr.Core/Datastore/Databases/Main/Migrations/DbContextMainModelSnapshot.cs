﻿// <auto-generated />

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Radarr.Core.Datastore.Databases.Main.Migrations
{
    [DbContext(typeof(DbContextMain))]
    partial class DbContextMainModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("Radarr.Core.Datastore.Databases.Main.Models.CommandModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body")
                        .IsRequired();

                    b.Property<TimeSpan?>("Duration");

                    b.Property<DateTime?>("EndedAt");

                    b.Property<string>("Exception");

                    b.Property<string>("Message");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("Priority");

                    b.Property<DateTime>("QueuedAt");

                    b.Property<DateTime?>("StartedAt");

                    b.Property<int>("Status");

                    b.Property<int>("Trigger");

                    b.HasKey("Id");

                    b.ToTable("Commands");
                });

            modelBuilder.Entity("Radarr.Core.Datastore.Databases.Main.Models.ScheduledTaskModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Interval");

                    b.Property<DateTime>("LastExecution");

                    b.Property<string>("TypeName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("ScheduledTasks");
                });
#pragma warning restore 612, 618
        }
    }
}
