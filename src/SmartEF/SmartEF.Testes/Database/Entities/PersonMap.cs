using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SmartEF.Testes.Database.Entities
{
    public sealed class PersonMap : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Person", "Local");
            builder.HasKey(c => c.Id);
            
            builder
                .Property(p => p.Name)
                .HasColumnType("varchar(255)")
                .IsRequired();

            builder
                .Property(p => p.Birth)
                .HasColumnType("datetime")
                .IsRequired();

            builder
                .Property(p => p.Gender)
                .HasColumnType("varchar(15)")
                .HasConversion(new EnumToStringConverter<EPersonGender>())
                .IsRequired();

            builder
                .HasMany(p => p.Contacts)
                .WithOne()
                .HasForeignKey("PersonId")
                .OnDelete(DeleteBehavior.Cascade)
                .Metadata
                .PrincipalToDependent
                .SetField("_contacts");

            builder
                .Property(p => p.InsertDate)
                .IsRequired();

            builder
                .Property(p => p.IsDeleted)
                .IsRequired();

            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }

    public sealed class ContactMap : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.ToTable("Contact", "Local");
            builder.HasKey(c => c.Id);

            builder
                .Property(p => p.Value)
                .HasColumnType("varchar(255)")
                .IsRequired();

            builder
                .Property(p => p.Type)
                .HasColumnType("varchar(15)")
                .HasConversion(new EnumToStringConverter<EContactType>())
                .IsRequired();

            builder
                .Property(p => p.InsertDate)
                .IsRequired();

            builder
                .Property(p => p.IsDeleted)
                .IsRequired();

            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
