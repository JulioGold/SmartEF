using System;
using System.Collections.Generic;

namespace SmartEF.Testes.Database.Entities
{
    public sealed class Person : BaseEntity<int>, IVirtualDeletion
    {
        private readonly IList<Contact> _contacts;

        public string Name { get; set; }

        public DateTime Birth { get; set; }

        public EPersonGender Gender { get; set; }

        public IEnumerable<Contact> Contacts => _contacts;

        public DateTime InsertDate { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        protected Person()
        {
        }

        private Person(int id, string name, DateTime birth, EPersonGender gender, IList<Contact> contacts)
        {
            Id = id;
            Name = name;
            Birth = birth;
            Gender = gender;
            _contacts = contacts ?? [];
        }

        public static Person Create(string name, DateTime birth, EPersonGender gender, IList<Contact> contacts)
        {
            return new Person(0, name, birth, gender, contacts);
        }

        public Person UpdateName(string name)
        {
            Name = name;
            
            return this;
        }

        public void Delete()
        {
            IsDeleted = true;
        }
    }

    public sealed class Contact : BaseEntity<int>, IVirtualDeletion
    {
        public int PersonId { get; set; }

        public string Value { get; set; }

        public EContactType Type { get; set; }

        public DateTime InsertDate { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        protected Contact()
        {
        }

        private Contact(int id, string value, EContactType contactType)
        {
            Id = id;
            Value = value;
            Type = contactType;
        }

        public static Contact Create(string value, EContactType contactType)
        {
            return new Contact(0, value, contactType);
        }

        public void Delete()
        {
            IsDeleted = true;
        }
    }

    public enum EPersonGender
    {
        Female = 1,
        Male = 2,
        Other = 3
    }

    public enum EContactType
    {
        Phone = 1,
        Email = 2
    }
}
