using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleChat.Core
{
    public static class APIStatusCode
    {
        /* //-SAMPLE-//

          /// <summary>
          /// Desc
          /// </summary>
          public const string ERR01001 = "ERR01001";

        */

        /*
        -----------------------------------------------
        ERR - Error Codes
            01 - General
            02 - Identity Errors
            03 - ModelState Errors
        -----------------------------------------------
        */

        #region ERR01 - General Errors

        /// <summary>
        /// General Error Code
        /// </summary>
        public const string ERR01001 = "ERR01001";

        /// <summary>
        /// Record not found on this ID
        /// </summary>
        public const string ERR01002 = "ERR01002";

        /// <summary>
        /// Invalid parameter Or parameters
        /// </summary>
        public const string ERR01003 = "ERR01003";

        /// <summary>
        /// User not found
        /// </summary>
        public const string ERR01004 = "ERR01004";

        /// <summary>
        /// Login failed
        /// </summary>
        public const string ERR01005 = "ERR01005";

        /// <summary>
        /// Main group can't duplicate or delete
        /// </summary>
        public const string ERR01006 = "ERR01006";

        /// <summary>
        ///  Not owner of the record
        /// </summary>
        public const string ERR01007 = "ERR01007";

        /// <summary>
        ///  The record couldnt add to DB
        /// </summary>
        public const string ERR01008 = "ERR01008";

        /// <summary>
        ///  You cannot get all chat room records whitout a administrator role
        /// </summary>
        public const string ERR01009 = "ERR01009";

        /// <summary>
        ///  User couldnt update
        /// </summary>
        public const string ERR01010 = "ERR01010";


        #endregion

        #region ERR02 - Identity Errors

        /// <summary>
        /// Duplicate Email
        /// </summary>
        public const string ERR02001 = "ERR02001";

        /// <summary>
        /// Duplicate UserName
        /// </summary>
        public const string ERR02002 = "ERR02002";

        /// <summary>
        /// Invalid Email
        /// </summary>
        public const string ERR02003 = "ERR02003";

        /// <summary>
        /// Duplicate Role Name
        /// </summary>
        public const string ERR02004 = "ERR02004";

        /// <summary>
        /// Invalid Role Name
        /// </summary>
        public const string ERR02005 = "ERR02005";

        /// <summary>
        /// Invalid Token
        /// </summary>
        public const string ERR02006 = "ERR02006";

        /// <summary>
        /// Invalid UserName
        /// </summary>
        public const string ERR02007 = "ERR02007";

        /// <summary>
        /// Login Already Associated
        /// </summary>
        public const string ERR02008 = "ERR02008";

        /// <summary>
        /// Password Mismatch
        /// </summary>
        public const string ERR02009 = "ERR02009";

        /// <summary>
        /// Pasword Requires Digit
        /// </summary>
        public const string ERR02010 = "ERR02010";

        /// <summary>
        /// Pasword Requires Lower Charecter
        /// </summary>
        public const string ERR02011 = "ERR02011";

        /// <summary>
        /// Pasword Requires Non Alphanumeric Character
        /// </summary>
        public const string ERR02012 = "ERR02012";

        /// <summary>
        /// Pasword Requires Unique Characters
        /// </summary>
        public const string ERR02013 = "ERR02013";

        /// <summary>
        /// Pasword Requires Upper Characters
        /// </summary>
        public const string ERR02014 = "ERR02014";

        /// <summary>
        /// Pasword Too Short
        /// </summary>
        public const string ERR02015 = "ERR02015";

        /// <summary>
        /// User Already Has Password
        /// </summary>
        public const string ERR02016 = "ERR02016";

        /// <summary>
        /// User Already In Role
        /// </summary>
        public const string ERR02017 = "ERR02017";

        /// <summary>
        /// User Lockout Not Enabled
        /// </summary>
        public const string ERR02018 = "ERR02018";

        /// <summary>
        /// Recovery Code Redemption Failed
        /// </summary>
        public const string ERR02019 = "ERR02019";

        /// <summary>
        /// Concurrency Failure
        /// </summary>
        public const string ERR02020 = "ERR02020";

        /// <summary>
        /// Default Identity Error
        /// </summary>
        public const string ERR02021 = "ERR02021";

        /// <summary>
        /// User Not In Role
        /// </summary>
        public const string ERR02022 = "ERR02022";

        /// <summary>
        /// User dont have valid refresh token
        /// </summary>
        public const string ERR02023 = "ERR02023";

        /// <summary>
        /// Refresh tokens dont match
        /// </summary>
        public const string ERR02024 = "ERR02024";

        /// <summary>
        /// User already have a valid token
        /// </summary>
        public const string ERR02025 = "ERR02025";

        /// <summary>
        /// The token is not matching with active/expired tokens
        /// </summary>
        public const string ERR02026 = "ERR02026";

        #endregion

        #region ERR03 - ModelState Errors

        /// <summary>
        ///  Field Required
        /// </summary>
        public const string ERR03001 = "ERR03001";

        /// <summary>
        ///  Min Length Error
        /// </summary>
        public const string ERR03002 = "ERR03002";

        /// <summary>
        ///  Max Length Error
        /// </summary>
        public const string ERR03003 = "ERR03003";

        /// <summary>
        ///  Passwords Is Not Match
        /// </summary>
        public const string ERR03004 = "ERR03004";

        #endregion
    }
}
