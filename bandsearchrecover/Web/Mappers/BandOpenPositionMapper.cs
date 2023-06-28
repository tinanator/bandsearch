using BandSearch.Models;
using BandSearch.Web.DTOs;
using Riok.Mapperly.Abstractions;

namespace BandSearch.Web.Mappers
{
    [Mapper]
    public partial class BandOpenPositionMapper
    {
        public partial BandOpenPositionDTO OpenPositionToOpenPositionDTO(BandOpenPosition bandOpenPosition);

        public partial BandOpenPosition OpenPositionDTOToOpenPosition(BandOpenPositionDTO bandOpenPosition);
    }
}
