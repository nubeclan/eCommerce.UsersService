namespace eCommerce.UsersService.Api.Contracts.Users;

/**
 * Respuesta para obtener un usuario
 * 
 * DTO utilizado para devolver los datos de un usuario específico.
 *
 * @author: Angel Céspedes Quiroz
 * @Whatsapp: +591 33264587
 * @Linkedin: https://bo.linkedin.com/in/acq1305
 *
 */
public record GetUserResponse(
    /// <summary>
    /// Identificador único del usuario.
    /// </summary>
    Guid UserID,
    /// <summary>
    /// Nombre del usuario.
    /// </summary>
    string FirstName,
    /// <summary>
    /// Apellido del usuario.
    /// </summary>
    string LastName,
    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    string Email);