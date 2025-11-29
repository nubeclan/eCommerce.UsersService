namespace eCommerce.UsersService.Api.Contracts.Users;

/**
 * Solicitud para iniciar sesión
 * 
 * DTO utilizado para recibir las credenciales de login.
 *
 * @author: Angel Céspedes Quiroz
 * @Whatsapp: +591 33264587
 * @Linkedin: https://bo.linkedin.com/in/acq1305
 *
 */

public record LoginRequest(
    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    string Email,
    /// <summary>
    /// Contraseña del usuario.
    /// </summary>
    string Password);
