using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Net.Mail;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace DFWMobile.Controllers
{
    public class SendContactController : Controller
    { 
        // Post: SendContact
        [HttpPost]
        public string Index(string name, string streetcity, string phone, string email, string msg)
        {
            return SendWebContact(name, streetcity, phone, email, msg, Request.Browser.Browser + ": Responsive/" + Request.UserAgent);
        }

        private string SendWebContact(string var_name, string var_streetcity, string var_phone, string var_email, string var_msg, string var_agent)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress("webform@granitesouthlake.com");

            message.IsBodyHtml = true;

            // Proper Authentication Details need to be passed when sending email from gmail
            System.Net.NetworkCredential mailAuthentication = new
                System.Net.NetworkCredential("webform@granitesouthlake.com", "2*****e");

            // Smtp Mail server of Gmail is "smpt.gmail.com" and it uses port no. 587
            // For different server like yahoo this details changes and you can
            // Get it from respective server.
            System.Net.Mail.SmtpClient mailClient = new System.Net.Mail.SmtpClient("smtp.granitesouthlake.com", 587);

            // Enable SSL
            //mailClient.EnableSsl = true;
            mailClient.UseDefaultCredentials = false;
            mailClient.Credentials = mailAuthentication;

            //add accounts to notify
            //if (!var_name.Equals("gabstest".Trim()) || var_email.Trim() != "hewbertgabon@gmail.com" || !SpamEmailFound(var_email.Trim()))
            //{
            //}
            message.To.Add(new MailAddress("dhitt0327@gmail.com"));
            message.To.Add(new MailAddress("rlowman64@gmail.com"));
            message.To.Add(new MailAddress("hewbertgabon@gmail.com"));

            if ((var_name.Trim()).Equals(string.Empty) | (var_email.Trim()).Equals(string.Empty))
            {
                return "Message delivery failed. Please try again.";
            }
            else
            {
                //if (System.Text.RegularExpressions.Regex.IsMatch(email, @"\A(?:[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?)\Z") == false)
                //context.Response.Redirect("Default.aspx?Send=Failed");
                //else

                message.Subject = "[New Web Contact]: Message from " + var_name;
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                builder.AppendLine("<html><body><div style=\"padding:15px;background-color: #F8F8F8; font-family:Verdana, Geneva, Tahoma, sans-serif;font-size:small;\">");
                builder.AppendLine("<div style=\"border:2px solid #a1a1a1;padding:10px 40px; background:#dddddd;border-radius:25px; font-family:Georgia, 'Times New Roman', Times, serif; font-size:xx-large;text-align:center\">");
                builder.AppendLine("<a href=\"http://www.granitesouthlake.com\" style=\"color:black;text-decoration:none;\">DFW WHOLESALE GRANITE</a></div>");
                builder.AppendLine("<p>Hello! Here is a new message sent via the Web Contact Form.</p>");
                builder.AppendLine("<table style=\"width: 90%;border-collapse:collapse;font-family:Verdana, Geneva, Tahoma, sans-serif;font-size:small;\">");
                builder.AppendLine("<tr style=\"border-top: 1px solid gray;\"><td style=\"width: 20%;vertical-align: top;\"><strong>Name:</strong></td>");
                builder.AppendLine("<td style=\" vertical-align: top;\">" + var_name + "</td></tr>");
                builder.AppendLine("<tr style=\"border-top: 1px solid gray;\"><td style=\"vertical-align: top;\"><strong>Street:</strong></td>");
                builder.AppendLine("<td style=\"vertical-align: top;\">" + var_streetcity + "</td></tr>");
                builder.AppendLine("<tr style=\"border-top: 1px solid gray;\"><td style=\"vertical-align: top;\"><strong>Phone:</strong></td>");
                builder.AppendLine("<td style=\"vertical-align: top;\">" + var_phone + "</td></tr>");
                builder.AppendLine("<tr style=\"border-top: 1px solid gray;\"><td style=\"vertical-align: top;\"><strong>E-mail:</strong></td>");
                builder.AppendLine("<td style=\"vertical-align: top;\">" + var_email + " <a href=\"http://www.granitesouthlake.com/BlockEmail.aspx?name=" + var_name + "&email=" + var_email + "\" style=\"-moz-box-shadow:inset 0px 1px 0px 0px #f5978e;-webkit-box-shadow:inset 0px 1px 0px 0px #f5978e;box-shadow:inset 0px 1px 0px 0px #f5978e;background:-webkit-gradient( linear, left top, left bottom, color-stop(0.05, #f24537), color-stop(1, #c62d1f) );background:-moz-linear-gradient( center top, #f24537 5%, #c62d1f 100% );filter:progid:DXImageTransform.Microsoft.gradient(startColorstr='#f24537', endColorstr='#c62d1f');background-color:#f24537;-webkit-border-top-left-radius:20px;-moz-border-radius-topleft:20px;border-top-left-radius:20px;-webkit-border-top-right-radius:20px;-moz-border-radius-topright:20px;border-top-right-radius:20px;-webkit-border-bottom-right-radius:20px;-moz-border-radius-bottomright:20px;border-bottom-right-radius:20px;-webkit-border-bottom-left-radius:20px;-moz-border-radius-bottomleft:20px;border-bottom-left-radius:20px;text-indent:0px;border:1px solid #d02718;display:inline-block;color:#ffffff;font-family:Arial;font-size:10px;font-weight:bold;font-style:normal;height:18px;line-height:16px;width:64px;text-decoration:none;text-align:center;text-shadow:1px 1px 0px #810e05;\">BLOCK</a> ");
                builder.AppendLine("</td></tr>");
                builder.AppendLine("<tr style=\"border-top: 1px solid gray;border-bottom: 1px solid gray;\"><td style=\"vertical-align: top;\"><strong>Message:</strong></td>");
                builder.AppendLine("<td style=\"vertical-align: top;\">" + var_msg + "</td></tr></table>");
                builder.AppendLine("<p style=\"font-size:xx-small;color:gray;\">HTTP_USER_AGENT: " + var_agent + "</p></div></body></html>");

                message.Body = builder.ToString();

                try
                {
                    message.ReplyToList.Add(new MailAddress(var_email));
                    mailClient.Send(message);
                }
                catch (Exception ex)
                {
                    return "Message delivery failed. Please try again.";
                }

            }

            return "Message successfully sent.";                      

        }

        private bool SpamEmailFound(string CustEmail)
        {
            bool EmailFound = false;
            string strFilePath = Server.MapPath("/") + "App_Data\\SpamEmail.xml";

            if (System.IO.File.Exists(strFilePath))
            {
                XPathNavigator nav;
                XPathDocument docNav;
                //XPathNodeIterator NodeIter;
                string strExpression;

                // Open the XML.
                docNav = new XPathDocument(strFilePath);

                // Create a navigator to query with XPath.
                nav = docNav.CreateNavigator();

                // Find the average cost of a book.
                // This expression uses standard XPath syntax.
                strExpression = "count(//SpamEmail[email='" + CustEmail + "'])";

                // Use the Evaluate method to return the evaluated expression.
                if (int.Parse((nav.Evaluate(strExpression)).ToString()) > 0)
                    EmailFound = true;
            }
            return EmailFound;
        }

    }
}
