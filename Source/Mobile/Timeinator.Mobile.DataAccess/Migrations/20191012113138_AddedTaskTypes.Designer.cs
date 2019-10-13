﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using Timeinator.Core;
using Timeinator.Mobile.DataAccess;

namespace Timeinator.Mobile.DataAccess.Migrations
{
    [DbContext(typeof(TimeinatorMobileDbContext))]
    [Migration("20191012113138_AddedTaskTypes")]
    partial class AddedTaskTypes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026");

            modelBuilder.Entity("Timeinator.Mobile.DataAccess.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Type");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Timeinator.Mobile.DataAccess.TimeTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<TimeSpan>("AssignedTime");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description");

                    b.Property<bool>("IsImmortal");

                    b.Property<bool>("IsImportant");

                    b.Property<double>("MaxProgress");

                    b.Property<string>("Name");

                    b.Property<int>("Priority");

                    b.Property<double>("Progress");

                    b.Property<string>("TagsString");

                    b.Property<DateTime>("TargetStartDate");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("TimeTasks");
                });
#pragma warning restore 612, 618
        }
    }
}
