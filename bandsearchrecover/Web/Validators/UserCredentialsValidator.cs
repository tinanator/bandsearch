// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BandSearch.Web.DTOs;
using BandSearch.Web.Models;
using FluentValidation;

namespace BandSearch.Web.Validators
{
    public class UserCredentialsValidator : AbstractValidator<UserCredentials>
    {
        public UserCredentialsValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).NotNull().MinimumLength(8);
        }
    }
}
