using Application.Exceptions;
using Domain.Abstract;
using Domain.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using System.Reflection;

namespace Persistance.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        #region DBSET
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        #endregion

        private string CurrentUserName => "Admin";

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                    continue;

                var param = Expression.Parameter(entityType.ClrType, "e");
                var body = Expression.Equal(
                    Expression.Property(param, nameof(BaseEntity.IsDeleted)),
                    Expression.Constant(false));

                builder.Entity(entityType.ClrType)
                       .HasQueryFilter(Expression.Lambda(body, param));
            }
        }

        public override int SaveChanges()
        {
            CheckRestrictedSoftDeletesAsync(hardDelete: false, async: false, CancellationToken.None)
                .GetAwaiter().GetResult();
            StampAuditFields(hardDelete: false);
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => SaveChangesAsync(hardDelete: false, cancellationToken);

        public async Task<int> SaveChangesAsync(bool hardDelete, CancellationToken cancellationToken = default)
        {
            await CheckRestrictedSoftDeletesAsync(hardDelete, async: true, cancellationToken);
            StampAuditFields(hardDelete);
            return await base.SaveChangesAsync(cancellationToken);
        }

        private async Task CheckRestrictedSoftDeletesAsync(bool hardDelete, bool async, CancellationToken ct)
        {
            if (hardDelete)
                return;

            var deletedEntries = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in deletedEntries)
            {
                var restrictedFks = entry.Metadata.GetReferencingForeignKeys()
                    .Where(fk => fk.DeleteBehavior == DeleteBehavior.Restrict
                              && fk.PrincipalToDependent is not null);

                foreach (var fk in restrictedFks)
                {
                    var navigation = fk.PrincipalToDependent!;
                    if (!navigation.IsCollection)
                        continue;

                    var query = entry.Collection(navigation.Name).Query();
                    var dependentType = fk.DeclaringEntityType.ClrType;

                    var hasDependents = async
                        ? await AnyAsync(query, dependentType, ct)
                        : Any(query, dependentType);

                    if (hasDependents)
                        throw new DeleteRestrictedException(entry.Metadata.ClrType.Name, dependentType.Name);
                }
            }
        }

        private static readonly MethodInfo AnyAsyncMethod =
            typeof(ApplicationDbContext).GetMethod(nameof(AnyAsyncGeneric), BindingFlags.NonPublic | BindingFlags.Static)!;

        private static readonly MethodInfo AnyMethod =
            typeof(ApplicationDbContext).GetMethod(nameof(AnyGeneric), BindingFlags.NonPublic | BindingFlags.Static)!;

        private static Task<bool> AnyAsync(IQueryable query, Type type, CancellationToken ct)
            => (Task<bool>)AnyAsyncMethod.MakeGenericMethod(type).Invoke(null, new object[] { query, ct })!;

        private static bool Any(IQueryable query, Type type)
            => (bool)AnyMethod.MakeGenericMethod(type).Invoke(null, new object[] { query })!;

        private static Task<bool> AnyAsyncGeneric<T>(IQueryable query, CancellationToken ct)
            => ((IQueryable<T>)query).AnyAsync(ct);

        private static bool AnyGeneric<T>(IQueryable query)
            => ((IQueryable<T>)query).Any();

        private void StampAuditFields(bool hardDelete)
        {
            var date = DateTime.UtcNow;
            var userName = CurrentUserName;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = date;
                        entry.Entity.CreatedUsername = userName;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedDate = date;
                        entry.Entity.ModifiedUsername = userName;
                        entry.Property(x => x.CreatedDate).IsModified = false;
                        entry.Property(x => x.CreatedUsername).IsModified = false;
                        break;

                    case EntityState.Deleted when !hardDelete:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedDate = date;
                        entry.Entity.DeletedUsername = userName;
                        break;
                }
            }
        }
    }
}