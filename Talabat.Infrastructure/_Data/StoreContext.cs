using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Infrastructure.Data.Config;

namespace Talabat.Infrastructure.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            ////Apply The Entity Configuration Using The Old Approach
            //modelBuilder.ApplyConfiguration(new ProductConfiguration);
            //modelBuilder.ApplyConfiguration(new ProductBrandConfiguration);
            //modelBuilder.ApplyConfiguration(new ProductCategoryConfiguration);


            //New Approach, in run time will generate the previous code (this is reflection => write less and in run time generate the code)
            //This method will apply all configuration from all IEntityTypeConfiguration<TEntity> instances that are defined in provided assembly.
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
    }
}
