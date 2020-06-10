using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace GotIt.Common.Enums
{
    public enum EStateusCode : Int32
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
        [Description("Repeated data")]
        RepeatedData = 9,
        [Description("Database case an error")]
        DatabaseError = 10,
        [Description("Faild to generate token")]
        GenerateTokenFaild = 11,
        [Description("User not exist or not active")]
        UserNotExist = 12,
        [Description("This Data is Duplicated")]
        DuplicateData = 13,
        [Description("Username already exists")]
        UserNameExists = 14,
        [Description("Email already exists")]
        EmailExists = 15,
        [Description("Username & Email already exists")]
        UserNameEmailExists = 16,
        [Description("Stream Key isn't valid or live doesn't started yet")]
        InValidStreamKey = 17,
    }
}
