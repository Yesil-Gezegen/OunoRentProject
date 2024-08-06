using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Concrete.Context;

public class LocalDbContext : ApplicationDbContext
{
    public LocalDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}