using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRSCapstone.Models;

namespace PRSCapstone.Data {
    public class PRSCapstoneContext : DbContext {

        public virtual DbSet<PRSCapstone.Models.User> Users { get; set; }          
        public virtual DbSet<PRSCapstone.Models.Vendor> Vendors { get; set; }
        public virtual DbSet<PRSCapstone.Models.Product> Products { get; set; }
        public virtual DbSet<PRSCapstone.Models.Request> Requests { get; set; }
        public virtual DbSet<PRSCapstone.Models.RequestLine> RequestLines { get; set; }
        

        public PRSCapstoneContext (DbContextOptions<PRSCapstoneContext> options)
            : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            
            builder.Entity<User>(e => {
                e.HasIndex(u => u.Username).IsUnique();
            });
            
            builder.Entity<Vendor>(e => {
                e.HasIndex(v => v.Code).IsUnique();
            });

            builder.Entity<Product>(e => {
                e.HasIndex(p => p.PartNbr).IsUnique();
            });

        }

    }
}
