// DataAccessLayer/Concrete/ApplicationDbContext.cs

using EntityLayer;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Concrete.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<SubCategory> SubCategories { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<FeaturedCategory> FeaturedCategories { get; set; }

    public DbSet<Price> Prices { get; set; }
    
    public DbSet<FAQ> FAQ { get; set; }
    
    public DbSet<Brand> Brands { get; set; }

    public DbSet<FooterItem> FooterItems { get; set; }

    public DbSet<ContactForm> ContactForms { get; set; }
    
    public DbSet<Feature> Features { get; set; }
    
    public DbSet<Address> Addresses { get; set; }

    public DbSet<Contract> Contracts { get; set; }
    
    public DbSet<UserContract> UserContracts { get; set; }

    public DbSet<Channel> Channels { get; set; }

    public DbSet<Warehouse> Warehouses { get; set; }
    
    public override int SaveChanges()
    {
        UpdateAuditInformation();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditInformation();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Veritabanı değişikliklerini izler ve oluşturma ya da değiştirme işlemi yapılan varlıkların denetim bilgilerini günceller.
    /// </summary>
    private void UpdateAuditInformation()
    {
        var entries = ChangeTracker.Entries().Where(e =>
            e.Entity is AuditTrailer && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var baseEntity = (AuditTrailer)entry.Entity;
            if (entry.State == EntityState.Added)
            {
                baseEntity.CreatedDateTime = DateTime.UtcNow;
                baseEntity.CreatedBy = "System";
            }
            else if (entry.State == EntityState.Modified)
            {
                baseEntity.ModifiedDateTime = DateTime.UtcNow;
                baseEntity.ModifiedBy = "System";
            }
        }
    }
}