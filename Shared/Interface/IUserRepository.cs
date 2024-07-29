using Shared.DTO.Authentication.Response;
using Shared.DTO.User.Request;
using Shared.DTO.User.Response;

namespace Shared.Interface;

public interface IUserRepository
{
    /// <summary>
    /// Veritabanından tüm kullanıcıları getirir ve kullanıcı bilgilerini içeren bir liste döner.
    /// </summary>
    /// <returns>Tüm kullanıcıların bilgilerini içeren GetUsersResponse nesnelerinin listesi.</returns>
    Task<List<GetUsersResponse>> GetUsers();

    /// <summary>
    /// Verilen kullanıcı ID'sine göre kullanıcı bilgilerini getirir.
    /// </summary>
    /// <param name="userId">Getirilecek kullanıcının ID'si.</param>
    /// <returns>Kullanıcı bilgilerini içeren GetUserResponse nesnesi.</returns>
    /// <exception cref="KeyNotFoundException">Kullanıcı bulunamadığında fırlatılır.</exception>
    Task<GetUserResponse> GetUserById(Guid userId);

    /// <summary>
    /// Yeni bir kullanıcı oluşturur ve kullanıcı bilgilerini içeren bir nesne döner.
    /// </summary>
    /// <param name="request">Kullanıcı oluşturmak için gerekli bilgileri içeren istek nesnesi.</param>
    /// <returns>Oluşturulan kullanıcının bilgilerini içeren UserResponse nesnesi.</returns>
    Task<UserResponse> CreateUser(CreateUserRequest request);

    /// <summary>
    /// Verilen email adresine sahip bir kullanıcının mevcut olup olmadığını kontrol eder.
    /// </summary>
    /// <param name="email">Kontrol edilecek email adresi.</param>
    /// <returns>Email adresine sahip bir kullanıcının olup olmadığını belirten bir boolean değer döner.</returns>
    Task<bool> IsExistAsync(string email);

    /// <summary>
    /// Verilen email adresine göre kullanıcı bilgilerini getirir.
    /// </summary>
    /// <param name="email">Getirilecek kullanıcının email adresi.</param>
    /// <returns>Kullanıcı bilgilerini içeren UserDetailsResponse nesnesi.</returns>
    /// <exception cref="KeyNotFoundException">Kullanıcı bulunamadığında fırlatılır.</exception>
    Task<UserDetailsResponse> GetUserByEmail(string email);

    /// <summary>
    /// Verilen güncelleme isteğine göre bir kullanıcıyı günceller ve kullanıcı bilgilerini içeren bir nesne döner.
    /// </summary>
    /// <param name="request">Kullanıcıyı güncellemek için gerekli bilgileri içeren istek nesnesi.</param>
    /// <returns>Güncellenmiş kullanıcının bilgilerini içeren UserResponse nesnesi.</returns>
    /// <exception cref="KeyNotFoundException">Kullanıcı bulunamadığında fırlatılır.</exception>
    Task<UserResponse> UpdateUser(UpdateUserRequest request);

    /// <summary>
    /// Verilen kullanıcı ID'sine göre kullanıcıyı siler ve silinen kullanıcının bilgilerini içeren bir nesne döner.
    /// </summary>
    /// <param name="userId">Silinecek kullanıcının ID'si.</param>
    /// <returns>Silinen kullanıcının bilgilerini içeren UserResponse nesnesi.</returns>
    /// <exception cref="KeyNotFoundException">Kullanıcı bulunamadığında fırlatılır.</exception>
    Task<UserResponse> DeleteUser(Guid userId);
}
