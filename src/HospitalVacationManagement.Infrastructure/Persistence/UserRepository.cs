using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Domain.Users;
using HospitalVacationManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalVacationManagement.Infrastructure.Persistence;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
{
    return _dbContext.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
}

    public async Task<IReadOnlyCollection<User>> ListAsync(CancellationToken cancellationToken = default)
{
    return await _dbContext.Users
        .AsNoTracking()
        .OrderBy(user => user.FullName)
        .ToListAsync(cancellationToken);
}

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public void Update(User user)
    {
        _dbContext.Users.Update(user);
    }
}