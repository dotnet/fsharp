using System.Text.Json;
using ReproEvidence;

try
{
    object result = args.Length >= 2 ? args[0] switch
    {
        "binlog" when args.Length == 2 => BinlogEvidence.Read(args[1]),
        "inspect" => args.Skip(1).Select(AssetEvidence.Inspect).ToArray(),
        _ => throw new ArgumentException("Usage: Evidence binlog <build.binlog> | inspect <file...>")
    } : throw new ArgumentException("Usage: Evidence binlog <build.binlog> | inspect <file...>");
    Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    }));
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine(JsonSerializer.Serialize(new
    {
        schemaVersion = 1,
        complete = false,
        diagnostics = new[] { $"{ex.GetType().Name}: {ex.Message}" }
    }));
    return 1;
}
