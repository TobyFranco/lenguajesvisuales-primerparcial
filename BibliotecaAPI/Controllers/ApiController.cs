using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Models;
using BibliotecaAPI.Services;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                    return Unauthorized(new { message = "Credenciales inválidas" });

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if (!result.Succeeded)
                    return Unauthorized(new { message = "Credenciales inválidas" });

                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtService.GenerateToken(user, roles);

                return Ok(new AuthResponseDto
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddHours(1),
                    UserId = user.Id,
                    Email = user.Email,
                    Nombre = $"{user.Nombre} {user.Apellido}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en login para el usuario {Email}", loginDto.Email);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                    return BadRequest(new { message = "El email ya está registrado" });

                var user = new ApplicationUser
                {
                    UserName = registerDto.UserName ?? registerDto.Email,
                    Email = registerDto.Email,
                    Nombre = registerDto.Nombre,
                    Apellido = registerDto.Apellido,
                    FechaRegistro = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                    return BadRequest(new { errors = result.Errors });

                await _userManager.AddToRoleAsync(user, "User");

                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtService.GenerateToken(user, roles);

                return Ok(new AuthResponseDto
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddHours(1),
                    UserId = user.Id,
                    Email = user.Email,
                    Nombre = $"{user.Nombre} {user.Apellido}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en registro para el usuario {Email}", registerDto.Email);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.Id,
                user.Email,
                user.Nombre,
                user.Apellido,
                user.FechaRegistro
            });
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AutoresController : ControllerBase
    {
        private readonly IAutorService _autorService;

        public AutoresController(IAutorService autorService)
        {
            _autorService = autorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AutorDto>>> GetAutores()
        {
            var autores = await _autorService.GetAllAsync();
            return Ok(autores);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AutorDto>> GetAutor(int id)
        {
            var autor = await _autorService.GetByIdAsync(id);
            if (autor == null)
                return NotFound();

            return Ok(autor);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AutorDto>> CreateAutor(CreateAutorDto createAutorDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var autor = await _autorService.CreateAsync(createAutorDto);
            return CreatedAtAction(nameof(GetAutor), new { id = autor.Id }, autor);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAutor(int id, CreateAutorDto updateAutorDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _autorService.UpdateAsync(id, updateAutorDto);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAutor(int id)
        {
            var result = await _autorService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
        {
            var categorias = await _categoriaService.GetAllAsync();
            return Ok(categorias);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
        {
            var categoria = await _categoriaService.GetByIdAsync(id);
            if (categoria == null)
                return NotFound();

            return Ok(categoria);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CategoriaDto>> CreateCategoria(CreateCategoriaDto createCategoriaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoria = await _categoriaService.CreateAsync(createCategoriaDto);
            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, categoria);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategoria(int id, CreateCategoriaDto updateCategoriaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoriaService.UpdateAsync(id, updateCategoriaDto);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var result = await _categoriaService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class LibrosController : ControllerBase
    {
        private readonly ILibroService _libroService;

        public LibrosController(ILibroService libroService)
        {
            _libroService = libroService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LibroDto>>> GetLibros(
            [FromQuery] string? titulo = null,
            [FromQuery] int? categoriaId = null,
            [FromQuery] bool? disponible = null)
        {
            var libros = await _libroService.GetAllAsync(titulo, categoriaId, disponible);
            return Ok(libros);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LibroDto>> GetLibro(int id)
        {
            var libro = await _libroService.GetByIdAsync(id);
            if (libro == null)
                return NotFound();

            return Ok(libro);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<LibroDto>> CreateLibro(CreateLibroDto createLibroDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var libro = await _libroService.CreateAsync(createLibroDto);
                return CreatedAtAction(nameof(GetLibro), new { id = libro.Id }, libro);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateLibro(int id, CreateLibroDto updateLibroDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _libroService.UpdateAsync(id, updateLibroDto);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteLibro(int id)
        {
            var result = await _libroService.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PrestamosController : ControllerBase
    {
        private readonly IPrestamoService _prestamoService;

        public PrestamosController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrestamoDto>>> GetPrestamos(
            [FromQuery] string? usuarioId = null,
            [FromQuery] int? estado = null)
        {
            var prestamos = await _prestamoService.GetAllAsync(usuarioId, estado);
            return Ok(prestamos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PrestamoDto>> GetPrestamo(int id)
        {
            var prestamo = await _prestamoService.GetByIdAsync(id);
            if (prestamo == null)
                return NotFound();

            return Ok(prestamo);
        }

        [HttpGet("mis-prestamos")]
        public async Task<ActionResult<IEnumerable<PrestamoDto>>> GetMisPrestamos()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var prestamos = await _prestamoService.GetAllAsync(userId, null);
            return Ok(prestamos);
        }

        [HttpPost]
        public async Task<ActionResult<PrestamoDto>> CreatePrestamo(CreatePrestamoDto createPrestamoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var prestamo = await _prestamoService.CreateAsync(createPrestamoDto, userId);
                return CreatedAtAction(nameof(GetPrestamo), new { id = prestamo.Id }, prestamo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/devolver")]
        public async Task<IActionResult> DevolverLibro(int id, [FromBody] string? observaciones = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _prestamoService.DevolverLibroAsync(id, userId, observaciones);

            if (!result)
                return BadRequest(new { message = "No se pudo procesar la devolución" });

            return NoContent();
        }
    }
}