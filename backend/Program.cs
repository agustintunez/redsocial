using Microsoft.EntityFrameworkCore;
using SocialBackend.Data;
using SocialBackend.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SocialDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options =>
{
options.AddDefaultPolicy(policy =>
policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
var app = builder.Build();
app.UseCors();
// Auto-creación de tablas al iniciar
using (var scope = app.Services.CreateScope())
{
var db = scope.ServiceProvider.GetRequiredService<SocialDbContext>();
db.Database.EnsureCreated();
}
// Endpoints
app.MapPost("/api/users/register", async (User user, SocialDbContext db) =>
{
if (await db.Users.AnyAsync(u => u.Email == user.Email))
return Results.BadRequest("El email ya existe.");
db.Users.Add(user);
await db.SaveChangesAsync();
return Results.Ok(user);
});
app.MapGet("/api/users", async (SocialDbContext db) =>
await db.Users.Select(u => new { u.Id, u.Username, u.Email }).ToListAsync());
// Solicitudes de Conexión
app.MapPost("/api/connections/request", async (Guid requesterId, Guid receiverId,
SocialDbContext db) =>
{
if (requesterId == receiverId) return Results.BadRequest("No puedes conectarte contigo
mismo.");
var exists = await db.Connections.AnyAsync(c =>
(c.RequesterId == requesterId && c.ReceiverId == receiverId) ||
(c.RequesterId == receiverId && c.ReceiverId == requesterId));
if (exists) return Results.BadRequest("La solicitud o conexión ya existe.");
var connection = new Connection { RequesterId = requesterId, ReceiverId = receiverId };
db.Connections.Add(connection);
await db.SaveChangesAsync();
return Results.Ok(connection);
});
app.MapPost("/api/connections/accept/{id:guid}", async (Guid id, SocialDbContext db) =>
{
var conn = await db.Connections.FindAsync(id);
if (conn == null) return Results.NotFound();
conn.IsAccepted = true;
await db.SaveChangesAsync();
return Results.Ok(conn);
});
// Enviar Mensaje Privado (Sólo entre conectados)
app.MapPost("/api/messages", async (DirectMessage msg, SocialDbContext db) =>
{
var isConnected = await db.Connections.AnyAsync(c => c.IsAccepted &&
((c.RequesterId == msg.SenderId && c.ReceiverId == msg.ReceiverId) ||
(c.RequesterId == msg.ReceiverId && c.ReceiverId == msg.SenderId)));
if (!isConnected)
return Results.BadRequest("Sólo puedes enviar mensajes a usuarios con los que estés
conectado.");
db.Messages.Add(msg);
await db.SaveChangesAsync();
return Results.Ok(msg);
});
// Obtener Chat entre dos usuarios
app.MapGet("/api/messages/{user1:guid}/{user2:guid}", async (Guid user1, Guid user2,
SocialDbContext db) =>
{
var messages = await db.Messages
.Where(m => (m.SenderId == user1 && m.ReceiverId == user2) ||
(m.SenderId == user2 && m.ReceiverId == user1))
.OrderBy(m => m.SentAt)
.ToListAsync();
return Results.Ok(messages);
});
app.Run();