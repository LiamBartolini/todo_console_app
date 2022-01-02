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
        public virtual DbSet<User> Users { get; set; }

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

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.CreationDate).IsRequired();

                entity.Property(e => e.FkUserId).HasColumnName("FK_UserID");

                entity.Property(e => e.Title).IsRequired();

                entity.HasOne(d => d.FkUser)
                    .WithMany(p => p.Todos)
                    .HasForeignKey(d => d.FkUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Username).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
