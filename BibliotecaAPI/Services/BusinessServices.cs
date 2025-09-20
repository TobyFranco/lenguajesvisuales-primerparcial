using Microsoft.EntityFrameworkCore;
using BibliotecaAPI.Data;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Models;

namespace BibliotecaAPI.Services
{
    // Servicio para Autores
    public interface IAutorService
    {
        Task<IEnumerable<AutorDto>> GetAllAsync();
        Task<AutorDto?> GetByIdAsync(int id);
        Task<AutorDto> CreateAsync(CreateAutorDto createAutorDto);
        Task<bool> UpdateAsync(int id, CreateAutorDto updateAutorDto);
        Task<bool> DeleteAsync(int id);
    }

    public class AutorService : IAutorService
    {
        private readonly ApplicationDbContext _context;

        public AutorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AutorDto>> GetAllAsync()
        {
            return await _context.Autores
                .Select(a => new AutorDto
                {
                    Id = a.Id,
                    Nombre = a.Nombre,
                    Apellido = a.Apellido,
                    FechaNacimiento = a.FechaNacimiento,
                    Nacionalidad = a.Nacionalidad
                })
                .ToListAsync();
        }

        public async Task<AutorDto?> GetByIdAsync(int id)
        {
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null) return null;

            return new AutorDto
            {
                Id = autor.Id,
                Nombre = autor.Nombre,
                Apellido = autor.Apellido,
                FechaNacimiento = autor.FechaNacimiento,
                Nacionalidad = autor.Nacionalidad
            };
        }

        public async Task<AutorDto> CreateAsync(CreateAutorDto createAutorDto)
        {
            var autor = new Autor
            {
                Nombre = createAutorDto.Nombre,
                Apellido = createAutorDto.Apellido,
                FechaNacimiento = createAutorDto.FechaNacimiento,
                Nacionalidad = createAutorDto.Nacionalidad,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();

            return new AutorDto
            {
                Id = autor.Id,
                Nombre = autor.Nombre,
                Apellido = autor.Apellido,
                FechaNacimiento = autor.FechaNacimiento,
                Nacionalidad = autor.Nacionalidad
            };
        }

        public async Task<bool> UpdateAsync(int id, CreateAutorDto updateAutorDto)
        {
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null) return false;

            autor.Nombre = updateAutorDto.Nombre;
            autor.Apellido = updateAutorDto.Apellido;
            autor.FechaNacimiento = updateAutorDto.FechaNacimiento;
            autor.Nacionalidad = updateAutorDto.Nacionalidad;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var autor = await _context.Autores.FindAsync(id);
            if (autor == null) return false;

            _context.Autores.Remove(autor);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    // Servicio para Categorías
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaDto>> GetAllAsync();
        Task<CategoriaDto?> GetByIdAsync(int id);
        Task<CategoriaDto> CreateAsync(CreateCategoriaDto createCategoriaDto);
        Task<bool> UpdateAsync(int id, CreateCategoriaDto updateCategoriaDto);
        Task<bool> DeleteAsync(int id);
    }

    public class CategoriaService : ICategoriaService
    {
        private readonly ApplicationDbContext _context;

        public CategoriaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoriaDto>> GetAllAsync()
        {
            return await _context.Categorias
                .Select(c => new CategoriaDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    CantidadLibros = c.Libros.Count
                })
                .ToListAsync();
        }

