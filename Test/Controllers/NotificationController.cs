using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Test.Models;

namespace Test.Controllers
{
    [Route("Test/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;

        public NotificationController(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

    // Token kaydetme endpoint'i
    [HttpPost("register-token")]
    public async Task<IActionResult> RegisterToken([FromBody] string token)
    {
        // Örnek: Kullanıcı ID'si JWT'den alınabilir
        var userId = "user-123"; 

        // Eski token'ları sil (opsiyonel)
        var existingTokens = _dbContext.FcmTokens.Where(t => t.UserId == userId);
        _dbContext.FcmTokens.RemoveRange(existingTokens);

        _dbContext.FcmTokens.Add(new FcmToken
        {
            UserId = userId,
            Token = token
        });

        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    // Tek bir token'a bildirim gönderme
    [HttpPost("send-to-token")]
    public async Task<IActionResult> SendToToken([FromBody] NotificationRequest request)
    {
        var message = new FirebaseAdmin.Messaging.Message()
        {
            Notification = new Notification
            {
                Title = request.Title,
                Body = request.Message
            },
            Token = request.Token
        };

        try
        {
            await FirebaseMessaging.DefaultInstance.SendAsync(message);
            return Ok();
        }
        catch (FirebaseException ex)
        {
            return BadRequest($"FCM Hatası: {ex.Message}");
        }
    }

    // Tüm kullanıcılara bildirim gönderme
    [HttpPost("broadcast")]
    public async Task<IActionResult> Broadcast([FromBody] NotificationRequest request)
    {
        var tokens = await _dbContext.FcmTokens.Select(t => t.Token).ToListAsync();

        var message = new FirebaseAdmin.Messaging.MulticastMessage()
        {
            Notification = new Notification
            {
                Title = request.Title,
                Body = request.Message
            },
            Tokens = tokens
        };

        var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
        return Ok(new { SuccessCount = response.SuccessCount });
    }
}

    public class NotificationRequest
    {
        public required string Title { get; set; }
        public required string Message { get; set; }
        public string? Token { get; set; } // Tekli gönderim için
    }

}
