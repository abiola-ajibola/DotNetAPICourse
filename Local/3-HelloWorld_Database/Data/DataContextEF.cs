// Dominic installed microsoft.entityframeworkcore and microsoft.entityframeworkcore.sqlserver separately
// but at https://learn.microsoft.com/en-us/ef/core/providers/sql-server/?tabs=dotnet-core-cli it is claimed
// that simply installing Microsoft.EntityFrameworkCore.SqlServer adds both.
// see https://learn.microsoft.com/en-us/ef/core/providers/sql-server/?tabs=dotnet-core-cli#install

// I only installed Microsoft.EntityFrameworkCore.SqlServer and I have access to Microsoft.EntityFrameworkCore also.

// See https://learn.microsoft.com/en-us/ef/core/ to learn more about entity framework


using HelloWorld.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HelloWorld.Data
{
    public class DataContextEF : DbContext
    {
        // DbSet - Defines the tables you would be working with
        public DbSet<Computer> Computers { get; set; }
        private string? _connectionString;

        public DataContextEF(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("default");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString, opt => opt.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<Computer>(); // No need to add this, because the entity Computer is already added in DbSet
            // see: https://learn.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=data-annotations#including-types-in-the-model

            // To see why we need this, see: https://learn.microsoft.com/en-us/ef/core/modeling/entity-types?tabs=fluent-api#table-schema
            // TLDR; It tells the DB to use the Computers table in TutorialAppSchema schema for the Computer entity.
            // If we don't do this, sql server will use the default schema 'dbo'
            modelBuilder.Entity<Computer>().ToTable("Computers", "TutorialAppSchema");
        }

    }
}