        public async Task<CategoriaDto?> GetByIdAsync(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Libros)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null) return null;

            return new CategoriaDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                CantidadLibros = categoria.Libros.Count
            };
        }

        public async Task<CategoriaDto> CreateAsync(CreateCategoriaDto createCategoriaDto)
        {
            var categoria = new Categoria
            {
                Nombre = createCategoriaDto.Nombre,
                Descripcion = createCategoriaDto.Descripcion,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return new CategoriaDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                CantidadLibros = 0
            };
        }

        public async Task<bool> UpdateAsync(int id, CreateCategoriaDto updateCategoriaDto)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return false;

            categoria.Nombre = updateCategoriaDto.Nombre;
            categoria.Descripcion = updateCategoriaDto.Descripcion;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return false;

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    // Servicio para Libros
    public interface ILibroService
    {
        Task<IEnumerable<LibroDto>> GetAllAsync(string? titulo = null, int? categoriaId = null, bool? disponible = null);
        Task<LibroDto?> GetByIdAsync(int id);
        Task<LibroDto> CreateAsync(CreateLibroDto createLibroDto);
        Task<bool> UpdateAsync(int id, CreateLibroDto updateLibroDto);
        Task<bool> DeleteAsync(int id);
    }

    public class LibroService : ILibroService
    {
        private readonly ApplicationDbContext _context;

        public LibroService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LibroDto>> GetAllAsync(string? titulo = null, int? categoriaId = null, bool? disponible = null)
        {
            var query = _context.Libros
                .Include(l => l.Categoria)
                .Include(l => l.LibrosAutores)
                    .ThenInclude(la => la.Autor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(titulo))
                query = query.Where(l => l.Titulo.Contains(titulo));

            if (categoriaId.HasValue)
                query = query.Where(l => l.CategoriaId == categoriaId.Value);

            if (disponible.HasValue)
                query = query.Where(l => l.Disponible == disponible.Value);

            return await query.Select(l => new LibroDto
            {
                Id = l.Id,
                Titulo = l.Titulo,
                ISBN = l.ISBN,
                AñoPublicacion = l.AñoPublicacion,
                NumeroPaginas = l.NumeroPaginas,
                Descripcion = l.Descripcion,
                Disponible = l.Disponible,
                Categoria = new CategoriaDto
                {
                    Id = l.Categoria.Id,
                    Nombre = l.Categoria.Nombre,
                    Descripcion = l.Categoria.Descripcion
                },
                Autores = l.LibrosAutores.Select(la => new AutorDto
                {
                    Id = la.Autor.Id,
                    Nombre = la.Autor.Nombre,
                    Apellido = la.Autor.Apellido,
                    FechaNacimiento = la.Autor.FechaNacimiento,
                    Nacionalidad = la.Autor.Nacionalidad
                }).ToList()
            }).ToListAsync();
        }

        public async Task<LibroDto?> GetByIdAsync(int id)
        {
            var libro = await _context.Libros
                .Include(l => l.Categoria)
                .Include(l => l.LibrosAutores)
                    .ThenInclude(la => la.Autor)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (libro == null) return null;

            return new LibroDto
            {
                Id = libro.Id,
                Titulo = libro.Titulo,
                ISBN = libro.ISBN,
                AñoPublicacion = libro.AñoPublicacion,
                NumeroPaginas = libro.NumeroPaginas,
                Descripcion = libro.Descripcion,
                Disponible = libro.Disponible,
                Categoria = new CategoriaDto
                {
                    Id = libro.Categoria.Id,
                    Nombre = libro.Categoria.Nombre,
                    Descripcion = libro.Categoria.Descripcion
                },
                Autores = libro.LibrosAutores.Select(la => new AutorDto
                {
                    Id = la.Autor.Id,
                    Nombre = la.Autor.Nombre,
                    Apellido = la.Autor.Apellido,
                    FechaNacimiento = la.Autor.FechaNacimiento,
                    Nacionalidad = la.Autor.Nacionalidad
                }).ToList()
            };
        }

        public async Task<LibroDto> CreateAsync(CreateLibroDto createLibroDto)
        {
            // Validar que la categoría existe
            var categoria = await _context.Categorias.FindAsync(createLibroDto.CategoriaId);
            if (categoria == null)
                throw new ArgumentException("La categoría especificada no existe");

            // Validar que todos los autores existen
            var autores = await _context.Autores
                .Where(a => createLibroDto.AutoresIds.Contains(a.Id))
                .ToListAsync();

            if (autores.Count != createLibroDto.AutoresIds.Count)
                throw new ArgumentException("Uno o más autores especificados no existen");

            var libro = new Libro
            {
                Titulo = createLibroDto.Titulo,
                ISBN = createLibroDto.ISBN,
                AñoPublicacion = createLibroDto.AñoPublicacion,
                NumeroPaginas = createLibroDto.NumeroPaginas,
                Descripcion = createLibroDto.Descripcion,
                CategoriaId = createLibroDto.CategoriaId,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();

            // Agregar relaciones con autores
            foreach (var autorId in createLibroDto.AutoresIds)
            {
                _context.LibrosAutores.Add(new LibroAutor
                {
                    LibroId = libro.Id,
                    AutorId = autorId
                });
            }

            await _context.SaveChangesAsync();

            return await GetByIdAsync(libro.Id);
        }

        public async Task<bool> UpdateAsync(int id, CreateLibroDto updateLibroDto)
        {
            var libro = await _context.Libros
                .Include(l => l.LibrosAutores)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (libro == null) return false;

            // Validar categoría
            var categoria = await _context.Categorias.FindAsync(updateLibroDto.CategoriaId);
            if (categoria == null)
                throw new ArgumentException("La categoría especificada no existe");

            // Validar autores
            var autores = await _context.Autores
                .Where(a => updateLibroDto.AutoresIds.Contains(a.Id))
                .ToListAsync();

            if (autores.Count != updateLibroDto.AutoresIds.Count)
                throw new ArgumentException("Uno o más autores especificados no existen");

            // Actualizar libro
            libro.Titulo = updateLibroDto.Titulo;
            libro.ISBN = updateLibroDto.ISBN;
            libro.AñoPublicacion = updateLibroDto.AñoPublicacion;
            libro.NumeroPaginas = updateLibroDto.NumeroPaginas;
            libro.Descripcion = updateLibroDto.Descripcion;
            libro.CategoriaId = updateLibroDto.CategoriaId;

            // Actualizar relaciones con autores
            _context.LibrosAutores.RemoveRange(libro.LibrosAutores);

            foreach (var autorId in updateLibroDto.AutoresIds)
            {
                _context.LibrosAutores.Add(new LibroAutor
                {
                    LibroId = libro.Id,
                    AutorId = autorId
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null) return false;

            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    // Servicio para Préstamos
    public interface IPrestamoService
    {
        Task<IEnumerable<PrestamoDto>> GetAllAsync(string? usuarioId = null, int? estado = null);
        Task<PrestamoDto?> GetByIdAsync(int id);
        Task<PrestamoDto> CreateAsync(CreatePrestamoDto createPrestamoDto, string usuarioId);
        Task<bool> DevolverLibroAsync(int prestamoId, string usuarioId, string? observaciones = null);
    }

    public class PrestamoService : IPrestamoService
    {
        private readonly ApplicationDbContext _context;

        public PrestamoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PrestamoDto>> GetAllAsync(string? usuarioId = null, int? estado = null)
        {
            var query = _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Libro)
                    .ThenInclude(l => l.Categoria)
                .Include(p => p.Libro.LibrosAutores)
                    .ThenInclude(la => la.Autor)
                .AsQueryable();

            if (!string.IsNullOrEmpty(usuarioId))
                query = query.Where(p => p.UsuarioId == usuarioId);

            if (estado.HasValue)
                query = query.Where(p => (int)p.Estado == estado.Value);

            return await query.Select(p => new PrestamoDto
            {
                Id = p.Id,
                FechaPrestamo = p.FechaPrestamo,
                FechaDevolucionEsperada = p.FechaDevolucionEsperada,
                FechaDevolucionReal = p.FechaDevolucionReal,
                Observaciones = p.Observaciones,
                Estado = p.Estado.ToString(),
                UsuarioNombre = $"{p.Usuario.Nombre} {p.Usuario.Apellido}",
                UsuarioEmail = p.Usuario.Email,
                Libro = new LibroDto
                {
                    Id = p.Libro.Id,
                    Titulo = p.Libro.Titulo,
                    ISBN = p.Libro.ISBN,
                    AñoPublicacion = p.Libro.AñoPublicacion,
                    NumeroPaginas = p.Libro.NumeroPaginas,
                    Descripcion = p.Libro.Descripcion,
                    Disponible = p.Libro.Disponible,
                    Categoria = new CategoriaDto
                    {
                        Id = p.Libro.Categoria.Id,
                        Nombre = p.Libro.Categoria.Nombre,
                        Descripcion = p.Libro.Categoria.Descripcion
                    },
                    Autores = p.Libro.LibrosAutores.Select(la => new AutorDto
                    {
                        Id = la.Autor.Id,
                        Nombre = la.Autor.Nombre,
                        Apellido = la.Autor.Apellido,
                        FechaNacimiento = la.Autor.FechaNacimiento,
                        Nacionalidad = la.Autor.Nacionalidad
                    }).ToList()
                },
                DiasRestantes = p.Estado == EstadoPrestamo.Activo
                    ? (int)(p.FechaDevolucionEsperada - DateTime.UtcNow).TotalDays
                    : 0
            }).ToListAsync();
        }

        public async Task<PrestamoDto?> GetByIdAsync(int id)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Libro)
                    .ThenInclude(l => l.Categoria)
                .Include(p => p.Libro.LibrosAutores)
                    .ThenInclude(la => la.Autor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prestamo == null) return null;

            return new PrestamoDto
            {
                Id = prestamo.Id,
                FechaPrestamo = prestamo.FechaPrestamo,
                FechaDevolucionEsperada = prestamo.FechaDevolucionEsperada,
                FechaDevolucionReal = prestamo.FechaDevolucionReal,
                Observaciones = prestamo.Observaciones,
                Estado = prestamo.Estado.ToString(),
                UsuarioNombre = $"{prestamo.Usuario.Nombre} {prestamo.Usuario.Apellido}",
                UsuarioEmail = prestamo.Usuario.Email,
                Libro = new LibroDto
                {
                    Id = prestamo.Libro.Id,
                    Titulo = prestamo.Libro.Titulo,
                    ISBN = prestamo.Libro.ISBN,
                    AñoPublicacion = prestamo.Libro.AñoPublicacion,
                    NumeroPaginas = prestamo.Libro.NumeroPaginas,
                    Descripcion = prestamo.Libro.Descripcion,
                    Disponible = prestamo.Libro.Disponible,
                    Categoria = new CategoriaDto
                    {
                        Id = prestamo.Libro.Categoria.Id,
                        Nombre = prestamo.Libro.Categoria.Nombre,
                        Descripcion = prestamo.Libro.Categoria.Descripcion
                    },
                    Autores = prestamo.Libro.LibrosAutores.Select(la => new AutorDto
                    {
                        Id = la.Autor.Id,
                        Nombre = la.Autor.Nombre,
                        Apellido = la.Autor.Apellido,
                        FechaNacimiento = la.Autor.FechaNacimiento,
                        Nacionalidad = la.Autor.Nacionalidad
                    }).ToList()
                },
                DiasRestantes = prestamo.Estado == EstadoPrestamo.Activo
                    ? (int)(prestamo.FechaDevolucionEsperada - DateTime.UtcNow).TotalDays
                    : 0
            };
        }

        public async Task<PrestamoDto> CreateAsync(CreatePrestamoDto createPrestamoDto, string usuarioId)
        {
            // Validar que el libro existe y está disponible
            var libro = await _context.Libros.FindAsync(createPrestamoDto.LibroId);
            if (libro == null)
                throw new ArgumentException("El libro especificado no existe");

            if (!libro.Disponible)
                throw new ArgumentException("El libro no está disponible para préstamo");

            // Validar que el usuario no tenga préstamos vencidos
            var prestamosVencidos = await _context.Prestamos
                .Where(p => p.UsuarioId == usuarioId
                    && p.Estado == EstadoPrestamo.Activo
                    && p.FechaDevolucionEsperada < DateTime.UtcNow)
                .CountAsync();

            if (prestamosVencidos > 0)
                throw new ArgumentException("No puedes realizar nuevos préstamos hasta devolver los libros vencidos");

            // Crear el préstamo
            var prestamo = new Prestamo
            {
                LibroId = createPrestamoDto.LibroId,
                UsuarioId = usuarioId,
                FechaPrestamo = DateTime.UtcNow,
                FechaDevolucionEsperada = createPrestamoDto.FechaDevolucionEsperada,
                Observaciones = createPrestamoDto.Observaciones,
                Estado = EstadoPrestamo.Activo
            };

            _context.Prestamos.Add(prestamo);

            // Marcar el libro como no disponible
            libro.Disponible = false;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(prestamo.Id);
        }

        public async Task<bool> DevolverLibroAsync(int prestamoId, string usuarioId, string? observaciones = null)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(p => p.Id == prestamoId && p.UsuarioId == usuarioId);

            if (prestamo == null || prestamo.Estado != EstadoPrestamo.Activo)
                return false;

            // Actualizar el préstamo
            prestamo.FechaDevolucionReal = DateTime.UtcNow;
            prestamo.Estado = EstadoPrestamo.Devuelto;

            if (!string.IsNullOrEmpty(observaciones))
                prestamo.Observaciones += $" | Devolución: {observaciones}";

            // Marcar el libro como disponible
            prestamo.Libro.Disponible = true;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}