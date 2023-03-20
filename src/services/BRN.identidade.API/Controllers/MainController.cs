using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BRN.identidade.API.Controllers
{
    [ApiController]

    public abstract class MainController : ControllerBase
    {
        protected ICollection<string> Erros = new List<string>();

        protected ActionResult CustomResponse(object result = null)
        {
           if(ValidOperation())
            {
                return Ok(result);
            }

           return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
           {
               { "Messange", Erros.ToArray() }
           }));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState) // overload
        {
            var erros = modelState.Values.SelectMany(x => x.Errors);
            foreach(var erro in erros)
            {
                AddError(erro.ErrorMessage);
            }

            return CustomResponse();
        }

        protected bool ValidOperation()
        {
            return !Erros.Any();
        }

        protected void AddError(string erro)
        {
            Erros.Add(erro);
        }

        protected void ClearErrors(string erro)
        {
            Erros.Clear();
        }
        
    }
}
