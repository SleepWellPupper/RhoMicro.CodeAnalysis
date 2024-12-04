namespace RhoMicro.CodeAnalysis.JsonSchemaGenerator.Cli;
using System;

sealed class Settings
{
    public required String AssemblyPath { get; set; }
    public required String SchemataPath { get; set; }
}