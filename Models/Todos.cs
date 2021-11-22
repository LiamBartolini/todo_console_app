using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace todo_console_app.Models
{
    public partial class Todos : DbContext
    {
        public Todos()
        {
        }

        public Todos(DbContextOptions<Todos> options)
            : base(options)
        {
        }

        public virtual DbSet<Todo> Db { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=Todos.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Todo>(entity => 
            {
                entity.ToTable("Todo");

                entity.Property(e => e.ID)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Checked).HasDefaultValueSql("0");

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.CreationDate).IsRequired();

                entity.Property(e => e.Title).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
