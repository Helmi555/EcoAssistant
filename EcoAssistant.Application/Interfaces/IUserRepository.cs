using EcoAssistant.Domain.Entities;

namespace EcoAssistant.Application.Interfaces;


public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<User> AddAsync(User user, CancellationToken ct = default);
    Task<List<User>> ListAsync(int limit = 50, int offset = 0, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}