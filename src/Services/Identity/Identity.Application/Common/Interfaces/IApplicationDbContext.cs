using Microsoft.EntityFrameworkCore;
using sattec.Identity.Domain.Entities;

namespace sattec.Identity.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<BankAccount> BankAccounts { get; }
    DbSet<Organizations> Organization { get; }
    DbSet<Documentation> Documentation { get; }
    DbSet<Brand> Brand { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
