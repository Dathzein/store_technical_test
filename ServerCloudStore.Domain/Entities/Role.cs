namespace ServerCloudStore.Domain.Entities;

/// <summary>
/// Entidad que representa un rol del sistema
/// </summary>
public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty; // JSON string con permisos
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public List<User> Users { get; set; } = new();
}

