using Domain.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistance.Configuration.Abstract;

namespace Persistance.Configuration.Concrete
{
    public class EmployeeConfiguration : BaseConfiguration<Employee>
    {
        public override void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(x => x.Name).IsRequired(true).HasMaxLength(30);
            builder.Property(x => x.Surname).IsRequired(true).HasMaxLength(30);
            builder.Property(x => x.Email).IsRequired(true).HasMaxLength(75);
            builder.HasOne(x => x.Department).WithMany(x => x.Employees).HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);

            base.Configure(builder);
        }
    }
}
