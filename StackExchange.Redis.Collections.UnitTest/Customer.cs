using System;

namespace Tests
{
    public class Customer
    {
        public Customer()
        {
            ID = Guid.NewGuid();
        }
        public Guid ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (!(obj is Customer))
                return false;

            Customer other = obj as Customer;

            return this.ID.Equals(other.ID);
        }
    }
}