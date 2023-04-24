using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Models;

[Table("users", Schema = "auth")]
public class User
{
    [Column("id"), Required]
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    [Column("name"), Required]
    public string Name { get; set; }
    
    /// <summary>
    /// IP-адрес пользователя.
    /// TODO: Очень изменчивая штука так-то, если оставлять, стоит перенести наверное в таблицу RefreshToken
    /// </summary>
    [Column("ip_adress")]
    public string IpAdress { get; set; }
    
    [Column("email"), Required, EmailAddress]
    public string Email { get; set; }
    
    [Column("status"), Required]
    public Status Status { get; set; }

    [Column("password_hash"), Required]
    public byte[] PasswordHash { get; set; }

    [Column("password_salt"), Required]
    public byte[] PasswordSalt { get; set; }
}

public enum Status
{
    Active,
    Unactive,
    InGame
}