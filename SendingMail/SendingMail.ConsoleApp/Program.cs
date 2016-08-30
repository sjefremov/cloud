using System;
using System.Net;
using System.Net.Mail;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Threading.Tasks;

namespace SendingMail.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            Execute().Wait();
            Console.ReadLine();
            
        }

        static async Task Execute()
        {
            // Create the email object first, then add the properties.
            var mail = new Mail(new Email("riste.poposki@gmail.com"), "Testing the SendGrid Library", new Email(@"orce.petreski@gmail.com"), new Content("text", "Hello World plain text!"));

            var sendGrid = new SendGridAPIClient("");
            dynamic response = await sendGrid.client.mail.send.post(requestBody: mail.Get());
        }
    }
}
