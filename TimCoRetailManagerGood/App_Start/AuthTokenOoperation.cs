using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace TimCoRetailManagerGood.App_Start
{
    public class AuthTokenOperation : IDocumentFilter
    {
        //https://stackoverflow.com/questions/51117655/how-to-use-swagger-in-asp-net-webapi-2-0-with-token-based-authentication
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            swaggerDoc.paths.Add("/token", new PathItem  //add a new route
            {
                post = new Operation //a POST command
                {
                    tags = new List<string> { "Auth" },  //in the Auth category
                    consumes = new List<string>
                    {
                        "application/x-www-form-urlencoded"  //how the paramters should come through
                    },
                    parameters = new List<Parameter>  //definition of the three parameters
                    {
                        //there will be a login box for each paramter
                        new Parameter
                        {
                            type = "string",
                            name = "grant_type",
                            required = true,
                            @in = "formData", //should always be "password"
                            @default = "password"
                        },  //paramter passed to the token method
                        new Parameter
                        {
                            type = "string",
                            name = "username",
                            required = false,
                            @in = "formData"
                        },
                        new Parameter
                        {
                            type = "string",
                            name = "password",
                            required = false,
                            @in = "formData"
                        }
                    }
                }
            });  //for login screen
        }
    }
}