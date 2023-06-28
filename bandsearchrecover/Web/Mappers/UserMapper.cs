using BandSearch.Models;
using BandSearch.Web.DTOs;
using Riok.Mapperly.Abstractions;

namespace BandSearch.Web.Mappers
{
    [Mapper]
    public partial class UserMapper
    {
        public partial UserDTO UserToUserDto(User user);
        public partial User UserDTOToUser(UserDTO userDTO);
        public partial User UpdateUserDTOToUser(UpdateUserDTO userDTO);
        public partial BandMemberDTO UserToMemberDTO(User member);
    }
}
