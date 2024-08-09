using System.Linq.Expressions;
using Shared.DTO.Category.Request;
using Shared.DTO.Category.Response;

namespace Shared.Interface;

public interface ICategoryRepository
{
    /// <summary>
    /// Veritabanından tüm kategorileri getirir ve kategori bilgilerini içeren bir liste döner.
    /// </summary>
    /// <returns>Tüm kategorilerin bilgilerini içeren GetCategoriesResponse nesnelerinin listesi.</returns>
    Task<List<GetCategoriesResponse>> GetCategories(Expression<Func<GetCategoryResponse, bool>>? predicate = null);

    /// <summary>
    /// Verilen kategori ID'sine göre kategoriyi getirir ve kategori bilgilerini içeren bir nesne döner.
    /// </summary>
    /// <param name="categoryId">Getirilecek kategorinin ID'si.</param>
    /// <returns>Kategori bilgilerini içeren GetCategoryResponse nesnesi.</returns>
    /// <exception cref="KeyNotFoundException">Kategori bulunamadığında fırlatılır.</exception>
    Task<GetCategoryResponse> GetCategory(Guid categoryId);

    /// <summary>
    /// Yeni bir kategori oluşturur ve kategori bilgilerini içeren bir nesne döner.
    /// </summary>
    /// <param name="categoryName">Oluşturulacak kategorinin adı.</param>
    /// <returns>Oluşturulan kategorinin bilgilerini içeren CategoryResponse nesnesi.</returns>
    Task<CategoryResponse> CreateCategory(CreateCategoryRequest createCategoryRequest);

    /// <summary>
    /// Verilen güncelleme isteğine göre bir kategoriyi günceller ve kategori bilgilerini içeren bir nesne döner.
    /// </summary>
    /// <param name="request">Kategoriyi güncellemek için gerekli bilgileri içeren istek nesnesi.</param>
    /// <returns>Güncellenmiş kategorinin bilgilerini içeren CategoryResponse nesnesi.</returns>
    /// <exception cref="KeyNotFoundException">Kategori bulunamadığında fırlatılır.</exception>
    Task<CategoryResponse> UpdateCategory(UpdateCategoryRequest request);

    /// <summary>
    /// Verilen kategori ID'sine göre kategoriyi siler ve silinen kategorinin ID'sini döner.
    /// </summary>
    /// <param name="categoryId">Silinecek kategorinin ID'si.</param>
    /// <returns>Silinen kategorinin ID'si.</returns>
    /// <exception cref="KeyNotFoundException">Kategori bulunamadığında fırlatılır.</exception>
    Task<Guid> DeleteCategory(Guid categoryId);
    Task<byte[]> ExportCategoriesToExcel(Expression<Func<GetCategoriesResponse, bool>>? predicate = null);
}