using BandSearch.Web.DTOs;
using FluentValidation;

namespace BandSearch.Web.Validators
{
    public class BandOpenPositionValidator : AbstractValidator<BandOpenPositionDTO>
    {
        public BandOpenPositionValidator()
        {
            RuleFor(x => x.AgeMin).GreaterThan(0);
            RuleFor(x => x.AgeMax).LessThan(100);
            RuleFor(x => x.Gender).InclusiveBetween(0, 1);
            RuleFor(x => x.InstrumentLevel)
            .SetValidator(new InstrumentLevelValidator()).When(x => x.InstrumentLevel != null);
        }
    }
}
