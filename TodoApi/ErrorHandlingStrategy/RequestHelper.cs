﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApi.ErrorHandlingStrategy
{
    public static class RequestHelper
    {
        public static string GetIp(HttpContext context)
        {
            IHttpConnectionFeature connection = context.Features.Get<IHttpConnectionFeature>();

            return connection?.RemoteIpAddress?.ToString();
        }

        public static async Task<string> GetBody(HttpContext context)
        {
            string cachedRequestBody = (string)context.Items["RequestBody"];

            if (cachedRequestBody != null)
                return cachedRequestBody;

            var request = context.Request;

            request.EnableRewind();

            byte[] buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            string requestBody = Encoding.UTF8.GetString(buffer);

            request.Body.Seek(0, SeekOrigin.Begin);

            context.Items["RequestBody"] = requestBody;

            return requestBody;
        }

        public static string GetUser(HttpRequest request)
        {
            return request.Headers.TryGetValue("User", out StringValues userHeader) ? userHeader.FirstOrDefault().ToString() : "User header unavailable";
        }
    }
}