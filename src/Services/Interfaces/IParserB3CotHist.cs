namespace Services;

public interface IParserB3CotHist
{
    Task ParseAndSyncDatabaseAsync(DateTime referenceDate);
}