using AutoMapper;
using BusinessLayer.Middlewares;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Authentication.Response;
using Shared.DTO.User.Request;
using Shared.DTO.User.Response;
using Shared.Interface;

namespace DataAccessLayer.Concrete.Repository;

public class UserRepository : IUserRepository
{

    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    #region CreateUser
    public async Task<UserResponse> CreateUser(CreateUserRequest request)
    {
        var user = new User
        {
            Email = request.Email,
            AccountStatus = true,
            PasswordHash = request.PasswordHash,
        };

        _applicationDbContext.Users.Add(user);

        await _applicationDbContext.SaveChangesAsync();

        var userResponse = _mapper.Map<UserResponse>(user);
        return userResponse;
    }
    #endregion

    #region DeleteUser
    public async Task<UserResponse> DeleteUser(Guid userId)
    {
        var deletedUser = _applicationDbContext.Users
        .FirstOrDefault(x => x.Id == userId)
        ?? throw new NotFoundException(UserExceptionMessage.NotFound);

        _applicationDbContext.Users.Remove(deletedUser);

        await _applicationDbContext.SaveChangesAsync();

        var userResponse = _mapper.Map<UserResponse>(deletedUser);
        return userResponse;
    }
    #endregion

    #region GetUserById
    public async Task<GetUserResponse> GetUserById(Guid userId)
    {
        var user = await _applicationDbContext.Users
        .AsNoTracking()
        .Where(x => x.Id == userId)
        .FirstOrDefaultAsync()
        ?? throw new NotFoundException(UserExceptionMessage.NotFound);

        var userResponse = _mapper.Map<GetUserResponse>(user);
        return userResponse;
    }
    #endregion

    #region IsExistAsync
    public async Task<bool> IsExistAsync(string email)
    {
        var isExist = await _applicationDbContext.Users
        .AsNoTracking()
        .AnyAsync(u => u.Email == email);
        return isExist;
    }
    #endregion

    #region GetUserByEmail
    public async Task<UserDetailsResponse> GetUserByEmail(string email)
    {
        var user = await _applicationDbContext.Users
            .AsNoTracking()
            .Where(x => x.Email == email)
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException(UserExceptionMessage.NotFound);

        var userDetailsResponse = _mapper.Map<UserDetailsResponse>(user);
        return userDetailsResponse;
    }
    #endregion

    #region GetUsers
    public async Task<List<GetUsersResponse>> GetUsers()
    {
        var users = await _applicationDbContext.Users
        .AsNoTracking()
        .OrderByDescending(x => x.ModifiedDateTime ?? x.CreatedDateTime)
        .ToListAsync();

        var usersResponse = _mapper.Map<List<GetUsersResponse>>(users);
        return usersResponse;
    }
    #endregion

    #region UpdateUser

    public async Task<UserResponse> UpdateUser(UpdateUserRequest request)
    {
        var user = await _applicationDbContext.Users
        .FirstOrDefaultAsync(x => x.Id == request.Id)
        ?? throw new NotFoundException(UserExceptionMessage.NotFound);

        user.Name = request.Name;
        user.Surname = request.Surname;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;
        user.Address = request.Address;
        user.TC = request.Tc;
        user.Gender = request.Gender;
        user.BirthDate = request.BirthDate;

        _applicationDbContext.Update(user);

        await _applicationDbContext.SaveChangesAsync();

        var userResponse = _mapper.Map<UserResponse>(user);

        return userResponse;
    }
    #endregion
}
