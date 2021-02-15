using app.Entities;
using Microsoft.EntityFrameworkCore;

namespace app.Context
{
    public class CartContext : DbContext
    {
        public CartContext(DbContextOptions options) : base(options)
        {
        }
        public virtual DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {     
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(x => x.OrderId);
            });
        }
    }
}
