using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DFWMobile.Models;
using DFWMobile.Models.ViewModels;
using System.Globalization;
using System.Configuration;

namespace DFWMobile.Controllers
{
    public class OnlineQuoteController : Controller
    {
        // GET: OnlineQuote
        public ActionResult Index(string SlabColorID)
        {
            if(this.SlabPromoOnly("-1") == "true" && String.IsNullOrEmpty(SlabColorID))
            {
                return Redirect("http://granitesouthlake.com/OnlineQuote-promo.aspx");
            }

            DFWRepository dfwContext = new DFWRepository();

            OnlineQuoteVM onlineQuoteVM = new OnlineQuoteVM();
            onlineQuoteVM.SlabColors = dfwContext.getAllSlabColor(SlabColorID);
            onlineQuoteVM.Edges = dfwContext.getAllEdges();
            onlineQuoteVM.Measures = dfwContext.getAllMeasures();
            onlineQuoteVM.Sinks = dfwContext.getAllSinks();
            onlineQuoteVM.Services = dfwContext.getAllServices();
            ViewBag.SlabColorID = SlabColorID;
            ViewBag.SlabPromo = SlabColorID;

            if ((String.IsNullOrEmpty(ViewBag.SlabPromo) || ViewBag.SlabPromo == "0"))
            {
                ViewBag.ImageVisible = "hidden";
                ViewBag.ImageHidden = "";
            }
            else
            {
                ViewBag.ImageFileName = GetSlabImage(SlabColorID);
                ViewBag.ImageVisible = "";
                ViewBag.ImageHidden = "hidden";
            }
            return View(onlineQuoteVM);

        }

        [HttpPost]
        public string AddOnlineQuote(string jsonMeasures, string SlabColorID, string SFQty, string EdgeID, string jsonSinks, string jsonServices, string jsonCustomer, string Notes, string SlabPromoOveride)
        {
            QuoteCustomer _customer = Newtonsoft.Json.JsonConvert.DeserializeObject<QuoteCustomer>(jsonCustomer);
            QuoteCountertop _countertop = new QuoteCountertop(int.Parse(SlabColorID), double.Parse(SFQty));
            List<QuoteMeasure> _measures = Newtonsoft.Json.JsonConvert.DeserializeObject<List<QuoteMeasure>>(jsonMeasures);
            List<QuoteSink> _sinks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<QuoteSink>>(jsonSinks);
            List<QuoteService> _services = Newtonsoft.Json.JsonConvert.DeserializeObject<List<QuoteService>>(jsonServices);

            DFWRepository dfwContext = new DFWRepository();
            _customer.CustomerFirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_customer.CustomerFirstName.ToLower());
            _customer.CustomerLastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_customer.CustomerLastName.ToLower());
            int OnlineQuoteID = dfwContext.addCustomer(_customer, EdgeID, Notes);
            if (!String.IsNullOrEmpty(SlabPromoOveride))
                _countertop.FabPricePrintOveride = dfwContext.getFabPriceOveride(_countertop.SlabColorID);
            else
                _countertop.FabPricePrintOveride = null;
            int OnlineQuoteStoneID = dfwContext.addSlab(_countertop, OnlineQuoteID);

            foreach (var measure in _measures)
            {
                try
                {
                    dfwContext.addMeasure(measure, OnlineQuoteID, OnlineQuoteStoneID);
                }
                catch { }
            }

            foreach (var sink in _sinks)
            {
                try
                {
                    dfwContext.addSink(sink, OnlineQuoteID);
                }
                catch { }
            }

            foreach (var servicej in _services)
            {
                try
                {
                    dfwContext.addService(servicej, OnlineQuoteID);
                }
                catch { }
            }

            //Add Edge
            var filePath = Server.MapPath("/") + "Web.config";
            var map = new ExeConfigurationFileMap { ExeConfigFilename = filePath };
            var configFile = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            string EdgeServiceID = configFile.AppSettings.Settings["EdgeServiceID"].Value;
            QuoteService edgeService = new QuoteService();
            edgeService.ServicesID = EdgeServiceID;
            dfwContext.addService(edgeService, OnlineQuoteID);

