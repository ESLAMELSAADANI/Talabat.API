using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entities;
using Talabat.Infrastructure.Generic_Repository.Data.Config;

namespace Talabat.Infrastructure.Generic_Repository.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

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
