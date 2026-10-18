using Domain.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configuration.Abstract
{
    public abstract class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.ModifiedDate).IsRequired(false);
            builder.Property(x => x.DeletedDate).IsRequired(false);

            builder.Property(x => x.CreatedUsername).HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.ModifiedUsername).HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.DeletedUsername).HasMaxLength(100).IsRequired(false);

            builder.Property(x => x.IsDeleted).IsRequired();
            builder.Property(x => x.Status).IsRequired();

            builder.HasIndex(x => x.IsDeleted);
        }
    }
}