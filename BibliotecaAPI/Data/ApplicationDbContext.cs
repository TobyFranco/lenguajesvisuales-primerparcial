using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BibliotecaAPI.Models;

namespace BibliotecaAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para las entidades
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<LibroAutor> LibrosAutores { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuración de la tabla LibroAutor (relación N:M)
            builder.Entity<LibroAutor>(entity =>
            {
                entity.HasKey(la => new { la.LibroId, la.AutorId });

                entity.HasOne(la => la.Libro)
                    .WithMany(l => l.LibrosAutores)
                    .HasForeignKey(la => la.LibroId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(la => la.Autor)
                    .WithMany(a => a.LibrosAutores)
                    .HasForeignKey(la => la.AutorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Libro
            builder.Entity<Libro>(entity =>
            {
                entity.HasIndex(l => l.ISBN).IsUnique();
                entity.HasIndex(l => l.Titulo);
                entity.HasIndex(l => l.Disponible);

                entity.HasOne(l => l.Categoria)
                    .WithMany(c => c.Libros)
                    .HasForeignKey(l => l.CategoriaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Prestamo
            builder.Entity<Prestamo>(entity =>
            {
                entity.HasIndex(p => p.Estado);
                entity.HasIndex(p => p.FechaDevolucionEsperada);

                entity.HasOne(p => p.Usuario)
                    .WithMany(u => u.Prestamos)
                    .HasForeignKey(p => p.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Libro)
                    .WithMany(l => l.Prestamos)
                    .HasForeignKey(p => p.LibroId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Apellido).IsRequired().HasMaxLength(100);
            });

            // Seed Data Básico (sin fechas para evitar errores)
            SeedBasicData(builder);
        }

        private void SeedBasicData(ModelBuilder builder)
        {
            // Fecha fija para seed data (evita el warning de valores dinámicos)
            var fechaCreacion = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Seed Categorias
            builder.Entity<Categoria>().HasData(
                new Categoria
                {
                    Id = 1,
                    Nombre = "Ficción",
                    Descripcion = "Novelas y cuentos de ficción",
                    FechaCreacion = fechaCreacion
                },
                new Categoria
                {
                    Id = 2,
                    Nombre = "Ciencia",
                    Descripcion = "Libros científicos y técnicos",
                    FechaCreacion = fechaCreacion
                },
                new Categoria
                {
                    Id = 3,
                    Nombre = "Historia",
                    Descripcion = "Libros de historia y biografías",
                    FechaCreacion = fechaCreacion
                },
                new Categoria
                {
                    Id = 4,
                    Nombre = "Tecnología",
                    Descripcion = "Libros sobre programación y tecnología",
                    FechaCreacion = fechaCreacion
                },
                new Categoria
                {
                    Id = 5,
                    Nombre = "Literatura Clásica",
                    Descripcion = "Obras clásicas de la literatura universal",
                    FechaCreacion = fechaCreacion
                }
            );

            // Seed Autores
            builder.Entity<Autor>().HasData(
                new Autor
                {
                    Id = 1,
                    Nombre = "Gabriel",
                    Apellido = "García Márquez",
                    FechaNacimiento = new DateTime(1927, 3, 6),
                    Nacionalidad = "Colombiano",
                    FechaCreacion = fechaCreacion
                },
                new Autor
                {
                    Id = 2,
                    Nombre = "Miguel",
                    Apellido = "de Cervantes",
                    FechaNacimiento = new DateTime(1547, 9, 29),
                    Nacionalidad = "Español",
                    FechaCreacion = fechaCreacion
                },
                new Autor
                {
                    Id = 3,
                    Nombre = "Stephen",
                    Apellido = "Hawking",
                    FechaNacimiento = new DateTime(1942, 1, 8),
                    Nacionalidad = "Británico",
                    FechaCreacion = fechaCreacion
                },
                new Autor
                {
                    Id = 4,
                    Nombre = "Robert",
                    Apellido = "Martin",
                    FechaNacimiento = new DateTime(1952, 12, 5),
                    Nacionalidad = "Estadounidense",
                    FechaCreacion = fechaCreacion
                },
                new Autor
                {
                    Id = 5,
                    Nombre = "Yuval Noah",
                    Apellido = "Harari",
                    FechaNacimiento = new DateTime(1976, 2, 24),
                    Nacionalidad = "Israelí",
                    FechaCreacion = fechaCreacion
                }
            );

            // Seed Libros
            builder.Entity<Libro>().HasData(
                new Libro
                {
                    Id = 1,
                    Titulo = "Cien años de soledad",
                    ISBN = "978-84-376-0495-7",
                    AñoPublicacion = 1967,
                    NumeroPaginas = 471,
                    CategoriaId = 1,
                    Descripcion = "Una obra maestra del realismo mágico",
                    Disponible = true,
                    FechaCreacion = fechaCreacion
                },
                new Libro
                {
                    Id = 2,
                    Titulo = "Don Quijote de la Mancha",
                    ISBN = "978-84-376-0496-4",
                    AñoPublicacion = 1605,
                    NumeroPaginas = 863,
                    CategoriaId = 5,
                    Descripcion = "La primera novela moderna",
                    Disponible = true,
                    FechaCreacion = fechaCreacion
                },
                new Libro
                {
                    Id = 3,
                    Titulo = "Breve historia del tiempo",
                    ISBN = "978-84-376-0497-1",
                    AñoPublicacion = 1988,
                    NumeroPaginas = 256,
                    CategoriaId = 2,
                    Descripcion = "Una introducción a la cosmología moderna",
                    Disponible = true,
                    FechaCreacion = fechaCreacion
                }
            );

            // Seed LibroAutor (relaciones)
            builder.Entity<LibroAutor>().HasData(
                new LibroAutor { LibroId = 1, AutorId = 1 },
                new LibroAutor { LibroId = 2, AutorId = 2 },
                new LibroAutor { LibroId = 3, AutorId = 3 }
            );
        }
    }
}