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

        public virtual DbSet<CtasFacturas> CtasFacturas { get; set; }
        public virtual DbSet<CtasConceptos> CtasConceptos { get; set; }
        public virtual DbSet<CtasPagosFactura> CtasPagosFactura { get; set; }
        public virtual DbSet<CtasDocsRelacionados> CtasDocsRelacionados { get; set; }

    }   
}
