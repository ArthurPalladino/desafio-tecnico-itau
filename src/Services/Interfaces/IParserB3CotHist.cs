namespace Services;

public interface IParserB3CotHist
{
    Task ParseAndSyncDatabaseAsync(string filePath);
}