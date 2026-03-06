using System.Globalization;
using System.Text;
using Repositories.Interfaces;

namespace Services;

public class ParserB3CotHist : IParserB3CotHist
{
    private readonly ITickerRepository _tickerRepository;
    private readonly string _baseFolder = "cotacoes";

    public ParserB3CotHist(ITickerRepository tickerRepository)
    {
        _tickerRepository = tickerRepository;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public async Task ParseAndSyncDatabaseAsync()
    {
        string currentDir = Directory.GetCurrentDirectory();
        string baseFolder = Path.GetFullPath(Path.Combine(currentDir, "..", _baseFolder));

        if (!Directory.Exists(baseFolder))
            throw new CustomException("COTACAO_NAO_ENCONTRADA");

        var files = Directory.GetFiles(baseFolder, "COTAHIST_*.TXT");

        foreach (var filePath in files)
        {
            await ProcessFileAsync(filePath);
        }
    }

    private async Task ProcessFileAsync(string filePath)
    {
        var encoding = Encoding.GetEncoding("ISO-8859-1");
        var lines = File.ReadLines(filePath, encoding).ToList();

        var dailyCaches = new Dictionary<DateTime, Dictionary<string, Ticker>>();
        var newTickersToInsert = new List<Ticker>();

        foreach (var line in lines)
        {
            if (line.Length < 245 || !line.StartsWith("01"))
                continue;

            if (!DateTime.TryParseExact(line.Substring(2, 8), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime rowDate))
                continue;

            if (!dailyCaches.TryGetValue(rowDate, out var existingTickersDict))
            {
                existingTickersDict = await _tickerRepository.GetTickersByDateDictAsync(rowDate);
                dailyCaches[rowDate] = existingTickersDict;
            }

            var symbol = line.Substring(12, 12).Trim().ToUpperInvariant();
            var closingPrice = ParsePrice(line.Substring(108, 13));

            if (!existingTickersDict.ContainsKey(symbol))
            {
                var newEntry = new Ticker(symbol, closingPrice, rowDate);
                newTickersToInsert.Add(newEntry);
                existingTickersDict.Add(symbol, newEntry); 
            }
        }

        if (newTickersToInsert.Any())
        {
            await _tickerRepository.AddRangeAsync(newTickersToInsert);
            await _tickerRepository.SaveChangesAsync();
        }
    }

    private static decimal ParsePrice(string value) =>
        long.TryParse(value?.Trim(), out var result) 
            ? decimal.Divide(result, 100) 
            : decimal.Zero;
}