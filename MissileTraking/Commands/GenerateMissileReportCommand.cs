using System.Net.Sockets;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MissileTracking.Database;
using MissileTracking.Models;
using MissileTracking.Services;

namespace MissileTracking.Commands
{
    public class GenerateMissileReportCommand : ICommand
    {
        public async Task ExecuteAsync(string request, NetworkStream stream, Func<MissileDbContext> dbContextProvider)
        {
            try
            {
                Console.WriteLine("Generating missile report...");

                await using (var context = dbContextProvider())
                {
                    var missiles = context.Missiles.ToList();

                    // Ensure a valid file path
                    var filePath = Path.Combine(AppContext.BaseDirectory, "MissileReport.pdf");

                    // Delete existing file to avoid write errors
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    Console.WriteLine($"Saving PDF to: {filePath}");

                    var projectRoot = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.FullName;
                    var backgroundPath = Path.Combine(projectRoot, "Assets", "irondome2.jpg");
                    
                    using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (var doc = new Document(PageSize.A4))
                    {
                        var writer = PdfWriter.GetInstance(doc, fs);
                        doc.Open();
                        Console.WriteLine("PDF document created.");

                        // Set full-page background image
                        if (File.Exists(backgroundPath))
                            AddBackGroundImage(writer, backgroundPath);
                        else
                            Console.WriteLine($"[Warning] Background image not found at: {backgroundPath}");

                        // Add title & date
                        CreateTitleRow(doc);

                        // Add header row
                        PdfPTable table = CreateHeaderRow();
                        foreach (var missile in missiles)
                        {
                            GenerateNewRow(missile, table);
                        }
                        doc.Add(table);
                        doc.Close();
                        writer.Close();
                        Console.WriteLine("Missile report generated successfully.");
                    }

                    // Send confirmation to client
                    await TcpConnectionService.SendResponseAsync(stream, $"Report generated successfully: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating report: {ex.Message}");
                await TcpConnectionService.SendResponseAsync(stream, $"Error generating report: {ex.Message}");
            }
        }

        private void GenerateNewRow(MissileInfo missile, PdfPTable table)
        {
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);

            var rowColor = missile.InterceptSuccess
                    ? new BaseColor(144, 238, 144) // Light green for success
                    : new BaseColor(255, 102, 102); // Red for failure

            var idCell = new PdfPCell(new Phrase(missile.Id.ToString(), normalFont))
            {
                BackgroundColor = rowColor,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            table.AddCell(idCell);

            var cityCell = new PdfPCell(new Phrase(missile.HitLocation, normalFont))
            {
                BackgroundColor = rowColor,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            table.AddCell(cityCell);

            var statusCell = new PdfPCell(new Phrase(missile.InterceptSuccess ? "Success" : "Failed", normalFont))
            {
                BackgroundColor = rowColor,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            table.AddCell(statusCell);
        }

        private PdfPTable CreateHeaderRow()
        {
            var table = new PdfPTable(3) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 1, 3, 2 });

            var tableHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.WHITE);

            var cell = new PdfPCell(new Phrase("Missile ID", tableHeaderFont))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("City", tableHeaderFont))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Interception Status", tableHeaderFont))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            table.AddCell(cell);

            return table;
        }

        private void CreateTitleRow(Document doc)
        {
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24, BaseColor.BLACK);
            var title = new Paragraph("Missile Interception Report", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20f
            };
            doc.Add(title);

            var dateFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.BLACK);
            var date = new Paragraph($"Generated on: {DateTime.Now}", dateFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 10f
            };
            doc.Add(date);
        }

        private void AddBackGroundImage(PdfWriter writer,string backgroundPath)
        {
            var bgImage = Image.GetInstance(backgroundPath);
            bgImage.ScaleToFit(PageSize.A4.Width, PageSize.A4.Height);
            bgImage.SetAbsolutePosition(0, 0);

            var canvas = writer.DirectContentUnder;
            canvas.AddImage(bgImage);
        }
    }
}
