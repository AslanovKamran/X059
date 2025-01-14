using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.Xml;

namespace X509SignTest.Utils;

public static class CertificateUtils
{
    public static void SignXmlDocumentWithCertificate(XmlDocument doc, X509Certificate2 cert)
    {
        // Retrieve the private key using GetRSAPrivateKey()
        RSA? privateKey = cert.GetRSAPrivateKey() ?? throw new InvalidOperationException("Сертификат не содержит действительного закрытого ключа RSA.");

        // Create SignedXml object and set the signing key
        SignedXml signedXml = new(doc);
        signedXml.SigningKey = privateKey;


        // Create a reference to the XML document you want to sign
        Reference reference = new();
        reference.Uri = ""; // An empty URI means the entire document will be signed

        // Add the EnvelopedSignatureTransform to the reference
        // This ensures the signature is applied over the entire XML document without including the signature itself
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

        // Add the reference to the SignedXml object
        signedXml.AddReference(reference);

        // Create a KeyInfo object to include information about the signing key
        KeyInfo keyInfo = new();
        keyInfo.AddClause(new KeyInfoX509Data(cert)); // Add the certificate's X509 data to the KeyInfo


        // Set the KeyInfo property of the SignedXml object
        signedXml.KeyInfo = keyInfo;

        // Compute the XML digital signature
        signedXml.ComputeSignature();

        // Get the XML representation of the computed signature
        XmlElement xmlSignature = signedXml.GetXml();

        // Append the signature element to the XML document
        // Use ImportNode to ensure compatibility with the target document
        doc.DocumentElement!.AppendChild(doc.ImportNode(xmlSignature, true));
    }

    public static X509Certificate2? GetCertificateFromStore(StoreName name, StoreLocation location)
    {
        using (var store = new X509Store(name, location))
        {
            store.Open(OpenFlags.ReadOnly);
            var certificatesList = store.Certificates;

            if (certificatesList.Count == 0)
            {
                Console.WriteLine("В Store не найдено ни одного сертификата.");
                return null;
            }

            Console.WriteLine("Доступные сертификаты:\n");
            for (int i = 0; i < certificatesList.Count; i++)
            {
                Console.WriteLine($"#{i + 1} Отличительное имя субъекта: {certificatesList[i].Subject}");
                Console.WriteLine($"    Дружелюбное имя: {certificatesList[i].FriendlyName}");
                Console.WriteLine($"    Издатель: {certificatesList[i].Issuer}");
                Console.WriteLine($"    Отпечаток: {certificatesList[i].Thumbprint}");
                Console.WriteLine($"    Выдан: {certificatesList[i].NotBefore}");
                Console.WriteLine($"    Дата истечения: {certificatesList[i].NotAfter}");
                Console.WriteLine($"    Версия: {certificatesList[i].Version}");
                Console.WriteLine($"    Можно ли использовать для шифрования: {certificatesList[i].HasPrivateKey}");
                Console.WriteLine(new string('-', 50));
            }

            do
            {
                Console.Write("Введите номер сертификата, который вы хотите использовать  => ");
                if (int.TryParse(Console.ReadLine(), out int selectedIndex) &&
                    selectedIndex > 0 &&
                    selectedIndex <= certificatesList.Count)
                {
                    return certificatesList[selectedIndex - 1];
                }
                Console.WriteLine("Неверный выбор. Пожалуйста, попробуйте еще раз.");
            } while (true);

        }
    }
}
