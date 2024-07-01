using HelloEndpoint.Models;
using IPGeolocation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HelloEndpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {

        [HttpGet("hello")]
        public async Task<IActionResult> GetHello(string visitor_name = "Mark")
        {
            var clientIp = HttpContext.Connection.RemoteIpAddress.ToString();
            var location = await GetLocationAsync(clientIp);
            var temperature = await GetTemperatureAsync(location);

            var response = new Hello
            {
                ClientIP = clientIp,
                Location = location,
                Greetings = $"Hello, {visitor_name}!, the temperature is {temperature} in {location}"
            };

            return Ok(response);
        }

        private async Task<string> GetLocationAsync(string ip)
        {
            if (ip == "::1" || ip == "127.0.0.1")
            {
                return "London";
            }

            try
            {
                var url = $"http://ip-api.com/json/{ip}";
                var client = new HttpClient();
                var response = await client.GetStringAsync(url);

                var json = JObject.Parse(response);

                
                if (json["status"].ToString().ToLower() == "success")
                {
                    
                    if (json["city"] != null)
                    {
                        return json["city"].ToString();
                    }
                    else
                    {
                        return "Unknown";
                    }
                }
                else
                {
                    
                    string message = json["message"].ToString();
                    Console.WriteLine($"IP geolocation API returned status: fail. Message: {message}");
                    return "Unknown";
                }
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error in GetLocationAsync: {ex.Message}");
                return "Unknown"; 
            }
        }


        /*        private async Task<string> GetLocationAsync(string ip)
                {
                    // Use an IP geolocation service 

                    *//*            var url = $"https://freegeoip.app/json/{ip}";
                                var client = new HttpClient();
                                var response = await client.GetStringAsync(url);
                                var json = JObject.Parse(response);
                                return json["city"].ToString();*//*
                    var url = $"http://ip-api.com/json/{ip}";
                    var client = new HttpClient();
                    var response = await client.GetStringAsync(url);
                    var json = JObject.Parse(response);
                    return json["city"].ToString();
                }*/


        private async Task<string> GetTemperatureAsync(string city)
        {
            var apiKey = "17e08ebaa1abcc538e4ff9b81cb6c5c6"; 
            var url = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";
            var client = new HttpClient();
            var response = await client.GetStringAsync(url);
            var json = JObject.Parse(response);
            var temp = json["main"]["temp"].ToString();
            return $"{temp} degrees Celsius";
        }
    }
}
