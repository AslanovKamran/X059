using System.Security.Cryptography.X509Certificates;
using X059ValidateTest.Utils;
using System.Xml;

namespace X059ValidateTest;

class Program
{
    // Paths to the signed XML file and the public key certificate
    const string TargetFilePath = @"C:\Users\DOIT\Desktop\signed.xml";
    const string PublicKeyPath = @"C:\Users\DOIT\Desktop\doit-test-cert-public.pem";
    static void Main(string[] args)
    {
        //Change Console Output Encoding for letters 
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        if (!File.Exists(TargetFilePath))
        {
            Console.WriteLine("Файла с именем Signed.xml не существует.");
            return;
        }

        // Read the contents of the signed XML file
        string xmlString = File.ReadAllText(TargetFilePath);

        // Load the XML into an XmlDocument object
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlString);

        // Load the public key certificate for validation
        X509Certificate2 pubCert;
        try
        {
            pubCert = new X509Certificate2(PublicKeyPath);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ошибка при загрузке сертификата: {ex.Message}");
            Console.ResetColor();
            return;
        }

        // Validate the XML document's signature using the public key certificate
        Console.WriteLine("Проверка файла Signed.xml на валидность ЭП...\n");

        bool isValid = ValidationUtils.ValidateXmlDocumentWithCertificate(doc, pubCert);

        Console.ForegroundColor = isValid ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine(isValid ? "Валидация прошла успешно" : "Не успешная валидация");
        Console.ResetColor();
    }

   
}
