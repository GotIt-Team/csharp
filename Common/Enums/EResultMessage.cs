using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.Common.Enums
{
    public enum EResultMessage
    {
        [Description("Process Done Successfully")]
        ProcessSuccess = 1,
        [Description("Process Faild")]
        ProcessFailed = 2,
        [Description("Unauthorized")]
        Unauthorized = 3,
        [Description("Forbidden")]
        Forbidden = 4,
        [Description("Not Found")]
        NotFound = 5,
        [Description("Internal server error")]
        InternalServerError = 6,
        [Description("Missed data")]
        MissedData = 7,
        [Description("Invalid data")]
        InvalidData = 8,
        [Description("Database case an error")]
        DatabaseError = 10,
        [Description("Faild to generate token")]
        GenerateTokenFaild = 11,
        [Description("This Data is Duplicated")]
        DuplicateData = 13,
        [Description("Email already exists")]
        EmailExists = 14,
        [Description("Email or password is wrong")]
        EmailOrPasswordWrong = 15,
        [Description("User account not confirmed")]
        UserNotConfirmed = 16,
        [Description("Password and repeated password not matched")]
        PasswordNotMatched = 16,
        [Description("User type is wrong")]
        NotUserType = 16,
    }
}
