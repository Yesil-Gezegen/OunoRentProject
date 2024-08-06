using AutoMapper;
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
        var contract = new Contract();

        var lastAddedContract = await _applicationDbContext.Contracts
            .AsNoTracking()
            .OrderByDescending(c => c.Version)
            .FirstOrDefaultAsync();

        if (lastAddedContract == null)
        {
            contract.Version = 0;
            contract.PreviousVersion = 0;
        }
        else
        {
            contract.Version = (Math.Round(lastAddedContract.Version, 2)) + 0.1;
            contract.PreviousVersion = lastAddedContract.Version;
        }
        
        contract.Name = createContractRequest.Name.Trim();
        contract.Body = createContractRequest.Body.Trim();
        contract.Type = createContractRequest.Type;
        contract.CreateDate = DateTime.UtcNow;
        contract.RequiresAt = createContractRequest.RequiresAt.Trim();
        contract.IsActive = createContractRequest.IsActive;

        _applicationDbContext.Contracts.Add(contract);

        await _applicationDbContext.SaveChangesAsync();

        return _mapper.Map<ContractResponse>(contract);
    }

    #endregion

    #region GetContracts

    public async Task<List<GetContractsResponse>> GetContracts()
    {
        var contractList = await _applicationDbContext.Contracts
            .AsNoTracking()
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .ToListAsync();

        return _mapper.Map<List<GetContractsResponse>>(contractList);
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
}