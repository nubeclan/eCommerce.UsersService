namespace eCommerce.UsersService.Api.Contracts.Users;

/**
 * Solicitud para registrar un nuevo usuario
 * 
 * DTO utilizado para recibir los datos de registro de usuario desde el cliente.
 *
 * @author: Angel Céspedes Quiroz
 * @Whatsapp: +591 33264587
 * @Linkedin: https://bo.linkedin.com/in/acq1305
 *
 */
public record RegisterRequest(
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
    string Email,
    /// <summary>
    /// Contraseña del usuario (será encriptada).
    /// </summary>
    string Password);