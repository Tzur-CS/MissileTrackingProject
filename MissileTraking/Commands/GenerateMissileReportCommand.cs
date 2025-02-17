using System.Net.Sockets;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MissileTracking.Database;
using MissileTracking.Models;

namespace MissileTracking.Commands
{
    public class GenerateMissileReportCommand : ICommand
    {
        public async Task ExecuteAsync(string request, NetworkStream stream, Func<MissileDbContext> dbContextProvider)
        {
            try
            {
                Console.WriteLine("Generating missile report...");

                using (var context = dbContextProvider())
                {
                    var missiles = context.Missiles.ToList();

                    // Ensure a valid file path
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "MissileReport.pdf");

                    // Delete existing file to avoid write errors
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    Console.WriteLine(Directory.GetCurrentDirectory());

                    string backgroundPath =
                        Path.Combine("C:/Users/tzur4/Documents/C#_learning/ProjectMT/MissileTracking",
                            "irondome2.jpg"); // Change to your image

                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (Document doc = new Document(PageSize.A4))
                    {
                        PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                        doc.Open();
                        Console.WriteLine("PDF document created.");

                        // Set full-page background image
                        if (File.Exists(backgroundPath))
                        {
                            Image bgImage = Image.GetInstance(backgroundPath);
                            bgImage.ScaleToFit(PageSize.A4.Width, PageSize.A4.Height);
                            bgImage.SetAbsolutePosition(0, 0);
                        
                            PdfContentByte canvas = writer.DirectContentUnder;
                            canvas.AddImage(bgImage);
                        }

                        // Add title & date
                        CreateTitleRow(doc);

                        // Add header row (larger text for better visibility)
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
                    await SendResponseAsync(stream, $"Report generated successfully: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating report: {ex.Message}");
                await SendResponseAsync(stream, $"Error generating report: {ex.Message}");
            }
        }

        private async Task SendResponseAsync(NetworkStream stream, string message)
        {
            byte[] responseData = System.Text.Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(responseData, 0, responseData.Length);
        }

        private void GenerateNewRow(MissileInfo missile, PdfPTable table)
        {
            Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);

            BaseColor rowColor =
                missile.InterceptSuccess
                    ? new BaseColor(144, 238, 144)
                    : new BaseColor(255, 102, 102); // Green if success, red if fail

            PdfPCell idCell = new PdfPCell(new Phrase(missile.Id.ToString(), normalFont));
            idCell.BackgroundColor = rowColor;
            idCell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(idCell);

            PdfPCell cityCell = new PdfPCell(new Phrase(missile.HitLocation, normalFont));
            cityCell.BackgroundColor = rowColor;
            cityCell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cityCell);

            PdfPCell statusCell = new PdfPCell(new Phrase(missile.InterceptSuccess ? "Success" : "Failed", normalFont));
            statusCell.BackgroundColor = rowColor;
            statusCell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(statusCell);
        }

        private PdfPTable CreateHeaderRow()
        {
            
            // Create table
            PdfPTable table = new PdfPTable(3); // 3 columns: ID, City, Status
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 1, 3, 2 });
            
            Font tableHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.WHITE);
            PdfPCell cell = new PdfPCell(new Phrase("Missile ID", tableHeaderFont));
            cell.BackgroundColor = BaseColor.DARK_GRAY;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("City", tableHeaderFont));
            cell.BackgroundColor = BaseColor.DARK_GRAY;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Interception Status", tableHeaderFont));
            cell.BackgroundColor = BaseColor.DARK_GRAY;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);
            
            return table;
        }

        private void CreateTitleRow(Document doc)
        {
            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24, BaseColor.BLACK);
            Paragraph title = new Paragraph("Missile Interception Report", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            title.SpacingAfter = 20f;
            doc.Add(title);

            // Add generated date (larger size)
            Font dateFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.BLACK);
            Paragraph date = new Paragraph($"Generated on: {DateTime.Now}", dateFont);
            date.Alignment = Element.ALIGN_CENTER;
            date.SpacingAfter = 10f;
            doc.Add(date);
        }
    }
}