using Microsoft.EntityFrameworkCore;

namespace AutotrackEntityChange.DBContext
{
    public class ApplicationDbContext: DbContext
    {
        //Define DbSets
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        public DbSet<CustomerContact> CustomerContacts { get; set; }
        public DbSet<Audit> Audits { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
    }

}
