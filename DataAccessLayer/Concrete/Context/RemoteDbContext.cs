using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Concrete.Context;

public class RemoteDbContext : ApplicationDbContext
{
    public RemoteDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}