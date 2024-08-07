using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using BusinessLayer.Middlewares;
using DataAccessLayer.Concrete.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Contract.Request;
using Shared.DTO.Contract.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class ContractRepository : IContractRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public ContractRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    #region CreateContract

   public async Task<ContractResponse> CreateContract(CreateContractRequest createContractRequest)
{
    try
    {
        var contract = new Contract();

        // Tüm sözleşmeleri getir ve bellek içinde aynı isimde sözleşme olup olmadığını kontrol et
        var existingContracts = await _applicationDbContext.Contracts
            .AsNoTracking()
            .ToListAsync();

        var existingContractWithSameName = existingContracts
            .Where(c => c.Name.Trim().Equals(createContractRequest.Name.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (existingContractWithSameName.Count == 0)
        {
            // Aynı isimde sözleşme yoksa versiyon 1.0 ile başlat
            contract.Version = 1.0;
            contract.PreviousVersion = 0.0;
        }
        else
        {
            // Aynı isimde sözleşme varsa, en son eklenenin versiyonunu artır
            var lastAddedContract = existingContractWithSameName.MaxBy(c => c.Version);

            if (lastAddedContract != null)
            {
                contract.Version = Math.Round(lastAddedContract.Version + 0.1, 1);
                contract.PreviousVersion = lastAddedContract.Version;
            }
            else
            {
                contract.Version = 1.0;
                contract.PreviousVersion = 0.0;
            }
        }

        // Yeni sözleşme bilgilerini belirle
        contract.Name = createContractRequest.Name.Trim();
        contract.Body = createContractRequest.Body.Trim();
        contract.Type = createContractRequest.Type;
        contract.CreateDate = DateTime.UtcNow;
        contract.RequiresAt = createContractRequest.RequiresAt.Trim();
        contract.IsActive = createContractRequest.IsActive;

        // Yeni sözleşmeyi veritabanına ekle ve değişiklikleri kaydet
        _applicationDbContext.Contracts.Add(contract);
        await _applicationDbContext.SaveChangesAsync();

        // Yeni sözleşmeyi ContractResponse tipine dönüştür ve döndür
        return _mapper.Map<ContractResponse>(contract);
    }
    catch (Exception ex)
    {
        // Hata yakalama ve loglama
        // Loglama kodunuza uygun şekilde buraya ekleyebilirsiniz
        throw new Exception($"Something went wrong: {ex.Message}");
    }
}
    #endregion

    #region GetContracts

    public async Task<List<GetContractsResponse>> GetContracts(
        Expression<Func<GetContractsResponse, bool>>? predicate = null)
    {
        var contracts = _applicationDbContext.Contracts
            .AsNoTracking();

        if (predicate != null)
        {
            var contractPredicate = _mapper.MapExpression<Expression<Func<Contract, bool>>>(predicate);
            contracts = contracts.Where(contractPredicate);
        }
        
        var contractList = await contracts
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .ToListAsync();
        
        var contractResponse = _mapper.Map<List<GetContractsResponse>>(contractList);
        
        return contractResponse;
    }


    #endregion

    #region GetContract

    public async Task<GetContractResponse> GetContract(Guid contractId)
    {
        var contact = await _applicationDbContext.Contracts
                          .AsNoTracking()
                          .FirstOrDefaultAsync(x => x.ContractId == contractId)
                      ?? throw new NotFoundException(ContractExceptionMessages.NotFound);

        return _mapper.Map<GetContractResponse>(contact);
    }

    #endregion

    #region IsExist

    private async Task<bool> IsExistGeneric(Expression<Func<Contract, bool>> filter)
    {
        var result = await _applicationDbContext.Contracts.AnyAsync(filter);

        return result;
    }

    #endregion
}