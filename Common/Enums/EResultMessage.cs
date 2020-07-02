using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.Common.Enums
{
    public enum EResultMessage
    {
        ProcessSuccess = 1,
        ProcessFailed = 2,
        Unauthorized = 3,
        Forbidden = 4,
        NotFound = 5,
        InternalServerError = 6,
        MissedData = 7,
        InvalidData = 8,
        DatabaseError = 10,
        GenerateTokenFaild = 11,
        DuplicateData = 13,
        EmailExists = 14,
        EmailOrPasswordWrong = 15,
        UserNotConfirmed = 16,
        PasswordNotMatched = 16,
        NotUserType = 16,
        WrongPassword = 17,
        InvalidExtension = 18,
        ExceedMaxContent = 19,
        RegistrationDone = 20,
    }
}
