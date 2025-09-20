using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs
{
    // Authentication DTOs
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class RegisterDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public string UserName { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
    }

    // Autor DTOs
    public class AutorDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Nacionalidad { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }

    public class CreateAutorDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        [MaxLength(50)]
        public string Nacionalidad { get; set; }
    }

    // Categoria DTOs
    public class CategoriaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int CantidadLibros { get; set; }
    }

    public class CreateCategoriaDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [MaxLength(500)]
        public string Descripcion { get; set; }
    }

    // Libro DTOs
    public class LibroDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string ISBN { get; set; }
        public int AñoPublicacion { get; set; }
        public int NumeroPaginas { get; set; }
        public string Descripcion { get; set; }
        public bool Disponible { get; set; }
        public CategoriaDto Categoria { get; set; }
        public List<AutorDto> Autores { get; set; } = new List<AutorDto>();
    }

    public class CreateLibroDto
    {
        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; }

        [Required]
        [MaxLength(20)]
        public string ISBN { get; set; }

        [Range(1000, 3000)]
        public int AñoPublicacion { get; set; }

        [Range(1, 10000)]
        public int NumeroPaginas { get; set; }

        [MaxLength(1000)]
        public string Descripcion { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        [Required]
        public List<int> AutoresIds { get; set; } = new List<int>();
    }

    // Prestamo DTOs
    public class PrestamoDto
    {
        public int Id { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevolucionEsperada { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public string Observaciones { get; set; }
        public string Estado { get; set; }
        public string UsuarioNombre { get; set; }
        public string UsuarioEmail { get; set; }
        public LibroDto Libro { get; set; }
        public int DiasRestantes { get; set; }
    }

    public class CreatePrestamoDto
    {
        [Required]
        public int LibroId { get; set; }

        [Required]
        public DateTime FechaDevolucionEsperada { get; set; }

        [MaxLength(500)]
        public string Observaciones { get; set; }
    }
}