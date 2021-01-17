using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace SimpleChat.Core.Validation
{
    public class IdentityErrorDescriberForAPIStatusCodes : IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = "Email",
                Description = APIStatusCode.ERR02001
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = "UserName",
                Description = APIStatusCode.ERR02002
            };
        }

        public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError
            {
                Code = "Email",
                Description = APIStatusCode.ERR02003
            };
        }

        public override IdentityError DuplicateRoleName(string role)
        {
            return new IdentityError
            {
                Code = "RoleName",
                Description = APIStatusCode.ERR02004
            };
        }

        public override IdentityError InvalidRoleName(string role)
        {
            return new IdentityError
            {
                Code = "RoleName",
                Description = APIStatusCode.ERR02005
            };
        }

        public override IdentityError InvalidToken()
        {
            return new IdentityError
            {
                Code = "IdentityGeneral",
                Description = APIStatusCode.ERR02006
            };
        }

        public override IdentityError InvalidUserName(string userName)
        {
            return new IdentityError
            {
                Code = "UserName",
                Description = APIStatusCode.ERR02007
            };
        }

        public override IdentityError LoginAlreadyAssociated()
        {
            return new IdentityError
            {
                Code = "IdentityGeneral",
                Description = APIStatusCode.ERR02008
            };
        }

        public override IdentityError PasswordMismatch()
        {
            return new IdentityError
            {
                Code = "Password",
                Description = APIStatusCode.ERR02009
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = "Password",
                Description = APIStatusCode.ERR02010
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = "Password",
                Description = APIStatusCode.ERR02011
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = "Password",
                Description = APIStatusCode.ERR02012
            };
        }

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return new IdentityError
            {
                Code = "Password",
                Description = APIStatusCode.ERR02013
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = "Password",
                Description = APIStatusCode.ERR02014
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = "Password",
                Description = APIStatusCode.ERR02015
            };
        }

        public override IdentityError UserAlreadyHasPassword()
        {
            return new IdentityError
            {
                Code = "IdentityGeneral",
                Description = APIStatusCode.ERR02016
            };
        }

        public override IdentityError UserAlreadyInRole(string role)
        {
            return new IdentityError
            {
                Code = "IdentityGeneral",
                Description = APIStatusCode.ERR02017
            };
        }

        public override IdentityError UserNotInRole(string role)
        {
            return new IdentityError
            {
                Code = "IdentityGeneral",
                Description = APIStatusCode.ERR02022
            };
        }

        public override IdentityError UserLockoutNotEnabled()
        {
            return new IdentityError
            {
                Code = "IdentityGeneral",
                Description = APIStatusCode.ERR02018
            };
        }

        public override IdentityError RecoveryCodeRedemptionFailed()
        {
            return new IdentityError
            {
                Code = "IdentityGeneral",
                Description = APIStatusCode.ERR02019
            };
        }

        public override IdentityError ConcurrencyFailure()
        {
            return new IdentityError
            {
                Code = "IdentityGeneral",
                Description = APIStatusCode.ERR02020
            };
        }

        public override IdentityError DefaultError()
        {
            return new IdentityError
            {
                Code = "IdentityGeneral",
                Description = APIStatusCode.ERR02021
            };
        }
    }
}
