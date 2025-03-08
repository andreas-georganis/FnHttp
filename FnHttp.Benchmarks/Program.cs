// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using FnHttp.Benchmarks;



Console.WriteLine("Hello, World!");
var summary = BenchmarkRunner.Run<Benchmarks>();


