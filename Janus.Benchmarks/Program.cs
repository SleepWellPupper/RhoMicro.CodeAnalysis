// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography.X509Certificates;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run(typeof(Program).Assembly);
