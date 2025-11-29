namespace eCommerce.UsersService.Api.Contracts.Users;

/**
 * Solicitud para actualizar un usuario
 * 
 * DTO utilizado para recibir los datos de actualización de usuario.
 *
 * @author: Angel Céspedes Quiroz
 * @Whatsapp: +591 33264587
 * @Linkedin: https://bo.linkedin.com/in/acq1305
 *
 */
public record UpdateUserRequest(
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