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

        context.Result = new JsonResult(new
        {
           errors = new
           {
               Error = new[] { errorMessage }
           }
        },
        new System.Text.Json.JsonSerializerOptions
        {
           PropertyNamingPolicy = null
        })
        {
           StatusCode = 400
        };

        context.ExceptionHandled = true;
    }
}