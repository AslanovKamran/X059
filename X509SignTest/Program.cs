using System.Security.Cryptography.X509Certificates;
using X509SignTest.Utils;
using System.Xml;

namespace X509SignTest;

class Programm
{
    const string PathToSave = @"C:\Users\DOIT\Desktop\Signed.xml";
    const string PathToFile= @"C:\Users\DOIT\Desktop\Test Xml file.xml";

    static void Main(string[] args)
    {

        //Change Console Output Encoding for letters 
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var selectedCertificate = CertificateUtils.GetCertificateFromStore(StoreName.My ,StoreLocation.CurrentUser);

        if (selectedCertificate == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Сертификат не найден.");
            Console.ResetColor();
            return;
        }

        if (!selectedCertificate.HasPrivateKey)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Выбранный сертификат не содержит закрытого ключа. Подписание невозможно!");
            Console.ResetColor();
            return;
        }

        XmlDocument doc = new();
        try
        {
            // Load the actual XML file from disk
            doc.Load(PathToFile); 
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ошибка чтения XML файла: {ex.Message}");
            Console.ResetColor();
            return;
        }

        try
        {

            // Sign the XML document with the certificate
            CertificateUtils.SignXmlDocumentWithCertificate(doc, selectedCertificate);

            // Save the signed XML document to a file
            File.WriteAllText(PathToSave, doc.OuterXml);

            // Indicate success with a green console message
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"Файл сохранен по пути C:\Users\DOIT\Desktop\Signed.xml");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            // Handle any exceptions with a red console message
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }
    }
}

