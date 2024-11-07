using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProyectoDocucenter.ModelsDB
{
    public partial class BFAContext : DbContext
    {
        public BFAContext()
        {
        }

        public BFAContext(DbContextOptions<BFAContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Transaccion> Transaccions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
