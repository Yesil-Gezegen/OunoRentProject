using AutoMapper;
using BusinessLayer.Middlewares;
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

    public async Task<ContractResponse> CreateContract(CreateContractRequest createContractRequest)
    {
        var contract = new Contract();
        
        contract.Name = createContractRequest.Name.Trim();
        contract.Version = 1;
        contract.PreviousVersion = 0;
        contract.Body = createContractRequest.Body.Trim();
        contract.Type = createContractRequest.Type;
        contract.CreateDate = DateTime.UtcNow;
        contract.RequiresAt = createContractRequest.RequiresAt.Trim();
        contract.IsActive = createContractRequest.IsActive;

        _applicationDbContext.Contracts.Add(contract);
        
        await _applicationDbContext.SaveChangesAsync();
        
        return _mapper.Map<ContractResponse>(contract);
    }

    public async Task<ContractResponse> UpdateContract(UpdateContractRequest updateContractRequest)
    {
        var contract = await _applicationDbContext.Contracts
                          .FirstOrDefaultAsync(x => x.ContractId == updateContractRequest.ContractId)
                      ?? throw new NotFoundException(ContractExceptionMessages.NotFound);
        
        contract.Name = updateContractRequest.Name.Trim();
        contract.Version = contract.Version + 1;
        contract.PreviousVersion = contract.Version -1;
        contract.Body = updateContractRequest.Body.Trim();
        contract.Type = updateContractRequest.Type;
        contract.RequiresAt = updateContractRequest.RequiresAt.Trim();
        contract.IsActive = updateContractRequest.IsActive;

        _applicationDbContext.Contracts.Update(contract);
        
        await _applicationDbContext.SaveChangesAsync();
        
        return _mapper.Map<ContractResponse>(contract);
    }

    public async Task<List<GetContractsResponse>> GetContracts()
    {
        var contractList = await _applicationDbContext.Contracts
            .AsNoTracking()
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .ToListAsync();
        
        return _mapper.Map<List<GetContractsResponse>>(contractList);
    }

    public async Task<GetContractResponse> GetContract(Guid contractId)
    {
        var contact = await _applicationDbContext.Contracts
            .AsNoTracking()
            .FirstOrDefaultAsync(x=> x.ContractId == contractId)
                        ?? throw new NotFoundException(ContractExceptionMessages.NotFound);
        
        return _mapper.Map<GetContractResponse>(contact);
    }

    public async Task<Guid> DeleteContract(Guid contractId)
    {
        var contract = await _applicationDbContext.Contracts
                          .FirstOrDefaultAsync(x => x.ContractId == contractId)
                      ?? throw new NotFoundException(ContractExceptionMessages.NotFound);
        
        _applicationDbContext.Contracts.Remove(contract);
        
        await _applicationDbContext.SaveChangesAsync();
        
        return contract.ContractId;
    }
}