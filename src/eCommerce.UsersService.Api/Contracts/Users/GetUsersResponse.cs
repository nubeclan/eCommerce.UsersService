using System.Collections.Generic;

namespace eCommerce.UsersService.Api.Contracts.Users;

/**
 * Respuesta para obtener todos los usuarios
 * 
 * DTO utilizado para devolver una lista de usuarios.
 *
 * @author: Angel Céspedes Quiroz
 * @Whatsapp: +591 33264587
 * @Linkedin: https://bo.linkedin.com/in/acq1305
 *
 */

public record GetUsersResponse(
    /// <summary>
    /// Lista de usuarios.
    /// </summary>
    List<GetUserResponse> Users);