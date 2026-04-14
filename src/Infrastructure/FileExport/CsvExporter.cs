using EShop.Application.Contracts.Infrastructure;

namespace EShop.Infrastructure.FileExport
{
    public class CsvExporter : ICsvExporter
    {
        //public byte[] ExportEventsToCsv(List<EventExportDto> allEvents)
        //{
        //    using var memoryStream = new MemoryStream();
        //    using (var streamWriter = new StreamWriter(memoryStream))
        //    {
        //        using var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.CurrentCulture));
        //        csvWriter.WriteRecords(allEvents);
        //    }

        //    return memoryStream.ToArray();
        //}
    }
}
