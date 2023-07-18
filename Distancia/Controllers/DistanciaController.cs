using Microsoft.AspNetCore.Mvc;

namespace Distancia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistanciaController : ControllerBase
    {
        private readonly GoogleMapsApi _googleMapsApi;

        public DistanciaController()
        {
            _googleMapsApi = new GoogleMapsApi();
        }

        [HttpGet("distancia")]
        public async Task<ActionResult> CalculateDistances()
        {
            string Origem = Environment.GetCommandLineArgs()[1];
            string Destino = Environment.GetCommandLineArgs()[2];
            var distance = await _googleMapsApi.CalculateDistance(Origem, Destino);
            
            return Ok(distance);
        }
        public class GoogleMapsApi
        {
            private const string ApiKey = "---SUA CHAVE AQUI---";
            private const string BaseUrl = "https://maps.googleapis.com/maps/api/distancematrix/json";

            public async Task<string> CalculateDistance(string origin, string destination)
            {
                using (var httpClient = new HttpClient())
                {
                    var requestUri = $"{BaseUrl}?origins={origin}&destinations={destination}&key={ApiKey}";

                    var response = await httpClient.GetAsync(requestUri);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();

                        dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(content);
                        if (data.rows[0].elements[0].status == "NOT_FOUND")
                        {
                            return "Não Foi Possivel Calcular a Distancia";
                        }
                        var distanceText = data.rows[0].elements[0].distance.text;

                        string caminhoArquivo = "---CAMINHO PARA SALVAR O ARQUIVO---";
                        using (StreamWriter arquivo = new StreamWriter(caminhoArquivo))
                        {
                            arquivo.WriteLine(distanceText);
                        }

                        return distanceText;
                    }
                    return "Não Foi Possivel Calcular a Distancia";
                }
            }
            
        }
    }
}
