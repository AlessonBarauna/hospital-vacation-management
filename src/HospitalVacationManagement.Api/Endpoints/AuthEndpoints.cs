using HospitalVacationManagement.Application.Abstractions;
using HospitalVacationManagement.Application.Authentication;

namespace HospitalVacationManagement.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (
            LoginRequest request,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            CancellationToken cancellationToken) =>
        {
            var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user is null || !user.IsActive)
            {
                return Results.Unauthorized();
            }

            var passwordIsValid = passwordHasher.Verify(
                request.Password,
                user.PasswordHash);

            if (!passwordIsValid)
            {
                return Results.Unauthorized();
            }

            var loginResponse = jwtTokenGenerator.Generate(
                user.Id,
                user.Email,
                user.Role.ToString());

            return Results.Ok(loginResponse);
        });

        return app;
    }
}