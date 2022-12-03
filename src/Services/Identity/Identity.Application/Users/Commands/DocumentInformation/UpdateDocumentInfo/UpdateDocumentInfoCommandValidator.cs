﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using sattec.Identity.Application.Users.Commands.DocumentInformation.CreateDocumentInfo;

namespace sattec.Identity.Application.Users.Commands.DocumentInformation.UpdateDocumentInfo
{
    public class FileValidator : AbstractValidator<IFormFile>
    {
        public FileValidator()
        {
            RuleFor(x => x.Length).NotNull().LessThanOrEqualTo(1048576)
                .WithMessage("File size is larger than allowed");

            RuleFor(x => x.ContentType).NotNull().Must(x => x.Equals("image/jpeg") || x.Equals("image/jpg") || x.Equals("image/png"))
                .WithMessage("format file is wrong!.. valid formats are (jpg, jpeg, png)");
        }
    }
    public class UpdateDocumentInfoCommandValidator : AbstractValidator<UpdateDocumentInfoCommand>
    {
        public UpdateDocumentInfoCommandValidator()
        {
            RuleFor(x => x.File).SetValidator(new FileValidator());
        }
    }
}
