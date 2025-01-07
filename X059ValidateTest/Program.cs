using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace X059ValidateTest;

class Program
{
    // Paths to the signed XML file and the public key certificate
    const string TargetFilePath = @"C:\Users\DOIT\Desktop\signed.xml";
    const string PublicKeyPath = @"C:\Users\DOIT\Desktop\doit-test-cert-public.pem";
    static void Main(string[] args)
    {
        if (!File.Exists(TargetFilePath))
        {
            Console.WriteLine("File doesn't exist.");
            return;
        }

        // Read the contents of the signed XML file
        string xmlString = File.ReadAllText(TargetFilePath);

        // Load the XML into an XmlDocument object
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlString);

        // Load the public key certificate for validation
        X509Certificate2 pubCert = new X509Certificate2(PublicKeyPath);

        // Validate the XML document's signature using the public key certificate
        bool isValid = ValidateXmlDocumentWithCertificate(doc, pubCert);
        Console.WriteLine($"Signature validation result: {isValid}");
    }

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
                Console.WriteLine("No Signature node found in the XML document.");
                return false;
            }

            // Load the <Signature> element into the SignedXml object
            signedXml.LoadXml((XmlElement)signatureNode);

            // Check the signature's validity using the public key certificate
            bool isSignatureValid = signedXml.CheckSignature(cert, true);

            if (!isSignatureValid)
                Console.WriteLine("The XML signature is invalid.");

            return isSignatureValid;
        }
        catch (CryptographicException ex)
        {
            Console.WriteLine($"Cryptographic error during signature validation: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error validating XML signature: {ex.Message}");
            return false;
        }
    }
}
