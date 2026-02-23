using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PLDMS.BL.Common;

namespace PLDMS.PL.Middlewares;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly IModelMetadataProvider _modelMetadataProvider;

    public GlobalExceptionFilter(IModelMetadataProvider modelMetadataProvider)
    {
        _modelMetadataProvider = modelMetadataProvider;
    }

    public void OnException(ExceptionContext context)
    {
        string errorMessage = context.Exception is BaseException ex
            ? ex.Message
            : "Something went wrong";

        var result = new ViewResult
        {
            ViewName = context.RouteData.Values["action"]?.ToString(),
            ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState)
            {
                Model = context.RouteData.Values["dto"]
            }
        };

        result.ViewData.ModelState.AddModelError("Error", errorMessage);

        context.Result = result;
        context.ExceptionHandled = true;
    }
}