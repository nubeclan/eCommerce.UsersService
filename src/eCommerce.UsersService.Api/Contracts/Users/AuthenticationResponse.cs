namespace eCommerce.UsersService.Api.Contracts.Users;

/**
 * Respuesta de autenticación
 * 
 * DTO utilizado para devolver el token JWT y datos del usuario autenticado.
 *
 * @author: Angel Céspedes Quiroz
 * @Whatsapp: +591 33264587
 * @Linkedin: https://bo.linkedin.com/in/acq1305
 *
 */
public record AuthenticationResponse(
    /// <summary>
    /// Token JWT para autenticación.
    /// </summary>
    string Token,
    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    string Email,
    /// <summary>
    /// Nombre del usuario.
    /// </summary>
    string FirstName,
    /// <summary>
    /// Apellido del usuario.
    /// </summary>
    string LastName);
