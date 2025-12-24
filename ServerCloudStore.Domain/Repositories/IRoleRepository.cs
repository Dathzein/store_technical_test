using ServerCloudStore.Domain.Entities;

namespace ServerCloudStore.Domain.Repositories;

/// <summary>
/// Interfaz para el repositorio de roles
/// </summary>
public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int id);
    Task<Role?> GetByNameAsync(string name);
    Task<IEnumerable<Role>> GetAllAsync();
    Task<int> CreateAsync(Role role);
    Task<bool> UpdateAsync(Role role);
    Task<bool> DeleteAsync(int id);
}

