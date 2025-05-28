using DailyStream.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // Necesario para ClaimTypes
// Asegúrate de tener ILogger aquí si no está ya globalmente:
// using Microsoft.Extensions.Logging;


namespace DailyStream.Controllers
{
    [Authorize]
    public class InteraccionesController : Controller
    {
        private readonly DailystreamContext _context;
        private readonly ILogger<InteraccionesController> _logger;

        public InteraccionesController(DailystreamContext context, ILogger<InteraccionesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLike([FromForm] string videoId)
        {
            if (string.IsNullOrEmpty(videoId))
            {
                return Json(new { success = false, message = "VideoId es requerido." });
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Json(new { success = false, message = "Usuario no autenticado o ID inválido." });
            }

            var likeExistente = await _context.Likes
                .FirstOrDefaultAsync(l => l.Idvideo == videoId && l.Idusuario == userId);

            if (likeExistente != null)
            {
                _context.Likes.Remove(likeExistente);
                await _context.SaveChangesAsync();
                return Json(new
                {
                    success = true,
                    action = "removed",
                    likes = await _context.Likes.CountAsync(l => l.Idvideo == videoId)
                });
            }
            else
            {
                _context.Likes.Add(new Like
                {
                    Idvideo = videoId,
                    Idusuario = userId
                });
                await _context.SaveChangesAsync();
                return Json(new
                {
                    success = true,
                    action = "added",
                    likes = await _context.Likes.CountAsync(l => l.Idvideo == videoId)
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment([FromForm] string videoId, [FromForm] string contenido)
        {
            if (string.IsNullOrEmpty(videoId) || string.IsNullOrEmpty(contenido))
            {
                return Json(new { success = false, message = "VideoId y contenido son requeridos." });
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Json(new { success = false, message = "Usuario no autenticado o ID inválido." });
            }

            var comentario = new Comentario
            {
                Idvideo = videoId,
                Idusuario = userId,
                Contenido = contenido,
                Fecha = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();

            var usuario = await _context.Usuarios.FindAsync(userId);

            return Json(new
            {
                success = true,
                comment = new
                {
                    user = usuario?.Username ?? "Usuario Desconocido",
                    content = contenido,
                    date = FormatDate(comentario.Fecha)
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(string videoId)
        {
            if (string.IsNullOrEmpty(videoId))
            {
                return Json(new { success = false, message = "VideoId es requerido." });
            }

            // Añade este header para forzar JSON
            Response.Headers.Add("Content-Type", "application/json");

            try
            {
                var comentarios = await _context.Comentarios
                    .Where(c => c.Idvideo == videoId)
                    .OrderByDescending(c => c.Fecha)
                    .Select(c => new {
                        user = _context.Usuarios
                            .Where(u => u.Idusuario == c.Idusuario)
                            .Select(u => u.Username),
                        content = c.Contenido,
                    })
                    .ToListAsync();

                return Json(comentarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener comentarios");
                return StatusCode(500, new { success = false, message = "Error al cargar comentarios" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLikeStatus(string videoId)
        {
            if (string.IsNullOrEmpty(videoId))
            {
                return Json(new { success = false, message = "VideoId es requerido." });
            }

            var likesCount = await _context.Likes.CountAsync(l => l.Idvideo == videoId);

            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { likes = likesCount, isLiked = false });
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Json(new { likes = likesCount, isLiked = false });
            }

            var isLiked = await _context.Likes
                .AnyAsync(l => l.Idvideo == videoId && l.Idusuario == userId);

            return Json(new { likes = likesCount, isLiked });
        }

        private string FormatDate(DateOnly date)
        {
            // Si 'date' pudiera ser default(DateOnly) y eso causara problemas, se podría añadir una guarda.
            // Sin embargo, viniendo de un campo no nullable de EF Core, es menos probable.
            // if (date == default(DateOnly)) return "Fecha inválida";

            var dateTime = date.ToDateTime(TimeOnly.MinValue);
            var diff = DateTime.UtcNow - dateTime;

            if (diff.TotalMinutes < 1) return "hace unos momentos";
            if (diff.TotalHours < 1) return $"hace {(int)diff.TotalMinutes} minutos";
            if (diff.TotalDays < 1) return $"hace {(int)diff.TotalHours} horas";
            if (diff.TotalDays < 30) return $"hace {(int)diff.TotalDays} días";

            var months = (int)(diff.TotalDays / 30);
            return months <= 1 ? "hace 1 mes" : $"hace {months} meses";
        }
    }
}