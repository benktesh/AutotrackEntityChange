using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AutotrackEntityChange.DBContext
{
    public class ApplicationDbContext : DbContext
    {
        //Define DbSets
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        public DbSet<CustomerContact> CustomerContacts { get; set; }
        public DbSet<Audit> Audits { get; set; }

        private string _contextUser;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            _contextUser = "Default";
        }

        private static readonly Dictionary<string, String> RelationshipMap = new Dictionary<string, string>
        {
            { "CustomerId", "Customer"},
            { "ContactId", "Contact"},
        };

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(cancellationToken);
            await OnAfterSaveChanges(auditEntries);
            return result;
        }

        public override int SaveChanges()
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = base.SaveChanges();
            OnAfterSaveChanges(auditEntries);
            return result;
        }

        private List<Audit> OnBeforeSaveChanges()
        {
            UpdateAutoTrackedProperties();
            ChangeTracker.DetectChanges();
            var auditEntries = new List<Audit>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (!(entry.Entity is IAuditable))
                    continue;

                //handle   CreatedDate  ModifiedDate LastModifiedBy
                if ((entry.State == EntityState.Modified || entry.State == EntityState.Added || entry.State == EntityState.Deleted) && !(entry.Entity is Audit))
                {
                    var tableName = entry.Entity.GetType().Name;
                    
                    EntityStateChangeTypeEnum changeType = EntityStateChangeTypeEnum.Added;
                    if (entry.State == EntityState.Deleted)
                    {
                        changeType = EntityStateChangeTypeEnum.Deleted;
                    }

                

                    //find the difference in original and current and use only

                    foreach (var property in entry.OriginalValues.Properties)
                    {

                        var auditEntity = new Audit { User = _contextUser, Entity = tableName, DateTime = DateTime.Now };

                        changeType = EntityStateChangeTypeEnum.Added;

                        auditEntity.EntityId = entry.CurrentValues != null ? entry.CurrentValues["Id"] as Guid? : Guid.NewGuid();

                        object orig = string.Empty;


                        if (entry.State == EntityState.Modified)
                        {

                            changeType = EntityStateChangeTypeEnum.Modified;
                            orig = entry.GetDatabaseValues().GetValue<object>(property.Name) ?? string.Empty;

                            IAuditable curEntity;

                            //if entity state is modified, then the createdDate does not need to be logged.

                            if (property.Name == nameof(curEntity.CreatedDate))
                            {
                                continue;
                            }
                        }

                        var current = entry.CurrentValues != null && entry.CurrentValues[property.Name] != null ? entry.CurrentValues[property.Name]?.ToString() : string.Empty;

                        if (!orig.ToString().Equals(current))
                        {
                            //This routine modifies the relationship valus to brign Name in Name(Id) format.
                            if (RelationshipMap.ContainsKey(property.Name) && !string.IsNullOrEmpty(current))
                            {
                                var entityId = new Guid(current);
                                var existing = "";
                                switch (property.Name)
                                {
                                    case "CustomerId":
                                        existing = Customers.Where(p => p.Id == entityId)
                                            .Select(k => k.Name).FirstOrDefault();
                                        break;

                                    case "ContactId":
                                        existing = Contacts.Where(p => p.Id == entityId)
                                            .Select(k => k.Name).FirstOrDefault();
                                        break;
                                }
                                current = WrapNames(existing, current);

                            }
                            auditEntity.OldValue = string.IsNullOrEmpty(orig.ToString()) ? string.Empty : orig.ToString();
                            auditEntity.NewValue = current;
                            auditEntity.ColumnName = property.Name;
                            auditEntity.ChangeType = changeType;
                            auditEntries.Add(auditEntity);
                        }
                    }
                }
            }
            return auditEntries;
        }





        private static string WrapNames(string existing, string current)
        {
            if (existing != null)
            {
                current = $"{existing} ({current})";
            }
            return current;
        }





        //TODO Handle audit table here

        private Task OnAfterSaveChanges(List<Audit> auditEntries)

        {

            if (auditEntries == null || auditEntries.Count == 0)

                return Task.CompletedTask;

            Audits.AddRange(auditEntries);

            return SaveChangesAsync();

        }



        private void UpdateAutoTrackedProperties()
        {
            // get entries that are being Added or Updated
            var modifiedEntries = ChangeTracker.Entries()

                .Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));

            var now = DateTime.Now;

            foreach (var entry in modifiedEntries)
            {
                var entity = entry.Entity as IAuditable;
                if (entity == null)
                    continue;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedDate = now;
                }

                else if (entry.State == EntityState.Modified || entry.State == EntityState.Unchanged)
                {
                    entry.Property(nameof(entity.CreatedDate)).IsModified = false;
                }
                //TODO entity.LastModifiedBy = _userService.GetUser();
                entity.ModifiedDate = now;
            }

        }
    }

}
