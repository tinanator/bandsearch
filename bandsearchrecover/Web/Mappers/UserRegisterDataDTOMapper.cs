// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BandSearch.Models;
using BandSearch.Web.DTOs;
using Riok.Mapperly.Abstractions;

namespace BandSearch.Web.Mappers
{
    [Mapper]
    public partial class UserRegisterDataDTOMapper
    {
        public partial User UserRegisterDataToUser(UserRegisterDataDTO userRegisterDataDTO);
    }
}
