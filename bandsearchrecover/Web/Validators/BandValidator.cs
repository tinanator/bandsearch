using BandSearch.Models;
using FluentValidation;

namespace BandSearch.Web.Validators
{
    public class BandValidator : AbstractValidator<Band>
    {
        public BandValidator()
        {
            RuleFor(x => x.Name).NotNull();
        }
    }
}
