using Microsoft.EntityFrameworkCore;

namespace GrpcServiceDemo.Server.Models
{
    public class GrpcUserServiceContext : DbContext
    {
        public GrpcUserServiceContext(DbContextOptions<GrpcUserServiceContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}