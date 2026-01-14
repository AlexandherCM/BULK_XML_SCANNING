using BULK_DOWNLOAD_CFDI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BULK_DOWNLOAD_CFDI.Models
{
    public class Context : DbContext
    {   
        public Context(DbContextOptions<Context> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }   
}
