using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace TimCoRetailManagerGood.App_Start
{
    public class AuthorizationOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            //add parameter to every operation
            if (operation.parameters == null) //some methods don't have parameters
            {
                operation.parameters = new List<Parameter>();
            }
            //now there will always be a parameter
            operation.parameters.Add(new Parameter
            {
                name = "Authorization",
                @in = "header",
                description = "access token",
                required = false,  //some methods don't need the token
                type = "string"
            });
        }
    }
}