// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using FnHttp;
using LanguageExt.Common;

Console.WriteLine("Hello, World!");

var httpClient = new HttpClient();

var fnClient = new FnHttpClient(httpClient, null);

var uri = new Uri("http://localhost:5043/WeatherForecast");

var response1 = await fnClient.Get<List<WeatherForecast>>(uri);

//var a = response1.Match(list => { }, f1=> { }, f2=> { }, f3=> { });
var b = response1.IsT0? response1.AsT0 : null;

var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, uri));
var c = await response.Content.ReadAsByteArrayAsync();

//response.IfSucc(x => Console.Write(x.RawBytes.ToArray().Length));

Console.ReadKey();



public class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);

    public string? Summary { get; set; }
}