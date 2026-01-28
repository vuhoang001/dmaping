using InvoiceHub.Models;
using Microsoft.EntityFrameworkCore;

namespace InvoiceHub.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);
   }

   public DbSet<InvoiceInformation> InvoiceInfor { get; set; }
}