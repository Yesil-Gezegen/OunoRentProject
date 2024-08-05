using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Concrete;

public class LocalDbContext : ApplicationDbContext
{
    public LocalDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}