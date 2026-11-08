using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.SS.Formula.Functions;
using RaceDay.Models;
using RaceDay.Models.Enrolments;
using RaceDay.Models.Results;

namespace RaceDay.Models
{
    // WRAPPER MODEL: every service method returns one of these instead of throwing errors.
    // WHY: the controller can check Success and show ErrorMessage, and can tell a
    // 401 (login problem) apart from a 500 (server problem) using StatusCode.
    // <T> is a "generic": ApiResult<EventViewModel> holds an event, ApiResult<List<...>> holds a list.

    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }                       // the data on success (null on failure)
        public string ErrorMessage { get; set; } = string.Empty; // friendly text for the user
        public int StatusCode { get; set; }                // 200, 201, 400, 401, 403, 404...

        // Helper methods so services can write ApiResult<T>.Ok(data) instead of building the object by hand.
        public static ApiResult<T> Ok(T data, int statusCode = 200) =>
            new() { Success = true, Data = data, StatusCode = statusCode };

        public static ApiResult<T> Fail(string message, int statusCode) =>
            new() { Success = false, ErrorMessage = message, StatusCode = statusCode };
    }

   
}
