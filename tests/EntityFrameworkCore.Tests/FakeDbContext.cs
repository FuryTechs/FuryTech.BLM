﻿using FuryTechs.BLM.NetStandard.Tests;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace FuryTechs.BLM.EntityFrameworkCore.Tests
{
    public class FakeDbContext : DbContext
    {
        public FakeDbContext(DbContextOptions options): base(options)
        {
        }
        
        /// <summary>
        /// In EFCore 1.x there is no automated inheritance, only if we're providing it
        /// </summary>
        /// <param name="mb"></param>
        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<LogicalDeleteEntity>().HasBaseType<MockEntity>();
            mb.Entity<InheritedLogicalDeleteEntity>().HasBaseType<MockEntity>();
            mb.Entity<MockEntity>().Property("Discriminator")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
        }

        public virtual DbSet<MockEntity> MockEntities { get; set; }
        public virtual DbSet<MockNestedEntity> MockNestedEntities { get; set; }
        public virtual DbSet<MockInterpretedEntity> MockInterpretedEntities { get; set; }
    }

}
