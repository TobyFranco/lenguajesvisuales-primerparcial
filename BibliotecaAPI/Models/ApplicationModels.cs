using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotecaAPI.Models
{
    // 1. Usuario - Extiende IdentityUser
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Relación 1:N con Prestamos
        public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }

    // 2. Autor
    public class Autor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        [MaxLength(50)]
        public string Nacionalidad { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación N:M con Libros
        public virtual ICollection<LibroAutor> LibrosAutores { get; set; } = new List<LibroAutor>();
    }

    // 3. Categoria
    public class Categoria
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [MaxLength(500)]
        public string Descripcion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación 1:N con Libros
        public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
    }

    // 4. Libro
    public class Libro
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; }

        [Required]
        [MaxLength(20)]
        public string ISBN { get; set; }

        public int AñoPublicacion { get; set; }

        public int NumeroPaginas { get; set; }

        [MaxLength(1000)]
        public string Descripcion { get; set; }

        public bool Disponible { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación N:1 con Categoria
        [Required]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; }

        // Relación N:M con Autores
        public virtual ICollection<LibroAutor> LibrosAutores { get; set; } = new List<LibroAutor>();

        // Relación 1:N con Prestamos
        public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }

    // 5. Prestamo
    public class Prestamo
    {
        public int Id { get; set; }

        public DateTime FechaPrestamo { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime FechaDevolucionEsperada { get; set; }

        public DateTime? FechaDevolucionReal { get; set; }

        [MaxLength(500)]
        public string Observaciones { get; set; }

        public EstadoPrestamo Estado { get; set; } = EstadoPrestamo.Activo;

        // Relación N:1 con Usuario
        [Required]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser Usuario { get; set; }

        // Relación N:1 con Libro
        [Required]
        public int LibroId { get; set; }

        [ForeignKey("LibroId")]
        public virtual Libro Libro { get; set; }
    }

    // Tabla intermedia para relación N:M entre Libro y Autor
    public class LibroAutor
    {
        public int LibroId { get; set; }
        public int AutorId { get; set; }

        [ForeignKey("LibroId")]
        public virtual Libro Libro { get; set; }

        [ForeignKey("AutorId")]
        public virtual Autor Autor { get; set; }
    }

    // Enums
    public enum EstadoPrestamo
    {
        Activo = 1,
        Devuelto = 2,
        Vencido = 3,
        Perdido = 4
    }
}