            PrintQuoteVM printQuoteVM = dfwContext.getPrintQuote(OnlineQuoteID.ToString());            
            var pdfViewResult = new Rotativa.ViewAsPdf("Print", printQuoteVM);
            var pdfByteResult = pdfViewResult.BuildPdf(this.ControllerContext);

            return SendCustInfo(OnlineQuoteID, _customer.CustomerFirstName + " " + _customer.CustomerLastName, _customer.Email, _countertop.FabPricePrintOveride, pdfByteResult);
        }

        public ActionResult Print(string id, string AdminView)
        {
            if (!String.IsNullOrEmpty(id))
            {
                DFWRepository dfwContext = new DFWRepository();
                PrintQuoteVM printQuoteVM = dfwContext.getPrintQuote(id);
                ViewBag.ShowMenu = "none";

                if (String.IsNullOrEmpty(AdminView))
                    ViewBag.AdminView = "none";
                else
                    ViewBag.AdminView = "block";

                return new Rotativa.ViewAsPdf(printQuoteVM) 
                { 
                    IsGrayScale = true,
                    //CustomSwitches = "--title \"DFW Quote [ " + printQuoteVM.OnlineQuoteID + " - " + printQuoteVM.CustomerName + " ]\""
                };
            }
            else
                return RedirectToAction("Index");
        }

        public ActionResult Preview(string id, string AdminView)
        {
            if (!String.IsNullOrEmpty(id))
            {
                DFWRepository dfwContext = new DFWRepository();
                PrintQuoteVM printQuoteVM = dfwContext.getPrintQuote(id);
                ViewBag.ShowMenu = "block";

                if (String.IsNullOrEmpty(AdminView))
                    ViewBag.AdminView = "none";
                else
                    ViewBag.AdminView = "block";

                return View("Print", printQuoteVM);
            }
            else
                return RedirectToAction("Index");
        }
        public ActionResult PrintURL(string id)
        {
            return new Rotativa.UrlAsPdf("http://www.granitesouthlake.com/m/online-quote/QUOTE_DFW-Wholesale-Granite.aspx?OnlineQuoteID=" + id ) { FileName = "DFW Quote - " + id + ".pdf" };
        }

        public string GetSlabImage(string SlabColorID)
        {
            DFWRepository dfwRep = new DFWRepository();
            return dfwRep.getSlabFilename(SlabColorID);
        }

        //public string SendCustInfo(int OnlineQuoteID, string custname, string custemail, double? FabPricePrintOveride)
        //private string SendCustInfo(int OnlineQuoteID, string custname, string custemail, double? FabPricePrintOveride, byte[] objAttachment)
        public string SendCustInfo(int OnlineQuoteID, string custname, string custemail, double? FabPricePrintOveride, byte[] objAttachment)
        
        {
            string file = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"/App_Data"), "blocklist.txt");
            string blocklist = System.IO.File.ReadAllText(file);
            if(blocklist.Contains(custemail))
            {
                string ROOT = Url.Content(Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port));
                if (FabPricePrintOveride == null)
                    return ROOT + "/mirror/OnlineQuote/preview/"+ OnlineQuoteID + "?CustView=true";
            }


            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.From = new System.Net.Mail.MailAddress("webform@granitesouthlake.com");

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
            message.To.Add(new System.Net.Mail.MailAddress("webmaster@granitesouthlake.com"));

            string accdbConnection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath("/") + "App_Data\\DFWwebsiteDB.accdb;Persist Security Info=True";
            string query = "SELECT Email FROM tblEmailNotify WHERE (NotifyCustomerQuote = true)";
            System.Data.OleDb.OleDbConnection conn2 = new System.Data.OleDb.OleDbConnection(accdbConnection);
            System.Data.OleDb.OleDbCommand cmd = new System.Data.OleDb.OleDbCommand(query, conn2);
            System.Data.OleDb.OleDbDataReader dr;
            try
            {
                conn2.Open();
                dr = cmd.ExecuteReader();
                while (dr.Read())
                    message.To.Add(new System.Net.Mail.MailAddress(dr.GetString(dr.GetOrdinal("Email"))));
                dr.Close();
            }
            catch { }
            finally { conn2.Close(); }

            message.Attachments.Add(new System.Net.Mail.Attachment(new System.IO.MemoryStream(objAttachment), "DFW Quote [ " + OnlineQuoteID + " - " + custname + " ].pdf", System.Net.Mime.MediaTypeNames.Application.Pdf));
            
            message.ReplyToList.Add(new System.Net.Mail.MailAddress(custemail));


            message.Subject = "[New Web Quote]: from " + custname;
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendLine("<html><body><div marginwidth=\"0\" marginheight=\"0\" style=\"font:14px/20px 'Helvetica',Arial,sans-serif;margin:0;padding:20px 0 20px 0;text-align:center;background-color:#b84d45\">");
            builder.AppendLine("<center><table border=\"0\" cellpadding=\"20\" cellspacing=\"0\" height=\"100%\" width=\"100%\" style=\"background-color:#b84d45\">");
            builder.AppendLine("<tbody><tr><td align=\"center\" valign=\"top\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\" style=\"border-radius:6px;background-color:none\">");
            builder.AppendLine("<tbody><tr><td align=\"center\" valign=\"top\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\">");
            builder.AppendLine("<tbody><tr><td><div style=\"text-align:center\"><img src=\"http://www.granitesouthlake.com/images/logo.png\" alt=\"\" border=\"0\" /></div>");
            builder.AppendLine("</td></tr></tbody></table> </td> </tr>");
            builder.AppendLine("<tr><td align=\"center\" valign=\"top\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\" style=\"border-radius:6px;background-color:#ffffff\">");
            builder.AppendLine("<tbody><tr><td align=\"left\" valign=\"top\" style=\"line-height:150%;font-family:Helvetica;font-size:16px;color:#333333;padding:20px\">");
            builder.AppendLine("<div style=\"display:block;text-align:center\"><p style=\"padding:0 0 10px 0\">A new customer has just filled up the ONLINE QUOTE Mobile Form.</p>");
            if (FabPricePrintOveride == null)
                builder.AppendLine("<a href=\"http://www.granitesouthlake.com/QUOTE_DFW-Wholesale-Granite.aspx?OnlineQuoteID=" + OnlineQuoteID + "\" style=\"color:#ffffff;display:inline-block;font-family:'Helvetica',Arial,sans-serif;width:auto;white-space:nowrap;min-height:32px;margin:5px 5px 0 0;padding:0 22px;text-decoration:none;text-align:center;font-weight:bold;font-style:normal;font-size:15px;line-height:32px;border:0;border-radius:4px;vertical-align:top;background-color:#b84d45\" target=\"_blank\">");
            else
                builder.AppendLine("<a href=\"http://www.granitesouthlake.com/QUOTEWG_DFW-Wholesale-Granite.aspx?OnlineQuoteID=" + OnlineQuoteID + "\" style=\"color:#ffffff;display:inline-block;font-family:'Helvetica',Arial,sans-serif;width:auto;white-space:nowrap;min-height:32px;margin:5px 5px 0 0;padding:0 22px;text-decoration:none;text-align:center;font-weight:bold;font-style:normal;font-size:15px;line-height:32px;border:0;border-radius:4px;vertical-align:top;background-color:#b84d45\" target=\"_blank\">");
            builder.AppendLine("<span style=\"display:inline;font-family:'Helvetica',Arial,sans-serif;text-decoration:none;font-weight:bold;font-style:normal;font-size:15px;line-height:32px;border:none;background-color:#b84d45;color:#ffffff\">View the generated Online Quote</span></a>");
            builder.AppendLine("<p></p><a href=\"http://www.granitesouthlake.com/mobile/OnlineQuote/blocklist?email=" + custemail + "\" target=\"_blank\">");
            builder.AppendLine("<span style=\"display:inline;font-family:'Helvetica',Arial,sans-serif;text-decoration:none;font-weight:bold;font-style:normal;font-size:12px;line-height:20px;border:none;color:#555\">Add " + custemail + " to blocklist</span></a>");
            builder.AppendLine("</div><br /></td></tr></tbody></table></td></tr><tr><td align=\"center\" valign=\"top\"></td></tr></tbody></table></td></tr></tbody></table></center></div></body></html>");


            System.Net.Mail.AlternateView altView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(builder.ToString(), null, System.Net.Mime.MediaTypeNames.Text.Html);
            message.AlternateViews.Add(altView);

            try
            {
                mailClient.Send(message);
                string ROOT = Url.Content(Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port));
                if (FabPricePrintOveride == null)
                    return ROOT + "/QUOTE_DFW-Wholesale-Granite.aspx?OnlineQuoteID=" + OnlineQuoteID;
                else
                    return ROOT + "/QUOTEWG_DFW-Wholesale-Granite.aspx?OnlineQuoteID=" + OnlineQuoteID;

            }
            catch
            {
                return "err";
            }

        }

        public string EmailQuote(string id)
        {
            string OnlineQuoteID = id;
            DFWRepository dfwContext = new DFWRepository();
            PrintQuoteVM printQuoteVM = dfwContext.getPrintQuote(OnlineQuoteID);
            var pdfViewResult = new Rotativa.ViewAsPdf("Print", printQuoteVM);
            var objAttachment = pdfViewResult.BuildPdf(this.ControllerContext);

            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.From = new System.Net.Mail.MailAddress("webform@granitesouthlake.com");

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
            message.To.Add(new System.Net.Mail.MailAddress(printQuoteVM.Email));
            message.To.Add(new System.Net.Mail.MailAddress("webmaster@granitesouthlake.com"));
            message.Attachments.Add(new System.Net.Mail.Attachment(new System.IO.MemoryStream(objAttachment), "DFW Quote [ " + OnlineQuoteID + " - " + printQuoteVM.CustomerName + " ].pdf", System.Net.Mime.MediaTypeNames.Application.Pdf));
            message.ReplyToList.Add(new System.Net.Mail.MailAddress("dhitt0327@gmail.com"));

            message.Subject = "[DFW Wholesale Granite] - Quote for your Granite Countertop";
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.AppendLine("<html><body><div marginwidth=\"0\" marginheight=\"0\" style=\"font:14px/20px 'Helvetica',Arial,sans-serif;margin:0;padding:20px 0 20px 0;text-align:center;background-color:#b84d45\">");
            builder.AppendLine("<center><table border=\"0\" cellpadding=\"20\" cellspacing=\"0\" height=\"100%\" width=\"100%\" style=\"background-color:#b84d45\">");
            builder.AppendLine("<tbody><tr><td align=\"center\" valign=\"top\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"560\" style=\"border-radius:6px;background-color:none\">");
            builder.AppendLine("<tbody><tr><td align=\"center\" valign=\"top\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"560\">");
            builder.AppendLine("<tbody><tr><td><div style=\"text-align:center\"><img src=\"http://www.granitesouthlake.com/images/logo.png\" alt=\"\" border=\"0\" /></div>");
            builder.AppendLine("</td></tr></tbody></table> </td> </tr>");
            builder.AppendLine("<tr><td align=\"center\" valign=\"top\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"560\" style=\"border-radius:6px;background-color:#ffffff\">");
            builder.AppendLine("<tbody><tr><td align=\"left\" valign=\"top\" style=\"line-height:150%;font-family:Helvetica;font-size:12px;color:#333333;padding:20px\">");
            builder.AppendLine("<div style=\"display:block;\"><p style=\"padding:0 0 10px 0\">Dear <strong>" + printQuoteVM.CustomerName + "</strong>,</p>");
            builder.AppendLine("<p>Thank you very much for choosing DFW Wholesale Granite!</p>");
            builder.AppendLine("<p>Attached is your quote. You can also view the live copy by clicking the red button below.</p>");
            if (!printQuoteVM.FabPricePrintOveride)
                builder.AppendLine("<a href=\"http://www.granitesouthlake.com/QUOTE_DFW-Wholesale-Granite.aspx?OnlineQuoteID=" + OnlineQuoteID + "\" style=\"color:#ffffff;display:inline-block;font-family:'Helvetica',Arial,sans-serif;width:auto;white-space:nowrap;min-height:32px;margin:5px 5px 0 0;padding:0 22px;text-decoration:none;text-align:center;font-weight:bold;font-style:normal;font-size:15px;line-height:32px;border:0;border-radius:4px;vertical-align:top;background-color:#b84d45\" target=\"_blank\">");
            else
                builder.AppendLine("<a href=\"http://www.granitesouthlake.com/QUOTEWG_DFW-Wholesale-Granite.aspx?OnlineQuoteID=" + OnlineQuoteID + "\" style=\"color:#ffffff;display:inline-block;font-family:'Helvetica',Arial,sans-serif;width:auto;white-space:nowrap;min-height:32px;margin:5px 5px 0 0;padding:0 22px;text-decoration:none;text-align:center;font-weight:bold;font-style:normal;font-size:15px;line-height:32px;border:0;border-radius:4px;vertical-align:top;background-color:#b84d45\" target=\"_blank\">");
            builder.AppendLine("<span style=\"display:inline;font-family:'Helvetica',Arial,sans-serif;text-decoration:none;font-weight:bold;font-style:normal;font-size:15px;line-height:32px;border:none;background-color:#b84d45;color:#ffffff\">View Online Quote</span></a>");
            builder.AppendLine("<p>This quote is only an estimate. We will come to your project to measure or template before we proceed. For future reference, your job quote number is " + printQuoteVM.OnlineQuoteID + ".</p>");
            builder.AppendLine("<p>Should you want to contact us, feel free to call us or simply reply to this email.</p>");
            builder.AppendLine("<p style=\"padding:0 0 10px 0\">Best regards,</p>");
            builder.AppendLine("<p><strong>DFW WHOLESALE GRANITE</strong> <br />David Hitt <br />817-300-3298 <br />info@granitesouthlake.com <br />info@granitefortworth.net</p>");
            builder.AppendLine("</div></td></tr></tbody></table></td></tr><tr><td align=\"center\" valign=\"top\"></td></tr></tbody></table></td></tr></tbody></table></center></div></body></html>");


            System.Net.Mail.AlternateView altView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(builder.ToString(), null, System.Net.Mime.MediaTypeNames.Text.Html);
            message.AlternateViews.Add(altView);

            try
            {
                mailClient.Send(message);
                return "sent";
            }
            catch
            {
                return "err";
            }

        }

        public string SlabPromoOnly(string id)
        {
            var filePath = Server.MapPath("/") + "Web.config";
            var map = new ExeConfigurationFileMap { ExeConfigFilename = filePath };
            var configFile = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            string SlabPromoOnly = configFile.AppSettings.Settings["SlabPromoOnly"].Value;

            if (id == "0")
            {
                configFile.AppSettings.Settings["SlabPromoOnly"].Value = "false";
                configFile.Save();
                return "false";
            }
            else if (id == "1")
            {
                configFile.AppSettings.Settings["SlabPromoOnly"].Value = "true";
                configFile.Save();
                return "true";
            }
            else
                return configFile.AppSettings.Settings["SlabPromoOnly"].Value;
            
        }

        public ActionResult Menu(string id)
        {
            ViewBag.OnlineQuoteID = id;
            return View();
        }

        [HttpGet]
        public ActionResult BlockList(string email)
        {
            string file = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"/App_Data"), "blocklist.txt");
            string blocklist = System.IO.File.ReadAllText(file);

            if (!String.IsNullOrEmpty(email) && !blocklist.Contains(email))
            {
                try
                {
                    System.IO.File.AppendAllText(file, Environment.NewLine + email );
                    ViewBag.Msg = email + " has been added to blocklist.";
                }
                catch
                {
                    ViewBag.Msg = "An error had occured. Unable to perform blocklisting. Please try again.";
                }
            }

            ViewBag.BlockList = System.IO.File.ReadAllText(file);
            return View();
        }

        [HttpPost]
        public ActionResult BlockList(FormCollection form)
        {
            string file = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"/App_Data"), "blocklist.txt");

            try
            {
                System.IO.File.WriteAllText(file, form["blocklist"]);
                ViewBag.Msg = "Changes has been successfuly saved.";
            }
            catch
            {
                ViewBag.Msg = "An error had occured. Changes not saved. Please try again.";
            }

            ViewBag.BlockList = System.IO.File.ReadAllText(file);
            return View();
        }

    }
}
