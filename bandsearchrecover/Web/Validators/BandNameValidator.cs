using BandSearch.Models;
using FluentValidation;

namespace BandSearch.Web.Validators
{
    public class BandNameValidator : AbstractValidator<string>
    {
        public BandNameValidator()
        {
            RuleFor(x => x).NotEmpty();
        }
    }
}
