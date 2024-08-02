using System.Linq.Expressions;
using AutoMapper;
using BusinessLayer.Middlewares;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.UserContracts.Request;
using Shared.DTO.UserContracts.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class UserContractRepository : IUserContractRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public UserContractRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    #region CreateUserContract

    public async Task<UserContractResponse> CraateUserContractAsync(CreateUserContractRequest createUserContractRequest)
    {
        await HasConflict(createUserContractRequest);

        var userContract = new UserContract();

        userContract.UserId = createUserContractRequest.UserId;
        userContract.ContractId = createUserContractRequest.ContractId;
        userContract.FileName = createUserContractRequest.FileName.Trim();
        userContract.Date = DateTime.Now.ToUniversalTime();

        await _applicationDbContext.UserContracts.AddAsync(userContract);
        await _applicationDbContext.SaveChangesAsync();

        return _mapper.Map<UserContractResponse>(userContract);
    }

    #endregion

    #region GetUserContract

    public async Task<GetUserContractResponse> GetUserContractAsync(Guid userContractId)
    {
        var userContract =
            await _applicationDbContext.UserContracts
                .Include(uc => uc.User)
                .Include(uc => uc.Contract)
                .AsNoTracking()
                .FirstOrDefaultAsync(uc => uc.UserContractId == userContractId) ??
            throw new NotFoundException(UserContractExceptionMessages.NotFound);

        return _mapper.Map<GetUserContractResponse>(userContract);
    }

    #endregion

    #region GetUserContracts

    public async Task<List<GetUserContractsResponse>> GetUserContractsAsync(
        Expression<Func<GetUserContractResponse, bool>>? predicate = null)
    {
        var userContracts = _applicationDbContext.UserContracts
            .Include(uc => uc.User)
            .Include(uc => uc.Contract)
            .AsNoTracking();

        if (predicate != null)
        {
            var userContractPredicate = _mapper.Map<Expression<Func<UserContract, bool>>>(predicate);
            userContracts.Where(userContractPredicate);
        }

        var userContractList = await userContracts
            .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
            .ToListAsync();

        return _mapper.Map<List<GetUserContractsResponse>>(userContractList);
    }

    #endregion

    #region HasConflict

    public async Task HasConflict(CreateUserContractRequest createUserContractRequest)
    {
        var isUserExist = await _applicationDbContext.Users.AnyAsync(u => u.Id == createUserContractRequest.UserId);

        var isContractExist =
            await _applicationDbContext.Contracts.AnyAsync(c => c.ContractId == createUserContractRequest.ContractId);

        if (!isUserExist)
            throw new NotFoundException(UserExceptionMessage.NotFound);

        if (!isContractExist)
            throw new NotFoundException(ContractExceptionMessages.NotFound);
    }

    #endregion
}