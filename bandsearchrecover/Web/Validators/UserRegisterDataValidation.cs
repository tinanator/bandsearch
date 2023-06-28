using BandSearch.Web.DTOs;
using FluentValidation;

namespace BandSearch.Web.Validators
{
    public class UserRegisterDataValidation : AbstractValidator<UserRegisterDataDTO>
    {
        public UserRegisterDataValidation() 
        {
            RuleFor(x => x.Name).NotNull();
            RuleFor(x => x.Surname).NotNull();
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Gender).NotNull().InclusiveBetween(0, 1);
            RuleFor(x => x.Age).LessThan(100).GreaterThan(0);
            RuleFor(x => x.Password).NotNull().MinimumLength(8);
        }
    }
}
