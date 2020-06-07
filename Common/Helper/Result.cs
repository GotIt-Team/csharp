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
        public static Result<TData> Succeeded(TData data, int? count = null, string message = "Process done successfuly")
        {
            return new Result<TData>
            {
                IsSucceeded = true,
                Message = message,
                Data = data,
                Count = count
            };
        }

        public static Result<TData> Failed(TData data, int? count = null, string message = "Process Failed!")
        {
            return new Result<TData>
            {
                IsSucceeded = false,
                Message = message,
                Data = data,
                Count = count
            };
        }

        public TData Data { get; set; }
        public int? Count { get; set; }
        public bool IsSucceeded { get; set; }
        public string Message { get; set; }
    }
}
