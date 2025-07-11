﻿using HomeworkDapper.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkDapper
{
    public class AppDbContext : DbContext
    {
        public DbSet<Dog> Dogs { get; set; }
        public DbSet<Adopter> Adopters { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = EfCoreRelationsDateBase;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dog>().Property(d => d.IsAdopted).HasDefaultValue(false);

            modelBuilder.Entity<Dog>()
        .HasOne(d => d.Adopter)
        .WithMany(a => a.Dogs)
        .HasForeignKey(d => d.AdopterId)
        .OnDelete(DeleteBehavior.SetNull);

        }


        }
}
