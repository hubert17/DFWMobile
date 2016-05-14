using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OleDb;
using DFWMobile.Models.ViewModels;
using System.Configuration;

namespace DFWMobile.Models
{
    public class DFWRepository
    {
        //string accdbConnection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\\DFWwebsiteDB.accdb;Persist Security Info=True";
        string accdbConnection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + HttpContext.Current.Server.MapPath("/") + "App_Data\\DFWwebsiteDB.accdb;Persist Security Info=True";

        public static class Cultures
        {
            public static readonly System.Globalization.CultureInfo UnitedStates =
                System.Globalization.CultureInfo.GetCultureInfo("en-US");
        }

        public List<SlabPromo> getSlabPromo(int maxRecord)
        {
            List<SlabPromo> slabPromos = new List<SlabPromo>();
            
            string query;
            if(maxRecord > 0)
                query = "SELECT TOP " + maxRecord + " SlabPromoID, SlabColorID, SlabColor, SlabPromoPrice, SlabLength, SlabWidth, SlabOnHand, SlabPromoNotes, Inactive, ImageFilename FROM SlabPromoQry WHERE (((Inactive)=False))";
            else
                query = "SELECT SlabPromoID, SlabColorID, SlabColor, SlabPromoPrice, SlabLength, SlabWidth, SlabOnHand, SlabPromoNotes, Inactive, ImageFilename FROM SlabPromoQry WHERE (((Inactive)=False))";

            using (OleDbConnection conn = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    conn.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        SlabPromo sp = new SlabPromo();

                        try
                        {
                            sp.SlabPromoID = reader.GetInt32(0);
                            sp.SlabColorID = reader.GetInt32(1);
                            sp.SlabColor = reader.GetString(2);
                            sp.SlabPromoPrice = Convert.ToDouble(reader.GetValue(3));
                            try { sp.SlabLength = reader.GetInt32(4); }
                            catch { sp.SlabLength = 0; }
                            try { sp.SlabWidth = reader.GetInt32(5); }
                            catch { sp.SlabWidth = 0; }
                            try { sp.SlabOnHand = Convert.ToDouble(reader.GetValue(6)); }
                            catch { sp.SlabOnHand = 0.0; }
                            sp.SlabPromoNotes = reader.GetValue(7).ToString();
                            sp.Inactive = reader.GetBoolean(8);
                            sp.ImageFilename = reader.GetValue(9).ToString();
                        }
                        catch
                        {
                            sp.SlabPromoID = reader.GetInt32(0);
                            sp.SlabColorID = reader.GetInt32(1);
                            sp.SlabColor = reader.GetString(2);
                            sp.SlabPromoPrice = Convert.ToDouble(reader.GetValue(3));
                        }

                        slabPromos.Add(sp);
                    }
                    //conn.Close();
                }
            }

