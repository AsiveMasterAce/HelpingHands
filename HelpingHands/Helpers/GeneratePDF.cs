using HelpingHands.Models;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace HelpingHands.Helpers
{
    public class GeneratePDF
    {
       
        public string GeneratePdfContract(IEnumerable<CareContract> model, string webRootPath)
        {

            string fileName = "CareContracts.pdf";
            string filePath = System.IO.Path.Combine(webRootPath, fileName);
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                PdfWriter writer = new PdfWriter(fs);
                PdfDocument pdf = new PdfDocument(writer);
        
                Document document = new Document(pdf, PageSize.A4.Rotate());
                // Add table to the document
                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1, 1, 1, 1, 1}));
                document.Add(table);

                // Add logo to the top left
                string logoPath = System.IO.Path.Combine(webRootPath, "img", "Logo.JPG");
                ImageData imageData = ImageDataFactory.Create(logoPath);
                Image image = new Image(imageData);
                image.ScaleAbsolute(80, 30);
                image.SetHorizontalAlignment(HorizontalAlignment.LEFT);
                image.SetFixedPosition(50, PageSize.A4.GetHeight() - image.GetImageHeight() +10); // 50 is the margin from the top
                document.Add(image);

                // Add title to the center
                Paragraph title = new Paragraph("Care Contract Reports");
                title.SetTextAlignment(TextAlignment.CENTER);
                document.Add(title);
                // Add header row
                table.AddHeaderCell(new Cell().Add(new Paragraph("Contract Date")).SetBold().SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Care Status")).SetBold().SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Wound Condition")).SetBold().SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Patient")).SetBold().SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Nurse")).SetBold().SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Start Date")).SetBold().SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                table.AddHeaderCell(new Cell().Add(new Paragraph("End Date")).SetBold().SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Contract Address")).SetBold().SetTextAlignment(TextAlignment.CENTER).SetBackgroundColor(ColorConstants.LIGHT_GRAY));

                // Add other header cells...

                // Add data rows
                foreach (var item in model)
                {
                    string PfullName =$"{item.Patient?.FirstName} {item.Patient?.LastName}";
                    string NfullName =$"{item.Nurse?.FirstName} {item.Nurse?.LastName}";
                    //string Address =$"{item.Nurse?.FirstName} {item.Nurse?.LastName}";
                    string Address=  $"{item.AddressLine1}, {item.AddressLine2}, {item.Suburb.City.Name}, {item.Suburb.PostalCode}";

                    table.AddCell(new Cell().Add(new Paragraph(item.ContractDate.Value.ToString("dd/MM/yyyy"))));
                    table.AddCell(new Cell().Add(new Paragraph(item.CareStatus)));
                    table.AddCell(new Cell().Add(new Paragraph(item.WoundDescription)));
                    table.AddCell(new Cell().Add(new Paragraph(PfullName)));
                    table.AddCell(new Cell().Add(new Paragraph(NfullName)));
                    table.AddCell(new Cell().Add(new Paragraph(item.StartDate.GetValueOrDefault().ToString("dd/MM/yyyy"))));
                    table.AddCell(new Cell().Add(new Paragraph(item.EndDate.GetValueOrDefault().ToString("dd/MM/yyyy"))));
                    table.AddCell(new Cell().Add(new Paragraph(Address)));


                    // Close the table cells after sadding the paragraphs

                }
                 table.Complete();
                // Add footer
                string footerText = "@HelpingHands: Report Generated generated at: " + DateTime.Now.ToString("dd/MM/yyyy");
                Paragraph footer = new Paragraph(footerText);
                document.ShowTextAligned(footer, 297, 20, 1, TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);
                document.Close();
            }

            return filePath;
        }
    }
}
