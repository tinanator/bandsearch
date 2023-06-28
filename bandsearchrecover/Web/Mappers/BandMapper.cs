using BandSearch.Models;
using BandSearch.Web.DTOs;
using Riok.Mapperly.Abstractions;

namespace BandSearch.Web.Mappers
{
    [Mapper]
    public partial class BandMapper
    {
        public partial BandDTO BandToBandDTO(Band band);

        public partial Band BandDTOToBand(BandDTO band);
    }
}
