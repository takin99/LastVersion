using sattec.Identity.Application.Common.Interfaces;
using sattec.Identity.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using IdentityModel;
using sattec.Identity.Application.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Twilio.Exceptions;
using Twilio.Rest.Verify.V2.Service;
using Twilio;
using sattec.Identity.Domain.Entities;
using Microsoft.AspNetCore.Http;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace sattec.Identity.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IApplicationDbContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly string verifyServiceSid;
    private readonly string authToken;
    private readonly string accountSid;
    private readonly IConfiguration _configuration;
    private ApplicationUser? _user;
    private IJwtUtils _jwtUtils;
    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IApplicationDbContext context,
        IAuthorizationService authorizationService,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IJwtUtils jwtUtils
       )
    {
        _jwtUtils = jwtUtils;
        _userManager = userManager;
        _configuration = configuration;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _context = context;
        _authorizationService = authorizationService;
        _signInManager = signInManager;
        //  verifyServiceSid = configuration["AC97759d22c0b980cc0dae48bdee41b734"];
        accountSid = "AC97759d22c0b980cc0dae48bdee41b734";
        authToken = "5370e9dbb6f0b8646b32e69deebcf562";
        TwilioClient.Init(accountSid, authToken);
    }
    public async Task<string> GetUserNameAsync(string userId)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

        return user.UserName;
    }
    public Result FindByPhoneNumber(string phoneNumber)
    {
        var user = _userManager.Users.Where(x => x.PhoneNumber == phoneNumber).SingleOrDefault();

        return Result.Success();
    }
    public async Task<string> CreateUserAsync(string firstName, string lastName, string userName, string email, string PhoneNumber, string password, string confirmPassword)
    {
       
        Random generator = new();
        var confirmCode = generator.Next(99999, 1000000).ToString();

        var newUser = new ApplicationUser
        {
            MobileVerificationCode = confirmCode,
            FirstName = firstName,
            LastName = lastName,
            UserName = userName,
            Email = email,
            PhoneNumber = PhoneNumber,
            ConfirmPassWord = confirmPassword
        };
        if (password != confirmPassword)
            throw new NotFoundException("password not match");

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(confirmPassword);

        var createUser = await _userManager.CreateAsync(newUser, password);

        string result = Convert.ToString(newUser.MobileVerificationCode);

        return result;
    }
    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }
    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }
    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }
    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }
    public Result GetByMobileVerificationCode(string verifiCode)
    {

        var user = _userManager.Users.Where(u => u.MobileVerificationCode != null && u.MobileVerificationCode == verifiCode).SingleOrDefault();

        if (user == null)

            throw new NotFoundException("verification code is not found");

        user.MobileVerificationCodeExpireTime = DateTime.UtcNow.AddHours(1);

        if (user.MobileVerificationCodeExpireTime < DateTime.UtcNow)
        {
            throw new NotFoundException("Verification code has expired!");
        }

        user.MobileIsVerifed = true;

        // Todo
        //به عنوان کاربر می خواهم در صورتی که کد ثبت شده درست بود جهت جلوگیری از استفاده در دفعات بعد منقضی گردد.
        return Result.Success();
    }
    public Result GetByEmailVerification(string email)
    {
        var user = _userManager.Users.Where(u => u.Email == email).SingleOrDefault();

        if (user == null)

            throw new NotFoundException("email is not found");

        user.EmailConfirmed = true;

        return Result.Success();
    }
    public async Task<(string token, string userId)> LoginByUserPassAsync(string phoneNumber, string password)
    {
        var targetUser = _userManager.Users.Where(u => u.PhoneNumber == phoneNumber).SingleOrDefault();

        var signinResult = targetUser == null
            ? SignInResult.Failed
            : await _signInManager.CheckPasswordSignInAsync(targetUser, password, true);

        if (!signinResult.Succeeded)
            throw new NotFoundException("please enter valid data");

        var date = DateTime.UtcNow.AddMinutes(-10);

        if (!targetUser.FailedLoginTry(date, 3))
        {
            if (phoneNumber != null)
            {
                throw new NotFoundException("Login has been disabled for 30 minutes");
            }
        }
        //Todo
        //در صورت تلاش ناموفق چهارم اکانت مربوطه برای 30 دقیقه برای ورود غیرفعال گردد.
        var claimsPrincipal = await getClaims(targetUser);

        var claims = claimsPrincipal.Claims.ToArray().AsEnumerable();

        var token = await _userManager.GeneratePasswordResetTokenAsync(targetUser);

        return (token, targetUser.Id);
    }
    public string GetByPhoneNumber(string phoneNumber)
    {
        var user = _userManager.Users.Where(x => x.PhoneNumber == phoneNumber).SingleOrDefault();

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        if (user != null && !user.MobileIsVerifed)
        {
            throw new NotFoundException("PhoneNumber not confirmed yet");
        }
        Random generator = new();
        user.MobileVerificationCode = generator.Next(99999, 1000000).ToString();
        user.MobileVerificationCodeExpireTime = DateTime.UtcNow.AddHours(1);

        //   var sms = new SMSMessage() { To = user.PhoneNumber, Message = "code is send" };

        return user.MobileVerificationCode;
    }
    public Result ResetPassword(string code, string newPassword)
    {
        var userTask = _userManager.Users.Where(x => x.MobileVerificationCode == code).SingleOrDefault();

        if (userTask == null)
            throw new NotFoundException("User not found");

        ChangeUserPassword(userTask, newPassword);

        return Result.Success();
    }
    public async Task<Result> CreateUserIdentityInfo(Guid id, string firstName, string lastName, string fatherName, string nationalId, string identitySerialNumber, DateTime birthday, string birthPlace, IFormFile nationalCardFile)
    {
        var user = _userManager.Users.Where(x => x.Id == id.ToString()).SingleOrDefault();

        if (user == null)
        {
            var newUser = new ApplicationUser
            {
                UserName = firstName,
                LastName = lastName,
                FatherName = fatherName,
                NationalId = nationalId,
                IdentitySerialNumber = identitySerialNumber,
                BirthDay = birthday,
                BirthPlace = birthPlace,
                NationalCardFile = nationalCardFile 
            };

            var Result = await _userManager.CreateAsync(newUser);
        }
        else
        {
            user.FirstName = firstName;
            user.LastName = lastName;
            user.FatherName = fatherName;
            user.NationalId = nationalId;
            user.IdentitySerialNumber = identitySerialNumber;
            user.BirthDay = birthday;
            user.BirthPlace = birthPlace;
            user.NationalCardFile = nationalCardFile;

            var Result = await _userManager.UpdateAsync(user);
        }
        //Todo
        //همچنین بتوانم تصاویر شناسنامه و کارت ملی خود را آپلود نماییم. برای این کار هم امکان تصویر برداری از گوشی موبایل یا انتخاب از روی مسیری در سیستم فراهم باشد.
        return Result.Success();
    }
    public async Task<Result> CreateContactInformation(Guid id, string essentialPhone, string phoneNumber, string postalCode, string address, string country, string state, string city, string description)
    {
        var user = _userManager.Users.Where(x => x.Id == id.ToString()).SingleOrDefault();

        if (user == null)
        {
            var newUser = new ApplicationUser
            {
                UserName= id.ToString(),
                Id = id.ToString(),
                EssentialPhone = essentialPhone,
                PhoneNumber = phoneNumber,
                PostalCode = postalCode,
                Address = address,
                Country = country,
                State = state,
                City = city,
                Description = description
            };

            var Result = await _userManager.CreateAsync(newUser);
        }
        else
        {
            user.EssentialPhone = essentialPhone;
            user.PhoneNumber = phoneNumber;
            user.PostalCode = postalCode;
            user.Address = address;
            user.Country = country;
            user.State = state;
            user.City = city;
            user.Description = description;

            var Result = await _userManager.UpdateAsync(user);
        }

        return Result.Success();
    }
    public Result CreateInsuranceInfo(string userId, string name, string insuranceNumber)
    {
        var user = _userManager.Users.Where(u => u.Id == userId).SingleOrDefault();

        if (user == null)
            throw new NotFoundException("User not found");

        var data = new Organizations
        {
            UserId = userId,
            Name = name,
            InsuranceNumber = insuranceNumber
        };

        user.AddOrganization(data);

        return Result.Success();
    }
    public Result UpdateInsuranceInfo(string userId, string name, string insuranceNumber)
    {
        var user = _userManager.Users.Where(u => u.Id == userId).FirstOrDefault();

        if (user == null)
            throw new NotFoundException("User not found");

        var info = _context.Organization.Where(b => b.UserId == userId).FirstOrDefault();

        if (info == null)
            throw new NotFoundException("info not found");

        if (!user.Organization.Any())
            throw new NotFoundException("Organization not found");

        var organization = user.Organization.Where(b => b.InsuranceNumber == insuranceNumber).SingleOrDefault();

        organization.InsuranceNumber = insuranceNumber;
        organization.Name = name;

        return Result.Success();
    }
    public Result CreateBankInfo(string userId, bool isDefaultAccount, Guid? bankId, string title, string accountNo, string cardNo, string iban, string accountName, string description)
    {
        var user = _userManager.Users.Where(u => u.Id == userId).SingleOrDefault();

        if (user == null)
            throw new NotFoundException("User not found");

        var newBankAccount = new BankAccount
            {   
                UserId = userId,
                IsDefaultAccount = isDefaultAccount,
                BankId = bankId,
                Title = title,
                AccountNo = accountNo,
                CardNo = cardNo,
                IBAN = iban,
                AccountName = user.UserName,
                Description = description
            };
       
            user.AddBankAccount(newBankAccount);
            user.IsEnableDeafaultAccount((Guid)newBankAccount.BankId, newBankAccount.IsDefaultAccount);
        //Todo
        //از بین همه حساب ها حتما یکی از حساب ها باید پیش فرض باشد.
        //آخرین حسابی که ثبت می شود تیک پیش فرض برای کاربر فعال باشد.
        //نام بانک از لیست بانک های دارای مجوز انتخاب شود.
      
        return Result.Success();
    }
    public Result UpdateBankInfo(string userId, bool isDefaultAccount, Guid bankId, string title, string accountNo, string cardNo, string iban, string accountName, string description)
    {
        var user = _userManager.Users.Where(u => u.Id == userId).FirstOrDefault();

        if (user == null)
            throw new NotFoundException("User not found");

        var info = _context.BankAccounts.Where(b=>b.UserId==userId).FirstOrDefault();

        if (info == null)
            throw new NotFoundException("info not found");

        if (!user.BankAccounts.Any())
            throw new NotFoundException("BankAccounts not found");

        var bankAccount= user.BankAccounts.Where(b=>b.AccountNo == accountNo).SingleOrDefault();

        bankAccount.Title = title;
        bankAccount.CardNo = cardNo;
        bankAccount.AccountNo = accountNo;
        bankAccount.IBAN = iban;
        bankAccount.AccountName = accountName;
        bankAccount.Description = description;

        return Result.Success();
    }
    public Result CreateDocument(string userId, string description, IFormFile file)
    {
        var user = _userManager.Users.Where(u => u.Id == userId).SingleOrDefault();

        if (user == null)
        {  
            throw new NotFoundException("User not found");
        }

        var data = new Documentation
        {
            UserId = userId,
            Description = description,
            File = file
        };
        user.AddDocumentation(data);

        return Result.Success();
    }
    public Result UpdateDocument(string userId, string description, IFormFile file)
    {
        var user = _userManager.Users.Where(u => u.Id == userId).FirstOrDefault();

        if (user == null)
            throw new NotFoundException("User not found");

        var info = _context.BankAccounts.Where(b => b.UserId == userId).FirstOrDefault();

        if (info == null)
            throw new NotFoundException("info not found");

        if (!user.Documentation.Any())
            throw new NotFoundException("Documentation not found");

        var documentation = user.Documentation.Where(b => b.UserId == userId).SingleOrDefault();

        documentation.Description = description;
        documentation.File = file;

        return Result.Success();
    }
    public Result CreateBrandInformation(string userId, string brandName, string address, string phoneNumber, string registrationNumber, IFormFile logo, string brandOwner)
    {
        var user = _userManager.Users.Where(u => u.Id == userId).SingleOrDefault();

        if (user == null)
            throw new NotFoundException("User not found");

        var data = new Brand
        {
            UserId = userId,
            BrandOwner = brandOwner,
           Address = address,
           PhoneNumber = phoneNumber,
           RegistrationNumber = registrationNumber,
           Logo = logo,
           BrandName = brandName
        };

        user.AddBrand(data);

        return Result.Success();
    }
    public Result UpdateBrandInformation(string userId, string brandName, string address, string phoneNumber, string registrationNumber, IFormFile logo, string brandOwner)
    {
        var user = _userManager.Users.Where(u => u.Id == userId).FirstOrDefault();

        if (user == null)
            throw new NotFoundException("User not found");

        var info = _context.Brand.Where(b => b.UserId == userId).FirstOrDefault();

        if (info == null)
            throw new NotFoundException("info not found");

        if (!user.Brands.Any())
            throw new NotFoundException("Brand infromation not found");

        var brand = user.Brands.Where(b => b.RegistrationNumber == registrationNumber).SingleOrDefault();

        if (brand == null)
        {
            throw new NotFoundException("RegistrationNumber is wrong.");
        }

        brand.BrandName = brandName;
        brand.Address = address;
        brand.PhoneNumber = phoneNumber;
        brand.RegistrationNumber = registrationNumber;
        brand.Logo = logo;
        brand.BrandOwner = brandOwner;

        return Result.Success();
    }
  
    public async Task<string> StartVerificationAsync(string phoneNumber)
    {
        string result = "";
        try
        {
            var verificationResource = await VerificationResource.CreateAsync(
                to: phoneNumber,
                channel: "sms",
                pathServiceSid: accountSid
            );
            return result;
        }
        catch (TwilioException e)
        {
            throw new NotFoundException("Wrong code. Try again.");
        }
        return result;
    }
    public async Task<Result> CheckVerificationAsync(string phoneNumber, string code)
    {
        var verificationCheckResource = await VerificationCheckResource.CreateAsync(
            to: phoneNumber,
            code: code,
            pathServiceSid: accountSid
        );
        return verificationCheckResource.Status.Equals("approved") ?
        Result.Success() :
        throw new NotFoundException("Wrong code. Try again.");
    }
    private async Task<ClaimsPrincipal> getClaims(ApplicationUser user)
    {
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(JwtClaimTypes.PreferredUserName, user.UserName));
        claimsIdentity.AddClaims(await _userManager.GetClaimsAsync(user));
        return new ClaimsPrincipal(claimsIdentity);
    }
    private string GeneratePasswordResetToken(ApplicationUser user)
    {
        var task = _userManager.GeneratePasswordResetTokenAsync(user);
        task.Wait();
        var token = task.Result;
        return token;
    }
    private void ChangeUserPassword(ApplicationUser user, string newPassword)
    {
        var token = GeneratePasswordResetToken(user);
        var task = _userManager.ResetPasswordAsync(user, token, newPassword);
        task.Wait();
        var result = task.Result;
    }
}
