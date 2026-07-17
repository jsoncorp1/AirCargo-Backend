using AC.Application.Abstractions.Messaging.Commands;
using AC.Application.Modules.Users.Specifications;
using AC.Application.Services.Security;
using AC.Domain.Modules.Users;
using AC.Domain.Persistence;
using AC.Domain.Results;

namespace AC.Application.Modules.Auth.Commands.Login;

public class LoginCommandHandler(
    IRepository<User> repository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService)
    : ICommandHandler<LoginCommand, LoginCommandResult>
{
    public async Task<Result<LoginCommandResult>> HandleAsync(
        LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await repository.GetBySpecificationAsync(
            new UserByEmailSpecification(command.Email), cancellationToken);

        if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash))
            return Result.Fail<LoginCommandResult>(
                "Credenciales inválidas.", "auth.invalidcredentials");

        var token = tokenService.GenerateToken(user);

        return Result.Success(new LoginCommandResult
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.Name,
            Token = token
        });
    }
}