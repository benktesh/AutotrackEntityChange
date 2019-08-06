using System;
using System.Collections.Generic;

namespace AutotrackEntityChange.DBContext
{
    /// <summary>
    /// This interface determines what will be automatically tracked.
    /// </summary>
    interface IAuditable
    {
        Guid Id { get; set; }

        DateTime? CreatedDate { get; set; }

        DateTime? ModifiedDate { get; set; }

        String LastModifiedBy { get; set; }

        bool IsInactive { get; set; }
    }

    public enum EntityStateChangeTypeEnum
    {
        Added,
        Deleted,
        Modified,
    }

    public enum ContactTypeEnum
    {
        Primary,
        Secondary,
        Emergency,
    }
    public class Customer : IAuditable
    {
        public Guid Id { get; set; }

        public String AccountNumber { get; set; }

        public String Name { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public String LastModifiedBy { get; set; }

        public bool IsInactive { get; set; }

        public ICollection<CustomerContact> CustomerContacts { get; set; }
    }

    public class Contact : IAuditable
    {

        public Guid Id { get; set; }

        public String Name { get; set; }

        public String Title { get; set; }

        public String Phone { get; set; }

        public String Email { get; set; }

        public ContactTypeEnum ContactType { get; set; }

        public String Note { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public String LastModifiedBy { get; set; }

        public bool IsInactive { get; set; }
        public ICollection<CustomerContact> CustomerContacts { get; set; }

    }

    public class CustomerContact:IAuditable
    {
        public Guid Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public bool IsInactive { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public Guid ContactId { get; set; }
        public Contact Contact { get; set; }
    }

    public class Audit
    {

        public Guid Id { get; set; }

        public Guid? EntityId { get; set; }

        public string User { get; set; }

        public String Entity { get; set; }

        public DateTime DateTime { get; set; }

        public string ColumnName { get; set; }

        public String OldValue { get; set; }

        public String NewValue { get; set; }

        public EntityStateChangeTypeEnum ChangeType { get; set; }

    }
}
