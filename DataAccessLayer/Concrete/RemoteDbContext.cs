using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Concrete;

public class RemoteDbContext : ApplicationDbContext
{
    public RemoteDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}