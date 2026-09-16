using System.ComponentModel.DataAnnotations;
namespace SocialBackend.Models;
public class User
{
public Guid Id { get; set; } = Guid.NewGuid();
[Required] public string Username { get; set; } = string.Empty;
[Required] public string Email { get; set; } = string.Empty;
[Required] public string PasswordHash { get; set; } = string.Empty;
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
public ICollection<Connection> SentConnections { get; set; } = new List<Connection>();
public ICollection<Connection> ReceivedConnections { get; set; } = new List<Connection>();
}
public class Connection
{
public Guid Id { get; set; } = Guid.NewGuid();
public Guid RequesterId { get; set; }
public User Requester { get; set; } = null!;
public Guid ReceiverId { get; set; }
public User Receiver { get; set; } = null!;
public bool IsAccepted { get; set; } = false;
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class DirectMessage
{
public Guid Id { get; set; } = Guid.NewGuid();
public Guid SenderId { get; set; }
public User Sender { get; set; } = null!;
public Guid ReceiverId { get; set; }
public User Receiver { get; set; } = null!;
public string Content { get; set; } = string.Empty;
public DateTime SentAt { get; set; } = DateTime.UtcNow;
}