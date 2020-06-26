using GotIt.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GotIt.Common.Helper
{
    public class Result<TData>
    {
        public Result(TData data, int? count, string message, bool isSucceeded)
        {
            Data = data;
            Count = count;
            Message = message;
            IsSucceeded = isSucceeded;
        }

        public TData Data { get; private set; }
        public int? Count { get; private set; }
        public bool IsSucceeded { get; private set; }
        public string Message { get; private set; }
    }

    public class PageResult<TData>
    {
        public TData Data { get; set; }
        public int Count { get; set; }
    }

    static class ResultHelper
    {
        public static Result<TData> Succeeded<TData>(TData data = default, int? count = null, string message = "")
        {
            if (string.IsNullOrWhiteSpace(message)) message = EResultMessage.ProcessSuccess.ToString();
            return new Result<TData>(data, count, message, true);
        }

        public static Result<TData> Failed<TData>(TData data = default, int? count = null, string message = "")
        {
            if (string.IsNullOrWhiteSpace(message)) message = EResultMessage.ProcessFailed.ToString();
            return new Result<TData>(data, count, message, false);
        }
    }
}
