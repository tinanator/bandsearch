using BandSearch.Web.DTOs;
using FluentValidation;

public class UserValidator : AbstractValidator<UpdateUserDTO>
{
    public UserValidator()
    {
        RuleFor(x => x.Name).NotNull();
        RuleFor(x => x.Surname).NotNull();
        RuleFor(x => x.Gender).NotNull();
    }
}
