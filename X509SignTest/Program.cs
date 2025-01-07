using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.Xml;

namespace X509SignTest;

class Programm
{
    const string PathToCertificate = @"C:\Users\DOIT\Desktop\doit-test-cert.pfx";
    const string Password = "1234567890";
    const string PathToSave = @"C:\Users\DOIT\Desktop\Signed.xml";
    static void Main(string[] args)
    {
        // Message to sign 
        string message = "Hello World!";

        // Create an XmlDocument and load the message into a root element
        XmlDocument doc = new();
        XmlElement root = doc.CreateElement("Message");
        root.InnerText = message;
        doc.AppendChild(root);

        // XML structure after this step:
        // <Message>
        //     Hello World!
        // </Message>

        try
        {
            // Load the X509 certificate from the PFX file with its password
            X509Certificate2 cert = new(rawData: File.ReadAllBytes(PathToCertificate), password: Password);

            // Sign the XML document with the certificate
            SignXmlDocumentWithCertificate(doc, cert);

            // Save the signed XML document to a file
            File.WriteAllText(PathToSave, doc.OuterXml);

            // Indicate success with a green console message
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Written to signed.xml");
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

    public static void SignXmlDocumentWithCertificate(XmlDocument doc, X509Certificate2 cert)
    {
        // Retrieve the private key using GetRSAPrivateKey()
        RSA? privateKey = cert.GetRSAPrivateKey() ?? throw new InvalidOperationException("The certificate does not contain a valid RSA private key.");

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
}

