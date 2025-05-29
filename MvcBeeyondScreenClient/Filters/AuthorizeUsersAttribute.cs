using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MvcBeeyondScreenClient.Filters
{
    public class AuthorizeUsersAttribute : AuthorizeAttribute,
        IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //POR AHORA, SOLAMENTE NOS VA A INTERESAR SI 
            //EXISTE EL EMPLEADO
            var user = context.HttpContext.User;
            if (user.Identity.IsAuthenticated == false)
            {
                context.Result = this.GetRoute("Managed", "Login");
            }
        }

        //TENDREMOS MULTIPLES REDIRECCIONES, POR LO QUE NOS CREAMOS UN 
        //METODO PARA FACILITAR EL CODIGO
        private RedirectToRouteResult GetRoute
            (string controller, string action)
        {
            RouteValueDictionary ruta =
                new RouteValueDictionary(
                    new { controller = controller, action = action }
                    );
            RedirectToRouteResult result =
                new RedirectToRouteResult(ruta);
            return result;
        }

    }
}