            return slabPromos;
        }

        public SlabPromoLabel getSlabPromoLabel()
        {
            SlabPromoLabel spl = new SlabPromoLabel();
            string query = "SELECT SlabPromoLabel, SlabPromoDesc FROM tblSETTINGS WHERE (((ID)=1))";
            using (OleDbConnection conn = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    conn.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        spl.SlabPromoName = reader.GetString(0);
                        spl.SlabPromoDesc = reader.GetString(1);
                    }
                    //conn.Close();
                }
            }

            return spl;
        }

        public List<SlabColor> getAllSlabColor(string SlabColorID)
        {
            List<SlabColor> slabColors = new List<SlabColor>();

            var filePath = HttpContext.Current.Server.MapPath("/") + "Web.config";
            var map = new ExeConfigurationFileMap { ExeConfigFilename = filePath };
            var configFile = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            string SlabPromoOnly = configFile.AppSettings.Settings["SlabPromoOnly"].Value;
            string query;
            if (SlabPromoOnly != "true" && String.IsNullOrEmpty(SlabColorID))
                query = "SELECT SlabColorID, SlabColorNamePrice FROM SlabColorNamePriceQry";
            else
                query = "SELECT SlabColorID, [SlabColor] +' - '+ Format([SlabPromoPrice],'Currency') AS SlabColorNamePrice FROM SlabPromoQry WHERE Inactive=False ORDER BY SlabColor";

            using (OleDbConnection conn = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    conn.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        SlabColor sc = new SlabColor();
                        sc.SlabColorID = reader.GetInt32(0);
                        sc.SlabColorNamePrice = reader.GetValue(1).ToString();
                        slabColors.Add(sc);
                    }
                    //conn.Close();
                }
            }

            return slabColors;
        }

        public string getSlabFilename(string slabColorID)
        {
            string imgFilename = String.Empty;
            string query = String.Format("SELECT ImageFilename FROM tblSlabColors WHERE (((SlabColorID)={0}))", slabColorID);
            using (OleDbConnection conn = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    conn.Open();
                    try
                    {
                        imgFilename = cmd.ExecuteScalar().ToString();
                    }
                    catch { }
                    //conn.Close();
                }
            }

            return imgFilename;
        }

        public List<Edge> getAllEdges()
        {
            List<Edge> edge = new List<Edge>();

            string query = "SELECT EdgeID, EdgeName, Price, ImageFilename, ImageFilenameNew FROM tblEdges WHERE ((Inactive)=False)  ORDER BY EdgeName";

            using (OleDbConnection conn = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    conn.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Edge eg = new Edge();
                        eg.EdgeID = reader.GetInt32(0);
                        eg.EdgeName = reader.GetString(1);
                        eg.Price = reader.GetValue(2).ToString();
                        if (String.IsNullOrEmpty(eg.Price) || Double.Parse(eg.Price) == 0.00) eg.Price = "Free"; else eg.Price = Double.Parse(eg.Price).ToString("C", Cultures.UnitedStates);
                        eg.ImageFilename = reader.GetString(4);
                        if(String.IsNullOrEmpty(eg.ImageFilename)) eg.ImageFilename = reader.GetString(3);
                        edge.Add(eg);
                    }
                    //conn.Close();
                }
            }

            return edge;
        }

        public List<MeasureAsset> getAllMeasures()
        {
            List<MeasureAsset> measure = new List<MeasureAsset>();

            string query = "SELECT MeasureAssetID, AssetName FROM tblMeasureAssets";

            using (OleDbConnection conn = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    conn.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        MeasureAsset meas = new MeasureAsset();
                        meas.MeasureAssetID = reader.GetInt32(0);
                        meas.MeasureName = reader.GetString(1);
                        measure.Add(meas);
                    }
                    //conn.Close();
                }
            }

            return measure;
        }

        public List<Sink> getAllSinks()
        {
            List<Sink> sink = new List<Sink>();

            string query = "SELECT SinkID, CatalogID, SinkName, SinkShortName, Price, ImageFilename, Type FROM tblSinks WHERE ((Inactive)=False) ORDER BY Type, SinkName ";

            using (OleDbConnection conn = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    conn.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Sink snk = new Sink();
                        snk.SinkID = reader.GetInt32(0);
                        snk.CatalogID = snk.Type = reader.GetValue(1).ToString();
                        snk.SinkName = snk.Type = reader.GetValue(2).ToString();
                        snk.SinkShortName = snk.Type = reader.GetValue(3).ToString();
                        snk.Price = reader.GetValue(4).ToString();
                        if (!String.IsNullOrEmpty(snk.Price)) snk.Price = Double.Parse(snk.Price).ToString("C", Cultures.UnitedStates);
                        snk.ImageFilename = reader.GetValue(5).ToString();
                        snk.Type = reader.GetValue(6).ToString();
                        sink.Add(snk);
                    }
                    //conn.Close();
                }
            }

            return sink;
        }

        public List<ServiceJ> getAllServices()
        {
            List<ServiceJ> serviceJ = new List<ServiceJ>();

            string query = "SELECT ServicesID, ServiceName, ServicePrice FROM tblServices WHERE ( ((Inactive)=False) AND ((AdminOnly)=False) ) ORDER BY ServiceName ";

            using (OleDbConnection conn = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    conn.Open();
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ServiceJ svc = new ServiceJ();
                        svc.ServicesID = reader.GetInt32(0);
                        svc.ServiceName = reader.GetString(1);
                        svc.ServicePrice = reader.GetValue(2).ToString();
                        if (!String.IsNullOrEmpty(svc.ServicePrice)) svc.ServicePrice = Double.Parse(svc.ServicePrice).ToString("C", Cultures.UnitedStates);
                        serviceJ.Add(svc);
                    }
                    //conn.Close();
                }
            }

            return serviceJ;
        }

        public int addCustomer(QuoteCustomer customer, string edge, string notes)
        {
            string query = "INSERT INTO tblOnlineQuotes " +
                        " ( CustomerFirstName, CustomerLastName, Phone, Email, Address, City, State, Zipcode, Edge, Notes ) VALUES " +
                       " ( @CustomerFirstName, @CustomerLastName, @Phone, @Email, @Address, @City, @State, @Zipcode, @Edge, @Notes ) ";
            string queryId = "Select @@Identity";

            using (OleDbConnection con = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@CustomerFirstName", customer.CustomerFirstName);
                    cmd.Parameters.AddWithValue("@CustomerLastName", customer.CustomerLastName);
                    cmd.Parameters.AddWithValue("@Phone", customer.Phone);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.AddWithValue("@Address", customer.Address);
                    cmd.Parameters.AddWithValue("@City", customer.City);
                    cmd.Parameters.AddWithValue("@State", customer.State);
                    cmd.Parameters.AddWithValue("@Zipcode", customer.Zipcode);
                    cmd.Parameters.AddWithValue("@Edge", edge);
                    cmd.Parameters.AddWithValue("@Notes", notes);
                    foreach (System.Data.OleDb.OleDbParameter param in cmd.Parameters)
                    {
                        if (param.Value == null)
                            param.Value = DBNull.Value;
                    }

                    con.Open();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = queryId;
                    customer.OnlineQuoteID = int.Parse(cmd.ExecuteScalar().ToString());
                    //con.Close();
                }
            }

            return customer.OnlineQuoteID;
        }

        public int addSlab(QuoteCountertop countertop, int OnlineQuoteID)
        {
            string query = "INSERT INTO tblOnlineQuoteStone " +
                        " ( OnlineQuoteID, SlabColorID, SquareFeetQty, FabPricePrintOveride ) VALUES " +
                       " ( @OnlineQuoteID, @SlabColorID, @SquareFeetQty, @FabPricePrintOveride  ) ";
            string queryId = "Select @@Identity";

            using (OleDbConnection con = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@OnlineQuoteID", OnlineQuoteID);
                    cmd.Parameters.AddWithValue("@SlabColorID", countertop.SlabColorID);
                    cmd.Parameters.AddWithValue("@SquareFeetQty", countertop.SquareFeetQty);
                    cmd.Parameters.AddWithValue("@FabPricePrintOveride", countertop.FabPricePrintOveride);
                    foreach (System.Data.OleDb.OleDbParameter param in cmd.Parameters)
                    {
                        if (param.Value == null)
                            param.Value = DBNull.Value;
                    }

                    con.Open();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = queryId;
                    countertop.OnlineQuoteStoneID = int.Parse(cmd.ExecuteScalar().ToString());
                    //con.Close();
                }
            }

            return countertop.OnlineQuoteStoneID;
        }

        public void addMeasure(QuoteMeasure measure, int OnlineQuoteID, int OnlineQuoteStoneID)
        {
            string query = "INSERT INTO tblOnlineQuotesMeasures " +
                        " ( OnlineQuoteID, OnlineQuoteStoneID, Measure, Length, Width ) VALUES " +
                       " ( @OnlineQuoteID, @OnlineQuoteStoneID, @Measure, @Length, @Width  ) ";

            using (OleDbConnection con = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@OnlineQuoteID", OnlineQuoteID);
                    cmd.Parameters.AddWithValue("@OnlineQuoteStoneID", OnlineQuoteStoneID);
                    cmd.Parameters.AddWithValue("@Measure", measure.Measure);
                    cmd.Parameters.AddWithValue("@Length", measure.Length);
                    cmd.Parameters.AddWithValue("@Width", measure.Width);
                    foreach (System.Data.OleDb.OleDbParameter param in cmd.Parameters)
                    {
                        if (param.Value == null)
                            param.Value = DBNull.Value;
                    }

                    con.Open();
                    cmd.ExecuteNonQuery();
                    //con.Close();
                }
            }
        }

        public void addSink(QuoteSink sink, int OnlineQuoteID)
        {
            string query = "INSERT INTO tblOnlineQuotesSinks " +
                        " ( OnlineQuoteID, SinkID, Quantity ) VALUES " +
                       " ( @OnlineQuoteID, @SinkID, @Quantity ) ";

            using (OleDbConnection con = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@OnlineQuoteID", OnlineQuoteID);
                    cmd.Parameters.AddWithValue("@SinkID", sink.SinkID);
                    cmd.Parameters.AddWithValue("@Quantity", sink.Quantity);
                    foreach (System.Data.OleDb.OleDbParameter param in cmd.Parameters)
                    {
                        if (param.Value == null)
                            param.Value = DBNull.Value;
                    }

                    con.Open();
                    cmd.ExecuteNonQuery();
                    //con.Close();
                }
            }
        }

        public void addService(QuoteService serviceJ, int OnlineQuoteID)
        {
            string query = "INSERT INTO tblOnlineQuotesServices " +
                        " ( OnlineQuoteID, ServicesID ) VALUES " +
                       " ( @OnlineQuoteID, @ServicesID ) ";

            using (OleDbConnection con = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@OnlineQuoteID", OnlineQuoteID);
                    cmd.Parameters.AddWithValue("@ServicesID", serviceJ.ServicesID);
                    foreach (System.Data.OleDb.OleDbParameter param in cmd.Parameters)
                    {
                        if (param.Value == null)
                            param.Value = DBNull.Value;
                    }

                    con.Open();
                    cmd.ExecuteNonQuery();
                    //con.Close();
                }
            }
        }

        public double getFabPriceOveride(int SlabColorID)
        {
            string query = "Select SlabPromoPrice from tblSlabPromo where SlabColorID=" + SlabColorID.ToString();
            OleDbConnection conn = new OleDbConnection(accdbConnection);
            OleDbCommand cmd = new OleDbCommand(query, conn);
            conn.Open();
            double FabPriceOveride = Convert.ToDouble(cmd.ExecuteScalar().ToString());
            conn.Close();

            return FabPriceOveride;
        }

        public PrintQuoteVM getPrintQuote(string OnlineQuoteID)
        {
            PrintQuoteVM printQuote = new PrintQuoteVM();
            //check blocklisting
            string file = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath(@"/App_Data"), "blocklist.txt");
            string blocklist = System.IO.File.ReadAllText(file);

            string qryCustomer = "SELECT OnlineQuoteID, CustomerName, Address, Address2, Phone, Email, Notes FROM SummaryInfoQry WHERE OnlineQuoteID = " + OnlineQuoteID;
            using(OleDbConnection conn = new OleDbConnection(accdbConnection))
            {
                using(OleDbCommand cmd = new OleDbCommand(qryCustomer,conn))
                {
                    conn.Open();
                    
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        printQuote.OnlineQuoteID = reader.GetInt32(0);
                        printQuote.CustomerName = reader.GetString(1);
                        printQuote.Address = reader.GetValue(2).ToString();
                        printQuote.Address2 = reader.GetValue(3).ToString();
                        printQuote.Phone = reader.GetString(4);
                        printQuote.Email = reader.GetString(5);
                        printQuote.Notes = reader.GetValue(6).ToString();
                    }
                }
            }

            string qryFabPricePrintOveride = "SELECT FabPricePrintOveride FROM tblOnlineQuoteStone WHERE OnlineQuoteID=" + OnlineQuoteID;
            using (OleDbConnection conn = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(qryFabPricePrintOveride, conn))
                {
                    conn.Open();

                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if(!String.IsNullOrEmpty(reader.GetValue(0).ToString()))
                        {
                            printQuote.FabPricePrintOveride = true;
                            break;
                        }
                    }
                }
            }

            string qryInstallFab;
            if(!printQuote.FabPricePrintOveride)
            {
                string qrySlab = "SELECT [OnlineQuoteStoneID], [OnlineQuoteID], [SlabColorName], [SlabPrice], [Quantity], [Total] FROM [SummarySlabQry] WHERE [OnlineQuoteID] = " + OnlineQuoteID;
                using (OleDbConnection conn = new OleDbConnection(accdbConnection))
                {
                    using (OleDbCommand cmd = new OleDbCommand(qrySlab, conn))
                    {
                        conn.Open();

                        OleDbDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            PrintQuoteDetail slab = new PrintQuoteDetail();
                            slab.Name = reader.GetString(2);
                            if (blocklist.Contains(printQuote.Email))
                            {
                                Random rnd = new Random();
                                double fakefig = 1 + (double)rnd.Next(1, 51) / 100;
                                slab.Price = Convert.ToDouble(reader.GetValue(3)) * fakefig;
                                slab.Quantity = Convert.ToDouble(reader.GetValue(4));
                                slab.Total = slab.Price * slab.Quantity;

                            }
                            else
                            {
                                slab.Price = Convert.ToDouble(reader.GetValue(3));
                                slab.Quantity = Convert.ToDouble(reader.GetValue(4));
                                slab.Total = Convert.ToDouble(reader.GetValue(5));
                            }
                            printQuote.TotalSlab += slab.Total;
                            printQuote.Slab.Add(slab);
                        }
                    }
                }
                qryInstallFab = "SELECT OnlineQuoteStoneID, OnlineQuoteID, SlabColorName, SF,              CCur(WebsiteFabPrice) AS WebsiteFabPrice,     SFplus10, TotalG AS TOTAL FROM SummaryStoneQry WHERE OnlineQuoteID = " + OnlineQuoteID;
            }
            else
                qryInstallFab = "SELECT OnlineQuoteStoneID, OnlineQuoteID, SlabColorName, SFplus15 AS SF, CCur(FabPricePrintOveride) AS WebsiteFabPrice, SFplus15, Total2 AS TOTAL FROM SummaryStoneQry WHERE OnlineQuoteID = " + OnlineQuoteID;

            using (OleDbConnection conn = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(qryInstallFab, conn))
                {
                    conn.Open();

                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        PrintQuoteDetail installFab = new PrintQuoteDetail();
                        installFab.Name = reader.GetString(2);
                        if (blocklist.Contains(printQuote.Email))
                        {
                            Random rnd = new Random();
                            double fakefig = 1 + (double) rnd.Next(1, 101) / 100;
                            installFab.Quantity = Convert.ToDouble(reader.GetValue(3)) * fakefig;
                            installFab.Price = Convert.ToDouble(reader.GetValue(4));
                            installFab.Total = installFab.Quantity * installFab.Price;
                        }
                        else
                        {
                            installFab.Quantity = Convert.ToDouble(reader.GetValue(3));
                            installFab.Price = Convert.ToDouble(reader.GetValue(4));
                            installFab.Total = Convert.ToDouble(reader.GetValue(6));
                        }
                        printQuote.TotalInstallFab += installFab.Total;
                        printQuote.InstallFab.Add(installFab);
                    }
                }
            }

            string qrySink = "SELECT [OnlineQuoteSinkID], [OnlineQuoteID], [CatalogID], [SinkName], [Quantity], [Price], [Total] FROM [SummarySinksQry] WHERE [OnlineQuoteID] = " + OnlineQuoteID;
            using (OleDbConnection conn = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(qrySink, conn))
                {
                    conn.Open();

                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        PrintQuoteDetail sink = new PrintQuoteDetail();
                        sink.Catalog = reader.GetValue(2).ToString();
                        sink.Name = reader.GetString(3);
                        sink.Quantity = Convert.ToDouble(reader.GetValue(4));
                        sink.Price = Convert.ToDouble(reader.GetValue(5));
                        sink.Total = Convert.ToDouble(reader.GetValue(6));
                        printQuote.TotalSink += sink.Total;
                        printQuote.Sink.Add(sink);
                    }
                }
            }

            string qryServices = "SELECT OnlineQuoteServiceID, OnlineQuoteID, ServiceName, ServicePrice, IIF(ISNULL(Quantity), 1, Quantity) AS QTY, ServicePrice * QTY AS Total FROM SummaryServicesQry WHERE OnlineQuoteID = " + OnlineQuoteID;
            using (OleDbConnection conn = new OleDbConnection(accdbConnection))
            {
                using (OleDbCommand cmd = new OleDbCommand(qryServices, conn))
                {
                    conn.Open();

                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        PrintQuoteDetail serviceJ = new PrintQuoteDetail();
                        serviceJ.Name = reader.GetString(2);
                        serviceJ.Price = Convert.ToDouble(reader.GetValue(3));
                        serviceJ.Quantity = Convert.ToDouble(reader.GetValue(4));
                        serviceJ.Total = Convert.ToDouble(reader.GetValue(5));
                        printQuote.TotalService += serviceJ.Total;
                        printQuote.ServiceJ.Add(serviceJ);
                    }
                }
            }

            printQuote.TotalCost = printQuote.TotalSlab + printQuote.TotalInstallFab + printQuote.TotalSink + printQuote.TotalService;

            //string qryDeposit = "SELECT SUM(Amount) AS TAMOUNT FROM tblPayments WHERE OnlineQuoteID = " + OnlineQuoteID;
            //using (OleDbConnection conn = new OleDbConnection(accdbConnection))
            //{
            //    using (OleDbCommand cmd = new OleDbCommand(qryDeposit, conn))
            //    {
            //        conn.Open();
            //        printQuote.TotalDeposit = Convert.ToDouble(cmd.ExecuteScalar());                    
            //    }
            //}

            //printQuote.AmountDue = printQuote.TotalCost - printQuote.TotalDeposit;

            return printQuote;
        }
    }
}