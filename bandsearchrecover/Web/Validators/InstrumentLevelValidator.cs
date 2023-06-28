using BandSearch.Web.DTOs;
using FluentValidation;

namespace BandSearch.Web.Validators
{
    public class InstrumentLevelValidator : AbstractValidator<InstrumentLevelDTO>
    {
        public InstrumentLevelValidator()
        {
            RuleFor(x => x.Instrument).NotEmpty();
            RuleFor(x => x.Level).IsInEnum();
        }
    }
}
