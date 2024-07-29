// DataAccessLayer/Concrete/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using EntityLayer.Entities;
using EntityLayer;

namespace DataAccessLayer.Concrete;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<SubCategory> SubCategories { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }

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
        var entries = ChangeTracker.Entries().Where(e => e.Entity is AuditTrailer && (e.State == EntityState.Added || e.State == EntityState.Modified));

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