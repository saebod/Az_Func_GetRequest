using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Az_Func_GetRequest.Models;
using Az_Func_GetRequest.Data;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Az_Func_GetRequest
{
    public class Az_Func_GetRequest
    {
        private readonly ILogger _logger;

        // Base URL
        private static HttpClient sharedClient = new HttpClient()
        {
            BaseAddress = new Uri("https://jsonplaceholder.typicode.com"),
        };

        public Az_Func_GetRequest(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Az_Func_GetRequest>();
        }

        [Function("Az_Func_GetRequest")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            // bruger til at skrive i database
            DataContextEF entityFramework = new DataContextEF();

            // Henter data fra todo url
            string jsonResponse = await GetAsync(sharedClient);

            Todo todoItem = null;
            if (!string.IsNullOrEmpty(jsonResponse))
            {
                todoItem = JsonConvert.DeserializeObject<Todo>(jsonResponse);
            }

            HttpResponseData response;


            // Skriver ned i database
            try
            {
                entityFramework.Add(todoItem);
                entityFramework.SaveChanges();

                response = req.CreateResponse(HttpStatusCode.OK);
                response.WriteString("Todo item has been added successfully.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"A database update error occurred: {ex.Message}");

                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner exception: {ex.InnerException.Message}");
                }

                response = req.CreateResponse(HttpStatusCode.InternalServerError);
                response.WriteString("An error occurred while updating the database.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");

                response = req.CreateResponse(HttpStatusCode.InternalServerError);
                response.WriteString("An unexpected error occurred.");
            }

            return response;
        }

        static async Task<string> GetAsync(HttpClient httpClient)
        {
            using HttpResponseMessage response = await httpClient.GetAsync("todos/");

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return jsonResponse;
        }
    }
}
