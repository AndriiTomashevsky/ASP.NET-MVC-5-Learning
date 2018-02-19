using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace SportsStore.Domain.Concrete
{
    public class EmailSettings
    {
        public string MailToAddress = "orders@example.com";
        public string MailFromAddress = "sportsstore@example.com";
        public bool UseSsl = true;
        public string Username = "MySmtpUsername";
        public string Password = "MySmtpPassword";
        public string ServerName = "smtp.example.com";
        public int ServerPort = 587;
        public bool WriteAsFile = false;
        public string FileLocation = @"c:\sports_store_emails";
    }

    public class EmailOrderProcessor : IOrderProcessor
    {
        EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings emailSettings)
        {
            this.emailSettings = emailSettings;
        }

        //Implement IOrderProcessor
        public void ProcessOrder(Cart cart, ShippingDetails shippingDetails)
        {
            using (var smptClient = new SmtpClient())
            {
                smptClient.EnableSsl = emailSettings.UseSsl;
                smptClient.Host = emailSettings.ServerName;
                smptClient.Port = emailSettings.ServerPort;
                smptClient.UseDefaultCredentials = false;
                smptClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                if (emailSettings.WriteAsFile)
                {
                    smptClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smptClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smptClient.EnableSsl = false;
                }

                StringBuilder stringBuilder = new StringBuilder()
                    .Append("A new order has been submitted")
                    .AppendLine("---")
                    .AppendLine("Items:");

                foreach (var item in cart.Lines)
                {
                    var subtotal = item.Product.Price * item.Quantity;
                    stringBuilder.AppendFormat($"{item.Quantity} x {item.Product.Name} ( subtotal: {subtotal}:c)");
                }

                stringBuilder.AppendFormat($"Total order value: {cart.ComputeTotalValue()}:c")
                    .AppendLine("---")
                    .AppendLine("Ship to:")
                    .AppendLine(shippingDetails.Name)
                    .AppendLine(shippingDetails.Line1)
                    .AppendLine(shippingDetails.Line2 ?? "")
                    .AppendLine(shippingDetails.Line3 ?? "")
                    .AppendLine(shippingDetails.City)
                    .AppendLine(shippingDetails.State ?? "")
                    .AppendLine(shippingDetails.Country)
                    .AppendLine(shippingDetails.Zip)
                    .AppendLine("---")
                    .AppendFormat("Gift wrap: {0}", shippingDetails.GiftWrap ? "Yes" : "No");

                MailMessage mailMessage =
                    new MailMessage(emailSettings.MailFromAddress, emailSettings.MailToAddress,
                                              "New order submitted!", stringBuilder.ToString());

                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }
                smptClient.Send(mailMessage);
            }
        }
    }
}
