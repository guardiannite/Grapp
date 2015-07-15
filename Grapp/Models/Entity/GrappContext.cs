namespace Grapp.Models.Entity
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class GrappContext : DbContext
    {
        public GrappContext()
            : base("name=GrappContext")
        {
        }

        public virtual DbSet<HighscoreEntity> Highscores { get; set; }
        public virtual DbSet<PlayerEntity> Players { get; set; }
        public virtual DbSet<SkillEntity> Skills { get; set; }
        public virtual DbSet<SkillEnumEntity> SkillEnums { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HighscoreEntity>()
                .HasMany(e => e.Skills)
                .WithRequired(e => e.Highscore)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PlayerEntity>()
                .Property(e => e.Name)
                .IsFixedLength();

            modelBuilder.Entity<PlayerEntity>()
                .HasMany(e => e.Highscores)
                .WithRequired(e => e.Player)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SkillEnumEntity>()
                .HasMany(e => e.Skills)
                .WithRequired(e => e.SkillEnum)
                .WillCascadeOnDelete(false);
        }
    }
}
