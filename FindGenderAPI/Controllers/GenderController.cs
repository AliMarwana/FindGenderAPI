using FindGenderAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;

namespace FindGenderAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class GenderController : ControllerBase
    {


        [HttpGet("classify")]
        public async Task<IActionResult> GetGender([FromQuery] string name)
        {
            try
            {
                if (String.IsNullOrEmpty(name))
                {
                    return BadRequest(new ErrorDto { Status = "error", Message = "Missing or empty name parameter" });
                }
                else if (name.GetType() != typeof(string))
                {
                    return UnprocessableEntity(new ErrorDto
                    {
                        Status = "error",
                        Message = "name is not a string",
                    

                    });
                }
                else
                {
                    using var client = new HttpClient();
                    string url = $"https://api.genderize.io/?name={name}";
                    // Gets the response body as a string directly
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        JObject obj = JObject.Parse(responseBody);

                        var responseApi = new ResponseDto
                        {
                            Status = "success",
                            Data = new DataDto
                            {
                                Name = name,
                                Gender = obj["gender"].ToString(),
                                Probability = float.Parse(obj["probability"].ToString()),
                                SampleSize = int.Parse(obj["count"].ToString()),
                                ProcessedAt = DateTime.UtcNow,
                            }
                        };
                        if(responseApi.Data.SampleSize == null ||  responseApi.Data.SampleSize < 0)
                            {
                            return BadRequest(new ErrorDto
                            {
                                Status = "error",
                                Message = "No prediction available for the provided name",
                            });
                        }
                        responseApi.Data.IsConfident = responseApi.Data.Probability >= 0.7f && responseApi.Data.SampleSize >= 100;

                        return Ok(responseApi);
                    }
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
                
            return Ok();
        }

   
    }
}
