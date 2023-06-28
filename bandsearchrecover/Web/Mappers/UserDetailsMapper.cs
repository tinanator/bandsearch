using BandSearch.Web.BusinessModels;
using BandSearch.Web.DTOs;
using Riok.Mapperly.Abstractions;

namespace BandSearch.Web.Mappers
{
    [Mapper]
    public partial class UserDetailsMapper
    {
        public partial UserDetailsDTO UserDetailsToUserDetailtsDto(UserDetails userDetails);
    }
}
