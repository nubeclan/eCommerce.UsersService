using eCommerce.UsersService.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace eCommerce.UsersService.Api.Database;

/**
 * Contexto de base de datos de la aplicación
 * 
 * Gestiona la conexión y configuración de Entity Framework Core para el microservicio de usuarios.
 *
 * @author: Angel Céspedes Quiroz
 * @Whatsapp: +591 33264587
 * @Linkedin: https://bo.linkedin.com/in/acq1305
 *
 */
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Constructor del contexto de base de datos.
    /// </summary>
    /// <param name="options">Opciones de configuración para el contexto.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    #region Entities
    /// <summary>
    /// Conjunto de entidades de usuarios.
    /// </summary>
    public DbSet<User> Users { get; set; }
    #endregion

    /// <summary>
    /// Configura el modelo de datos al crear el contexto.
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo para configurar entidades.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
