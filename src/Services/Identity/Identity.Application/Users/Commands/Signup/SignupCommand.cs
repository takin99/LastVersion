using sattec.Identity.Application.Common.Interfaces;
using MediatR;

namespace sattec.Identity.Application.Users.Commands.Signup;

public record SignupCommand : IRequest<SignupResponse>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; init; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; init; }
    public string? PassWord { get; init; }
    public string? ConfirmPassWord { get; init; }
}
public class SignupResponse
{
    public string Code { get; init; }
}
public class SignupCommandHandler : IRequestHandler<SignupCommand, SignupResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
   
    public SignupCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<SignupResponse> Handle(SignupCommand request, CancellationToken cancellationToken)
    {
        var user = _identityService.FindByPhoneNumber(request.PhoneNumber);

        var newUser = await _identityService.CreateUserAsync(request.FirstName, request.LastName, request.UserName, request.Email, request.PhoneNumber, request.PassWord, request.ConfirmPassWord);

        await _context.SaveChangesAsync(cancellationToken);

        return new SignupResponse() { Code = newUser };

    }
}
