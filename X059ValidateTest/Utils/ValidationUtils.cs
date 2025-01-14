using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.Xml;

namespace X059ValidateTest.Utils;

public static class ValidationUtils
{
    public static bool ValidateXmlDocumentWithCertificate(XmlDocument doc, X509Certificate2 cert)
    {
        try
        {
            // Initialize SignedXml object for validation
            SignedXml signedXml = new SignedXml(doc);

            // Locate the <Signature> element in the XML document
            XmlNode? signatureNode = doc.GetElementsByTagName("Signature")[0];

            if (signatureNode is null)
            {
                Console.WriteLine("В докумете отсутствует ЭП.");
                return false;
            }

            // Load the <Signature> element into the SignedXml object
            signedXml.LoadXml((XmlElement)signatureNode);

            // Check the signature's validity using the public key certificate
            bool isSignatureValid = signedXml.CheckSignature(cert, true);

            if (!isSignatureValid)
                Console.WriteLine("ЭП не валидна.");

            return isSignatureValid;
        }
        catch (CryptographicException ex)
        {
            Console.WriteLine($"Произошла ошибка криптографии во время валидации: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Непредвиденная ошибка во время валидации: {ex.Message}");
            return false;
        }
    }
}
