using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TP3_API.Data;
using TP3_API.DTOs.Auth;
using TP3_API.Models;

namespace TP3_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { mensaje = "El correo electrónico ya está registrado." });

            // Buscamos por defecto el rol "Cajero" o asignamos el ID 2 (asumiendo que 1 es Admin y 2 es Cajero)
            // O podemos buscar un rol por defecto en la base de datos:
            var rolCajero = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Cajero");
            int rolIdAsignar = rolCajero != null ? rolCajero.Id : 2; // Fallback al ID 2

            var usuario = new Usuario
            {
                NombreUsuario = dto.NombreUsuario,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RolId = rolIdAsignar
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario registrado correctamente." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // Incluimos el Rol para poder usarlo en los Claims del Token
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
                return Unauthorized(new { mensaje = "Credenciales inválidas." });

            var token = GenerarJwtToken(usuario);
            return Ok(new { token });
        }

        private string GenerarJwtToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            string secretKey = jwtSettings["Key"] ?? "ClaveSecretaSuperSeguraTP3_2026!ConSuficienteLongitud12345";
            var key = Encoding.UTF8.GetBytes(secretKey);

            string nombreRol = usuario.Rol?.Nombre ?? "Cajero";

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.Role, nombreRol)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = jwtSettings["Issuer"] ?? "TP3Api",
                Audience = jwtSettings["Audience"] ?? "TP3Clients",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}