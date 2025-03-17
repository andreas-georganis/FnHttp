using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using BenchmarkDotNet.Attributes;

namespace FnHttp.Benchmarks;

[MemoryDiagnoser]
public class Benchmarks
{
    private  FnHttpClient _fnHttpClient;
    private  HttpClient _httpClient;

    private  Uri _uri;
    
    [GlobalSetup]
    public void SetupBenchmarks()
    {
        _uri = new Uri("http://localhost:5043/WeatherForecast");
        _httpClient = new HttpClient();
        _fnHttpClient = new FnHttpClient(_httpClient, null);
    }

    [Benchmark]
    public async Task GetWeatherForecastUsingPlainHttpClient()
    {
        await _httpClient.GetFromJsonAsync<List<WeatherForecast>>(_uri);
    }
    
    [Benchmark]
    public async Task GetWeatherForecastUsingPlainHttpClientWithString()
    {
        var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get,  _uri));

        var content = await response.Content.ReadAsStringAsync();
        var rawBytes = Encoding.UTF8.GetBytes(content);
        var payload = JsonSerializer.Deserialize<List<WeatherForecast>>(content);
    }
    
    [Benchmark]
    public async Task GetWeatherForecastUsingFnHttpClient()
    {
        await _fnHttpClient.Get<List<WeatherForecast>>( _uri);
    }
}

public class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);

    public string? Summary { get; set; }
}