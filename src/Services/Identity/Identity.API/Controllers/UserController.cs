using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sattec.Identity.Application.Users.Commands.Signup;
using sattec.Identity.Application.Users.Commands.Signin;
using sattec.Identity.Application.Users.Commands.ConfirmMobile;
using sattec.Identity.Application.Users.Commands.ForgetPassword;
using sattec.Identity.Application.Users.Commands.ResetPassword;
using sattec.Identity.Application.Common.Models;
using sattec.Identity.Application.Users.Commands.IdentityInformation;
using sattec.Identity.Application.Users.Commands.ContactInformation;
using sattec.Identity.Application.Users.Commands.InsuranceInformation.CreateInsuranceInformation;
using sattec.Identity.Application.Users.Commands.InsuranceInformation.UpdateInsuranceInformation;
using sattec.Identity.Application.Users.Commands.DocumentInformation.CreateDocumentInfo;
using sattec.Identity.Application.Users.Commands.DocumentInformation.UpdateDocumentInfo;
using sattec.Identity.Application.Users.Commands.EmailConfirm;
using sattec.Identity.Application.Users.Commands.BankAccountInfromation.CreateBankAccount;
using sattec.Identity.Application.Users.Commands.BrandInformation.UpdateBrandInformationCommand;
using sattec.Identity.Application.Users.Commands.BrandInformation.CreateBrandInformation;

namespace sattec.Identity.WebUI.Controllers;

public class UserController : ApiControllerBase
{
    [Route("Signup"),HttpPost]
    public async Task<ActionResult<SignupResponse>> Signup(SignupCommand command)
    {
        return await Mediator.Send(command);
    }
    [Route("ConfirmMobile"), HttpPost]
    public async Task<ActionResult<Result>> MobileConfirm([FromBody] MobileConfirmCommand command)
    {
        return await Mediator.Send(command);
    }
    [Route("EmailConfirm"), HttpPost]
    public async Task<ActionResult<Result>> EmailConfirm([FromBody] EmailConfirmCommand command)
    {
        return await Mediator.Send(command);
    }
    [Route("Signin"),HttpPost]
    public async Task<ActionResult<SigninResponse>> Signin(SigninCommand command)
    {
        return await Mediator.Send(command);
    }
    [Route("ForgetPassword"), HttpPost]
    public async Task<ActionResult<ForgetPasswordResponse>> ForgetPassword(ForgetPasswordCommand command)
    {
        return await Mediator.Send(command);
    }
    [Route("ResetPassword"), HttpPost]
    public async Task<ActionResult<Result>> ResetPassword(ResetPasswordCommand command)
    {
        return await Mediator.Send(command);
    }
    [Route("IentityInformation"), HttpPost]
    public async Task<ActionResult<Result>> IdentityInformation([FromForm] IdentityInformationCommand command)
    {
        return await Mediator.Send(command);
    }
    [Route("ContactInformation"), HttpPost]
    public async Task<ActionResult<Result>> ContactInformation(ContactInformationCommand command)
    {
        return await Mediator.Send(command);
    } 
    [Route("InsuranceInformation"), HttpPost]
    public async Task<ActionResult<Result>> PostInsuranceInformation(CreateInsuranceInformationCommand command)
    {
        return await Mediator.Send(command);
    } 
    [Route("InsuranceInformation"), HttpPut]
    public async Task<ActionResult<Result>> PutInsuranceInformation(UpdateInsuranceInformationCommand command)
    {
        return await Mediator.Send(command);
    }
    [Route("DocumentInfo"), HttpPost]
    public async Task<ActionResult<Result>> PostDocumentInfo([FromForm] CreateDocumentInfoCommand command)
    {
        return await Mediator.Send(command);
    }
    [Route("DocumentInfo"), HttpPut]
    public async Task<ActionResult<Result>> PutDocumentInfo([FromForm] UpdateDocumentInfoCommand command)
    {
        return await Mediator.Send(command);
    }  
    [Route("BrandInformation"), HttpPost]
    public async Task<ActionResult<Result>> PostBrandInformation([FromForm] CreateBrandInformationCommand command)
    {
        return await Mediator.Send(command);
    }
    [Route("BrandInformation"), HttpPut]
    public async Task<ActionResult<Result>> PutBrandInformation([FromForm] UpdateBrandInformationCommand command)
    {
        return await Mediator.Send(command);
    }
}
