using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace GuardRail.Api;

public class ApiResult
    : ActionResult,
        IConvertToActionResult,
        IStatusCodeActionResult
{
    public ApiResult(
        int statusCode)
    {
        StatusCode = statusCode;
    }

    public IActionResult Convert() =>
        new ActionResult<object>(
                null)
            .Result;

    public int? StatusCode { get; }
}

public sealed class ApiResult<T>
    : ApiResult
{
    private readonly T? _item;

    public ApiResult(
        T? item,
        int statusCode)
        : base(
            statusCode)
    {
        _item = item;
    }
}