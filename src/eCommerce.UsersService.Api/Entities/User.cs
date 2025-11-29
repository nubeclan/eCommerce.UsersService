namespace eCommerce.UsersService.Api.Entities;

/**
 * Entidad User
 * 
 * Representa un usuario en el sistema eCommerce.
 * 
 *          pk
 * User [UserID, FirstName, LastName, Email, Password]
 *
 * @author: Angel Céspedes Quiroz
 * @Whatsapp: +591 33264587
 * @Linkedin: https://bo.linkedin.com/in/acq1305
 *
 */

public class User
{
    /// <summary>
    /// Identificador único del usuario (clave primaria).
    /// </summary>
    public Guid UserID { get; set; }

    /// <summary>
    /// Nombre del usuario.
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Apellido del usuario.
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Correo electrónico del usuario (debe ser único).
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Contraseña encriptada del usuario.
    /// </summary>
    public string Password { get; set; } = null!;
}
