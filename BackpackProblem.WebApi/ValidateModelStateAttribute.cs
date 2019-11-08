using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Linq;

namespace BackpackProblem.WebApi
{
    public class ValidateModelStateAttribute : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            base.OnResultExecuting(context);

            if (!context.ModelState.IsValid)
            {
                context.Result = new JsonResult(new
                {
                    Status = "Model State Validation Error",
                    Errors = context.ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                }, new JsonSerializerSettings() { Formatting = Formatting.Indented });
            }
        }
    }
}
