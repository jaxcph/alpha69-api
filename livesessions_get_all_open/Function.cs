using System;
using alpha69.common;
using alpha69.common.dto;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace livesessions_get_all_open
{
    public class Function
    {
        /// <summary>
        ///     A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Response FunctionHandler(Request input, ILambdaContext context)
        {
            var dba = new DBAccess();

            if (input.Body.IsPing)
                return new Response {StatusCode = 200, Message = dba.Test()};

            try
            {
                var list = LiveSession.LoadOpenAll(dba.Connection);


                var r = new Response
                {
                    StatusCode = 200,
                    Message = "ok",
                    Body = new ResponseBody
                    {
                        LiveSessions = list.ToArray(),
                        Count = list.Count
                    }
                };
                return r;
            }
            catch (Exception e)
            {
                var r = new Response();
                r.SetError(400, e);
                return r;
            }
        }
    }
}