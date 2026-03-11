using InventoryManagementApp.Model;
using InventoryManagementApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using System.Reflection.Emit;
namespace InventoryManagementApp.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<InventoryField> InventoryFields { get; set; }
        public DbSet<InventoryAccess> InventoryAccesses { get; set; }
        public DbSet<ItemFieldValue> ItemsFieldValues { get; set; }
        public DbSet<ItemLike> ItemLikes { get; set; }
        public DbSet<DiscussionPost> DiscussionPosts { get; set; }
        public DbSet<InventoryTag> InventoryTags { get; set; }
        public DbSet<ItemFieldValue> ItemFieldValues { get; set; }
        public DbSet<InventoryCustomIdPart> InventoryCustomIdParts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Inventory>()
                .Property(i => i.SearchVector)
                .HasColumnType("tsvector");

            builder.Entity<Inventory>()
                .HasIndex(i => i.SearchVector)
                .HasMethod("GIN");
            base.OnModelCreating(builder);
            builder.Entity<Inventory>()
                .HasMany(i => i.Tags)
                .WithMany(t => t.Inventories)
                .UsingEntity(j => j.ToTable("InventoryTagsMapping"));
        }
    }
}
