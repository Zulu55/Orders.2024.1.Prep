using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Orders.Shared.Entities;

namespace Orders.Backend.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<State> States { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<Country>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<State>().HasIndex(s => new { s.CountryId, s.Name }).IsUnique();
            modelBuilder.Entity<Product>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<City>().HasIndex(c => new { c.StateId, c.Name }).IsUnique();
            DisableCascadingDelete(modelBuilder);
            //SetupForeingKeys(modelBuilder);
        }

        //private void SetupForeingKeys(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<ProductCategory>()
        //        .HasKey(pc => new { pc.ProductId, pc.CategoryId });

        //    modelBuilder.Entity<ProductCategory>()
        //        .HasOne(pc => pc.Product)
        //        .WithMany(p => p.ProductCategories)
        //        .HasForeignKey(pc => pc.ProductId)
        //        .IsRequired();

        //    modelBuilder.Entity<ProductCategory>()
        //        .HasOne(pc => pc.Category)
        //        .WithMany(c => c.ProductCategories)
        //        .HasForeignKey(pc => pc.CategoryId)
        //        .IsRequired();
        //}

        private void DisableCascadingDelete(ModelBuilder modelBuilder)
        {
            var relationships = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            foreach (var relationship in relationships)
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}