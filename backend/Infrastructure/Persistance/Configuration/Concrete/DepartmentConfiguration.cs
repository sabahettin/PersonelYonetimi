using Domain.Concrete;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistance.Configuration.Abstract;

namespace Persistance.Configuration.Concrete
{
    public class DepartmentConfiguration : BaseConfiguration<Department>
    {
        public override void Configure(EntityTypeBuilder<Department> builder)
        {
            base.Configure(builder);
            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
