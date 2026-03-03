using System.Globalization;
using System.Text;
using Repositories.Interfaces;

namespace Services;

public class ParserB3CotHist : IParserB3CotHist
{
    private readonly ITickerRepository _tickerRepository;
    private readonly string _baseFolder = "cotacao";

    public ParserB3CotHist(ITickerRepository tickerRepository)
    {
        _tickerRepository = tickerRepository;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public async Task ParseAndSyncDatabaseAsync(DateTime referenceDate)
    {
        string currentDir = Directory.GetCurrentDirectory();

        string baseFolder = Path.GetFullPath(Path.Combine(currentDir, "..", "cotacoes"));

        string fileName = $"COTAHIST_D{referenceDate:ddMMyyyy}.TXT";

        string filePath = Path.Combine(baseFolder, fileName);

        if (!File.Exists(filePath))
                throw new CustomException("ARQUIVO_COTAHIST_NAO_ENCONTRADO");
        var encoding = Encoding.GetEncoding("ISO-8859-1");
        var lines = File.ReadLines(filePath, encoding).ToList();

        var firstDataLine = lines.FirstOrDefault(l => l.StartsWith("01"));
        if (firstDataLine == null) return;

        var fileDate = DateTime.ParseExact(firstDataLine.Substring(2, 8), "yyyyMMdd", CultureInfo.InvariantCulture);

        var existingTickersDict = await _tickerRepository.GetTickersByDateDictAsync(fileDate);

        var newTickersToInsert = new List<Ticker>();

        foreach (var line in lines)
        {
            if (line.Length < 245 || !line.StartsWith("01"))
                continue;

            var symbol = line.Substring(12, 12).Trim().ToUpperInvariant();
            var closingPrice = ParsePrice(line.Substring(108, 13));

            if (!existingTickersDict.ContainsKey(symbol))
            {
                var newEntry = new Ticker(symbol, closingPrice, fileDate);
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