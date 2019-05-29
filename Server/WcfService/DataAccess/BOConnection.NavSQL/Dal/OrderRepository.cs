﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using LSOmni.Common.Util;
using LSRetail.Omni.Domain.DataModel.Loyalty.Orders;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;

namespace LSOmni.DataAccess.BOConnection.NavSQL.Dal
{
    public class OrderRepository : BaseRepository
    {
        private string sql = string.Empty;

        public OrderRepository(Version navVersion) : base(navVersion)
        {
            if (navVersion > new Version("13.5"))
            {
                sql = "SELECT * FROM (" +
                        "SELECT mt.[Document ID],mt.[Receipt No_],mt.[Store No_],mt.[External ID],mt.[Document DateTime],mt.[Source Type],"+
                        "mt.[Member Card No_],mt.[Sales Order No_],mt.[Full Name],mt.[Address],mt.[Address 2]," +
                        "mt.[City],mt.[County],mt.[Post Code],mt.[Country_Region Code],mt.[Phone No_],mt.[Email],mt.[House_Apartment No_]," +
                        "mt.[Mobile Phone No_],mt.[Daytime Phone No_],mt.[Ship-to Full Name],mt.[Ship-to Address],mt.[Ship-to Address 2]," +
                        "mt.[Ship-to City],mt.[Ship-to County],mt.[Ship-to Post Code],mt.[Ship-to Country_Region Code],mt.[Ship-to Phone No_]," +
                        "mt.[Ship-to Email],mt.[Ship-to House_Apartment No_],mt.[Click and Collect Order], mt.[Shipping Agent Code]," +
                        "mt.[Shipping Agent Service Code], 0 AS Posted," +
                        "(SELECT COUNT(*) FROM [" + navCompanyName + "Customer Order Payment] cop WHERE cop.[Document ID]=mt.[Document ID]) AS CoPay " +
                        "FROM [" + navCompanyName + "Customer Order Header] mt " +
                        "UNION " +
                        "SELECT mt.[Document ID],mt.[Receipt No_],mt.[Store No_],mt.[External ID],mt.[Document DateTime],mt.[Source Type]," +
                        "mt.[Member Card No_],mt.[Sales Order No_],mt.[Full Name],mt.[Address],mt.[Address 2]," +
                        "mt.[City],mt.[County],mt.[Post Code],mt.[Country_Region Code],mt.[Phone No_],mt.[Email],mt.[House_Apartment No_]," +
                        "mt.[Mobile Phone No_],mt.[Daytime Phone No_],mt.[Ship-to Full Name],mt.[Ship-to Address],mt.[Ship-to Address 2]," +
                        "mt.[Ship-to City],mt.[Ship-to County],mt.[Ship-to Post Code],mt.[Ship-to Country_Region Code],mt.[Ship-to Phone No_]," +
                        "mt.[Ship-to Email],mt.[Ship-to House_Apartment No_],mt.[Click and Collect Order], mt.[Shipping Agent Code]," +
                        "mt.[Shipping Agent Service Code],1 AS Posted," +
                        "(SELECT COUNT(*) FROM [" + navCompanyName + "Posted Customer Order Payment] cop WHERE cop.[Document ID]=mt.[Document ID]) AS CoPay " +
                        "FROM [" + navCompanyName + "Posted Customer Order Header] mt) AS Orders";
            }
            else
            {
                sql = "SELECT * FROM (" +
                        "SELECT mt.[Document Id],mt.[Receipt No_],mt.[Store No_],mt.[Web Trans_ GUID],mt.[Document DateTime],mt.[Source Type]," +
                        "mt.[Member Card No_],mt.[Member Contact No_],mt.[Member Contact Name],mt.[Sales Order No_]," +
                        "mt.[Full Name],mt.[Address],mt.[Address 2],mt.[City],mt.[County],mt.[Post Code],mt.[Country Region Code]," +
                        "mt.[Phone No_],mt.[Email],mt.[House Apartment No_],mt.[Mobile Phone No_],mt.[Daytime Phone No_]," +
                        "mt.[Ship To Full Name],mt.[Ship To Address],mt.[Ship To Address 2],mt.[Ship To City],mt.[Ship To County],mt.[Ship To Post Code]," +
                        "mt.[Ship To Phone No_],mt.[Ship To Email],mt.[Ship To House Apartment No_],mt.[Ship To Country Region Code]," +
                        "mt.[Click And Collect Order],mt.[Anonymous Order],mt.[Shipping Agent Code],mt.[Shipping Agent Service Code]," +
                        "0 AS Posted," +
                        "(SELECT COUNT(*) FROM [" + navCompanyName + "Customer Order Payment] cop WHERE cop.[Document Id]=mt.[Document Id]) AS CoPay " +
                        "FROM [" + navCompanyName + "Customer Order Header] mt " +
                        "UNION " +
                        "SELECT mt.[Document Id],mt.[Receipt No_],mt.[Store No_],mt.[Web Trans_ GUID],mt.[Document DateTime],mt.[Source Type]," +
                        "mt.[Member Card No_],mt.[Member Contact No_],mt.[Member Contact Name],mt.[Sales Order No_]," +
                        "mt.[FullName],mt.[Address],mt.[Address2],mt.[City],mt.[County],mt.[PostCode],mt.[CountryRegionCode]," +
                        "mt.[PhoneNo],mt.[Email],mt.[HouseApartmentNo],mt.[MobilePhoneNo],mt.[DaytimePhoneNo]," +
                        "mt.[ShipToFullName],mt.[ShipToAddress],mt.[ShipToAddress2],mt.[ShipToCity],mt.[ShipToCounty],mt.[ShipToPostCode]," +
                        "mt.[ShipToPhoneNo],mt.[ShipToEmail],mt.[ShipToHouseApartmentNo],mt.[ShipToCountryRegionCode]," +
                        "mt.[ClickAndCollectOrder],mt.[AnonymousOrder],mt.[Shipping Agent Code],mt.[Shipping Agent Service Code]," +
                        "1 AS Posted," +
                        "(SELECT COUNT(*) FROM[" + navCompanyName + "Posted Customer Order Payment] cop WHERE cop.[Document Id]=mt.[Document Id]) AS CoPay " +
                        "FROM [" + navCompanyName + "Posted Customer Order Header] mt " +
                        ") AS Orders ";
            }
        }

        public Order OrderGetById(string id, bool includeLines)
        {
            Order order = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.Parameters.Clear();
                    command.CommandText = sql + (NavVersion > new Version("13.5") ? " WHERE [Document ID]=@id" : " WHERE [Document Id]=@id");
                    command.Parameters.AddWithValue("@id", id);
                    TraceSqlCommand(command);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            order = ReaderToOrder(reader, includeLines, false);
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
            return order;
        }

        public Order OrderGetByWebId(string id, bool includeLines)
        {
            Order order = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.Parameters.Clear();
                    command.CommandText = sql + (NavVersion > new Version("13.5") ? " WHERE [External ID]=@id" : " WHERE [Web Trans_ GUID]=@id");
                    command.Parameters.AddWithValue("@id", id.ToUpper());
                    TraceSqlCommand(command);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            order = ReaderToOrder(reader, includeLines, false);
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
            return order;
        }

        public Order OrderOldGetById(string id, string webId, bool includeLines)
        {
            Order order = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT [Order No_],[Store No_],[Web Transaction GUID],[Trans_ Date],[Source Type],[Member Card No_]," +
                                          "[Full Name],[Address],[Address 2],[City],[County],[Post Code],[Phone No_]," +
                                          "[Email],[Country_Region Code],[House_Apartment No_],[Mobile Phone No_],[Daytime Phone No_]," +
                                          "[Ship-to First Name],[Ship-to Last Name],[Ship-to Address 1],[Ship-to Address 2],[Ship-to City]," +
                                          "[Ship-to County],[Ship-to Post Code],[Ship-to Phone No_],[Ship-to Country_Region Code],[Ship-to House_Apartment No_]," +
                                          "[Order Collect],[Receipt No_],[POS Terminal No_],[Transaction No_] " +
                                          "FROM [" + navCompanyName + "Transaction Order Header]";

                    if (string.IsNullOrEmpty(webId))
                    {
                        command.CommandText += " WHERE [Order No_]=@id";
                    }
                    else
                    {
                        command.CommandText += " WHERE [Web Transaction GUID]=@id";
                        id = webId;
                    }

                    command.Parameters.AddWithValue("@id", id);
                    TraceSqlCommand(command);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            order = ReaderToOrderOld(reader, includeLines);
                        }
                        reader.Close();
                    }

                    if (order != null)
                    {
                        connection.Close();
                        return order;       // found in special orders
                    }

                    // check click and collect
                    command.Parameters.Clear();
                    command.CommandText = "SELECT mt.[Document Id],mt.[Store No_],mt.[Source Type],mt.[Collect Location],mt.[Web Trans_ GUID]," +
                                          "mt.[Document DateTime],mt.[Member Card No_],mt.[Member Contact No_],mt.[Member Contact Name] " +
                                          "FROM [" + navCompanyName + "Customer Order Header] mt " + (string.IsNullOrEmpty(webId) ? " WHERE [Document Id]=@id" : " WHERE [Web Trans_ GUID]=@id");
                    command.Parameters.AddWithValue("@id", id);
                    TraceSqlCommand(command);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            order = ReaderToOrder(reader, includeLines, true);
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
            return order;
        }

        private List<OrderLine> OrderOldLinesGet(string storeId, string termId, string id)
        {
            List<OrderLine> list = new List<OrderLine>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT [Line No_],[Item No_],[Variant Code],[Unit of Measure],[Price],[Net Price],[Quantity],[Discount Amount],[Net Amount],[VAT Amount] " +
                                          "FROM [" + navCompanyName + "Transaction Order Entry] WHERE [Store No_]=@sid AND [POS Terminal No_]=@tid AND [Transaction No_]=@id";

                    command.Parameters.AddWithValue("@sid", storeId);
                    command.Parameters.AddWithValue("@tid", termId);
                    command.Parameters.AddWithValue("@id", id);
                    TraceSqlCommand(command);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(ReaderToOrderOldLine(reader));
                        }
                    }
                }
                connection.Close();
            }
            return list;
        }

        public List<Order> OrderHistoryByCardId(string cardId, bool includeLines)
        {
            List<Order> list = new List<Order>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.Parameters.Clear();
                    command.CommandText = sql + " WHERE [Member Card No_]=@id ORDER BY " +
                        (NavVersion > new Version("13.5") ? "[Document ID] DESC" : "[Document Id] DESC");
                    command.Parameters.AddWithValue("@id", cardId);
                    TraceSqlCommand(command);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(ReaderToOrder(reader, includeLines, false));
                        }
                        reader.Close();
                    }
                }
                connection.Close();
            }
            return list;
        }

        private List<OrderLine> OrderLinesGet(string id)
        {
            string select = "SELECT ml.[Number],ml.[Variant Code],ml.[Unit of Measure Code],ml.[Line No_],ml.[Line Type]," +
                            "ml.[Net Price],ml.[Price],ml.[Quantity],ml.[Discount Amount],ml.[Discount Percent]," +
                            "ml.[Net Amount],ml.[Vat Amount],ml.[Amount],ml.[Item Description],ml.[Variant Description]" +
                            (NavVersion > new Version("13.5") ? ",ml.[Document ID]" : ",ml.[Document Id]");

            List<OrderLine> list = new List<OrderLine>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM ( " + select + " FROM [" + navCompanyName + "Customer Order Line] ml " +
                                            "UNION " + select + " FROM [" + navCompanyName + "Posted Customer Order Line] ml " +
                                          ") AS OrderLines WHERE " +
                                          (NavVersion > new Version("13.5") ? "[Document ID]=@id" : "[Document Id]=@id") +
                                          " ORDER BY [Line No_]";

                    command.Parameters.AddWithValue("@id", id);
                    TraceSqlCommand(command);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(ReaderToOrderLine(reader));
                        }
                    }
                }
                connection.Close();
            }
            return list;
        }

        private void OrderLinesGetTotals(string orderId, out int itemCount, out decimal totalAmount, out decimal totalNetAmount, out decimal totalDiscount)
        {
            itemCount = 0;
            totalAmount = 0;
            totalNetAmount = 0;
            totalDiscount = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string select = "SELECT [Document Id], SUM([Quantity]) AS Cnt, SUM([Discount Amount]) AS Disc, SUM([Net Amount]) AS NAmt, SUM([Amount]) AS Amt";

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM (" + select +
                                            " FROM [" + navCompanyName + "Customer Order Line] GROUP BY [Document Id] " + 
                                            "UNION "+ select +
                                            " FROM [" + navCompanyName + "Posted Customer Order Line] GROUP BY [Document Id] " + 
                                            ") AS OrderTotals WHERE [Document Id]=@id";
                    command.Parameters.AddWithValue("@id", orderId);
                    TraceSqlCommand(command);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            itemCount = SQLHelper.GetInt32(reader["Cnt"]);
                            totalAmount = SQLHelper.GetDecimal(reader, "Amt");
                            totalNetAmount = SQLHelper.GetDecimal(reader, "NAmt");
                            totalDiscount = SQLHelper.GetDecimal(reader, "Disc");
                        }
                    }
                }
                connection.Close();
            }
        }

        private void OrderV2LinesGetTotals(string orderId, out int itemCount, out decimal totalAmount, out decimal totalNetAmount, out decimal totalDiscount)
        {
            itemCount = 0;
            totalAmount = 0;
            totalNetAmount = 0;
            totalDiscount = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string select = "SELECT [Document ID], SUM([Quantity]) AS Cnt, SUM([Discount Amount]) AS Disc, SUM([Net Amount]) AS NAmt, SUM([Amount]) AS Amt";

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM (" + select +
                                            " FROM [" + navCompanyName + "Customer Order Line] GROUP BY [Document ID] " +
                                            "UNION " + select +
                                            " FROM [" + navCompanyName + "Posted Customer Order Line] GROUP BY [Document ID] " +
                                            ") AS OrderTotals WHERE [Document ID]=@id";
                    command.Parameters.AddWithValue("@id", orderId);
                    TraceSqlCommand(command);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            itemCount = SQLHelper.GetInt32(reader["Cnt"]);
                            totalAmount = SQLHelper.GetDecimal(reader, "Amt");
                            totalNetAmount = SQLHelper.GetDecimal(reader, "NAmt");
                            totalDiscount = SQLHelper.GetDecimal(reader, "Disc");
                        }
                    }
                }
                connection.Close();
            }
        }

        public void OrderPointsGetTotal(string orderId, out decimal rewarded, out decimal used)
        {
            rewarded = 0;
            used = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    //Awarded points are linked to Sales Invoice Id
                    command.CommandText = "SELECT [No_] FROM [" + navCompanyName + "Sales Invoice Header] Where [External Document No_]=@id";
                    command.Parameters.AddWithValue("@id", orderId);
                    TraceSqlCommand(command);
                    logger.Log(NLog.LogLevel.Info,"ORDERID: " + orderId);
                    connection.Open();
                    string salesId = (string)command.ExecuteScalar();

                    if (string.IsNullOrEmpty(salesId))
                    {
                        //Get rewarded points with SalesInvoice id
                        command.CommandText = "SELECT [Points] FROM [" + navCompanyName + "Member Point Entry] WHERE [Document No_]=@id AND [Entry Type]=0";
                        TraceSqlCommand(command);
                        var res1 = command.ExecuteScalar();
                        rewarded = res1 == null ? 0 : (decimal)res1;

                        command.CommandText = "SELECT [Points] FROM [" + navCompanyName + "Member Point Entry] WHERE [Document No_]=@id AND [Entry Type]=1";
                        TraceSqlCommand(command);
                        res1 = command.ExecuteScalar();
                        used = res1 == null ? 0 : -(decimal)res1;
                        return;
                    }

                    //Get Used points with orderId
                    command.CommandText = "SELECT [Points] FROM [" + navCompanyName + "Member Point Entry] WHERE [Document No_]=@id";
                    TraceSqlCommand(command);
                    var res = command.ExecuteScalar();
                    used = res == null ? 0 : -(decimal)res; //use '-' to convert to positive number

                    //Get rewarded points with SalesInvoice id
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@id", salesId);
                    TraceSqlCommand(command);
                    res = command.ExecuteScalar();
                    rewarded = res == null ? 0 : (decimal)res;
                }
                connection.Close();
            }
        }

        private List<OrderPayment> OrderPayGet(string id)
        {
            List<OrderPayment> list = new List<OrderPayment>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string select = "SELECT ml.[Store No_],ml.[Line No_],ml.[Pre Approved Amount],ml.[Finalised Amount],ml.[Tender Type]," +
                                "ml.[Card Type],ml.[Currency Code],ml.[Currency Factor],ml.[Authorisation Code],ml.[Pre Approved Valid Date],ml.[Card or Customer number]" +
                                (NavVersion > new Version("13.5") ? ",ml.[Document ID]" : ",ml.[Document Id]");

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM (" + select +
                                          " FROM [" + navCompanyName + "Customer Order Payment] ml " +
                                          " UNION " + select + " FROM [" + navCompanyName + "Posted Customer Order Payment] ml " +
                                          ") AS OrderTotal WHERE " +
                                          (NavVersion > new Version("13.5") ? "[Document ID]=@id" : "[Document Id]=@id");

                    command.Parameters.AddWithValue("@id", id);
                    TraceSqlCommand(command);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(ReaderToOrderPay(reader));
                        }
                    }
                }
                connection.Close();
            }
            return list;
        }

        private List<OrderDiscountLine> OrderDiscGet(string id)
        {
            List<OrderDiscountLine> list = new List<OrderDiscountLine>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string select = "SELECT ml.[Line No_],ml.[Entry No_],ml.[Discount Type],ml.[Offer No_]," +
                                "ml.[Periodic Disc_ Type],ml.[Periodic Disc_ Group],ml.[Description],ml.[Discount Percent],ml.[Discount Amount]" +
                                (NavVersion > new Version("13.5") ? ",ml.[Document ID]" : ",ml.[Document Id]");

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM (" + select + " FROM [" + navCompanyName + "Customer Order Discount Line] ml " + 
                                          "UNION " + select + " FROM [" + navCompanyName + "Posted CO Discount Line] ml " +
                                          ") AS OrderDiscounts WHERE " +
                                          (NavVersion > new Version("13.5") ? "[Document ID]=@id" : "[Document Id]=@id");

                    command.Parameters.AddWithValue("@id", id);
                    TraceSqlCommand(command);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(ReaderToOrderDisc(reader));
                        }
                    }
                }
                connection.Close();
            }
            return list;
        }

        private Order ReaderToOrder(SqlDataReader reader, bool includeLines, bool oldNav)
        {
            if (NavVersion > new Version("13.5"))
                return ReaderToOrderV2(reader, includeLines);

            Order order = new Order
            {
                DocumentId = SQLHelper.GetString(reader["Document Id"]),
                StoreId = SQLHelper.GetString(reader["Store No_"]),
                Id = SQLHelper.GetString(reader["Web Trans_ GUID"]),
                DocumentRegTime = SQLHelper.GetDateTime(reader["Document DateTime"]),
                SourceType = (SourceType)SQLHelper.GetInt32(reader["Source Type"]),
                ContactName = SQLHelper.GetString(reader["Member Contact Name"]),
                CardId = SQLHelper.GetString(reader["Member Card No_"]),
                ContactId = SQLHelper.GetString(reader["Member Contact No_"]),
                OrderStatus = OrderStatus.Created
            };

            if (oldNav)
            {
                order.ClickAndCollectOrder = true;
                order.AnonymousOrder = true;
                if (includeLines)
                {
                    order.OrderLines = OrderLinesGet(order.DocumentId);
                }
                return order;
            }

            order.ReceiptNo = SQLHelper.GetString(reader["Receipt No_"]);
            order.Posted = SQLHelper.GetBool(reader["Posted"]);
            order.ClickAndCollectOrder = SQLHelper.GetBool(reader["Click And Collect Order"]);
            order.AnonymousOrder = SQLHelper.GetBool(reader["Anonymous Order"]);

            order.PhoneNumber = SQLHelper.GetString(reader["Phone No_"]);
            order.Email = SQLHelper.GetString(reader["Email"]);
            order.MobileNumber = SQLHelper.GetString(reader["Mobile Phone No_"]);
            order.DayPhoneNumber = SQLHelper.GetString(reader["Daytime Phone No_"]);

            order.ShipToName = SQLHelper.GetString(reader["Ship To Full Name"]);
            order.ShipToPhoneNumber = SQLHelper.GetString(reader["Ship To Phone No_"]);
            order.ShipToEmail = SQLHelper.GetString(reader["Ship To Email"]);

            order.ShippingAgentCode = SQLHelper.GetString(reader["Shipping Agent Code"]);
            order.ShippingAgentServiceCode = SQLHelper.GetString(reader["Shipping Agent Service Code"]);

            order.ContactAddress = new Address()
            {
                Address1 = SQLHelper.GetString(reader["Address"]),
                Address2 = SQLHelper.GetString(reader["Address 2"]),
                HouseNo = SQLHelper.GetString(reader["House Apartment No_"]),
                City = SQLHelper.GetString(reader["City"]),
                StateProvinceRegion = SQLHelper.GetString(reader["County"]),
                PostCode = SQLHelper.GetString(reader["Post Code"]),
                Country = SQLHelper.GetString(reader["Country Region Code"])
            };

            order.ShipToAddress = new Address()
            {
                Address1 = SQLHelper.GetString(reader["Ship To Address"]),
                Address2 = SQLHelper.GetString(reader["Ship To Address 2"]),
                HouseNo = SQLHelper.GetString(reader["Ship To House Apartment No_"]),
                City = SQLHelper.GetString(reader["Ship To City"]),
                StateProvinceRegion = SQLHelper.GetString(reader["Ship To County"]),
                PostCode = SQLHelper.GetString(reader["Ship To Post Code"]),
                Country = SQLHelper.GetString(reader["Ship To Country Region Code"])
            };

            int copay = SQLHelper.GetInt32(reader["CoPay"]);
            order.PaymentStatus = (copay > 0) ? PaymentStatus.PreApproved : PaymentStatus.Approved;
            order.ShippingStatus = (order.ClickAndCollectOrder) ? ShippingStatus.ShippigNotRequired : ShippingStatus.NotYetShipped;

            OrderLinesGetTotals(order.DocumentId, out int cnt, out decimal amt, out decimal namt, out decimal disc);
            order.LineItemCount = cnt;
            order.TotalAmount = amt;
            order.TotalNetAmount = namt;
            order.TotalDiscount = disc;

            if (order.Posted)
            {
                string sorderNo = SQLHelper.GetString(reader["Sales Order No_"]);
                order.OrderStatus = OrderStatus.Complete;
                if (string.IsNullOrEmpty(sorderNo) == false)
                {
                    // we just use the data from the sales order for posted orders
                    int status = SaleOrderGetStatus(sorderNo);
                    if (status == 0)
                    {
                        order.OrderStatus = OrderStatus.Pending;
                        order.ShippingStatus = ShippingStatus.NotYetShipped;
                    }
                    else
                    {
                        order.OrderStatus = OrderStatus.Complete;
                    }
                }

                OrderPointsGetTotal(order.DocumentId, out decimal rewarded, out decimal used);
                order.PointsRewarded = rewarded;
                order.PointsUsedInOrder = used;
            }

            if (includeLines)
            {
                order.OrderLines = OrderLinesGet(order.DocumentId);
                order.OrderPayments = OrderPayGet(order.DocumentId);
                order.OrderDiscountLines = OrderDiscGet(order.DocumentId);

                List<OrderLine> list = new List<OrderLine>();
                foreach (OrderLine line in order.OrderLines)
                {
                    OrderLine exline = list.Find(l => l.Id.Equals(line.Id) && l.ItemId.Equals(line.ItemId) && l.VariantId.Equals(line.VariantId) && l.UomId.Equals(line.UomId));
                    if (exline == null)
                    {
                        list.Add(line);
                        continue;
                    }

                    OrderDiscountLine dline = order.OrderDiscountLines.Find(l => l.LineNumber >= line.LineNumber && l.LineNumber < line.LineNumber + 10000);
                    if (dline != null)
                    {
                        // update discount line number to match existing record, as we will sum up the orderlines
                        dline.LineNumber = exline.LineNumber + dline.LineNumber / 100;
                    }

                    exline.Amount += line.Amount;
                    exline.NetAmount += line.NetAmount;
                    exline.DiscountAmount += line.DiscountAmount;
                    exline.TaxAmount += line.TaxAmount;
                    exline.Quantity += line.Quantity;
                }
                order.OrderLines = list;
            }
            return order;
        }

        private Order ReaderToOrderV2(SqlDataReader reader, bool includeLines)
        {
            Order order = new Order
            {
                DocumentId = SQLHelper.GetString(reader["Document ID"]),
                ReceiptNo = SQLHelper.GetString(reader["Receipt No_"]),
                StoreId = SQLHelper.GetString(reader["Store No_"]),
                Id = SQLHelper.GetString(reader["External ID"]),
                DocumentRegTime = SQLHelper.GetDateTime(reader["Document DateTime"]),
                SourceType = (SourceType)SQLHelper.GetInt32(reader["Source Type"]),
                CardId = SQLHelper.GetString(reader["Member Card No_"]),
                OrderStatus = OrderStatus.Created,
                Posted = SQLHelper.GetBool(reader["Posted"]),
                ClickAndCollectOrder = SQLHelper.GetBool(reader["Click and Collect Order"]),
                PhoneNumber = SQLHelper.GetString(reader["Phone No_"]),
                Email = SQLHelper.GetString(reader["Email"]),
                MobileNumber = SQLHelper.GetString(reader["Mobile Phone No_"]),
                DayPhoneNumber = SQLHelper.GetString(reader["Daytime Phone No_"]),
                ShipToName = SQLHelper.GetString(reader["Ship-to Full Name"]),
                ShipToPhoneNumber = SQLHelper.GetString(reader["Ship-to Phone No_"]),
                ShipToEmail = SQLHelper.GetString(reader["Ship-to Email"]),
                ShippingAgentCode = SQLHelper.GetString(reader["Shipping Agent Code"]),
                ShippingAgentServiceCode = SQLHelper.GetString(reader["Shipping Agent Service Code"])
            };

            order.ContactAddress = new Address()
            {
                Address1 = SQLHelper.GetString(reader["Address"]),
                Address2 = SQLHelper.GetString(reader["Address 2"]),
                HouseNo = SQLHelper.GetString(reader["House_Apartment No_"]),
                City = SQLHelper.GetString(reader["City"]),
                StateProvinceRegion = SQLHelper.GetString(reader["County"]),
                PostCode = SQLHelper.GetString(reader["Post Code"]),
                Country = SQLHelper.GetString(reader["Country_Region Code"])
            };

            order.ShipToAddress = new Address()
            {
                Address1 = SQLHelper.GetString(reader["Ship-to Address"]),
                Address2 = SQLHelper.GetString(reader["Ship-to Address 2"]),
                HouseNo = SQLHelper.GetString(reader["Ship-to House_Apartment No_"]),
                City = SQLHelper.GetString(reader["Ship-to City"]),
                StateProvinceRegion = SQLHelper.GetString(reader["Ship-to County"]),
                PostCode = SQLHelper.GetString(reader["Ship-to Post Code"]),
                Country = SQLHelper.GetString(reader["Ship-to Country_Region Code"])
            };

            int copay = SQLHelper.GetInt32(reader["CoPay"]);
            order.PaymentStatus = (copay > 0) ? PaymentStatus.PreApproved : PaymentStatus.Approved;
            order.ShippingStatus = (order.ClickAndCollectOrder) ? ShippingStatus.ShippigNotRequired : ShippingStatus.NotYetShipped;

            OrderV2LinesGetTotals(order.DocumentId, out int cnt, out decimal amt, out decimal namt, out decimal disc);
            order.LineItemCount = cnt;
            order.TotalAmount = amt;
            order.TotalNetAmount = namt;
            order.TotalDiscount = disc;

            if (order.Posted)
            {
                string sorderNo = SQLHelper.GetString(reader["Sales Order No_"]);
                order.OrderStatus = OrderStatus.Complete;
                if (string.IsNullOrEmpty(sorderNo) == false)
                {
                    // we just use the data from the sales order for posted orders
                    int status = SaleOrderGetStatus(sorderNo);
                    if (status == 0)
                    {
                        order.OrderStatus = OrderStatus.Pending;
                        order.ShippingStatus = ShippingStatus.NotYetShipped;
                    }
                    else
                    {
                        order.OrderStatus = OrderStatus.Complete;
                    }
                }

                OrderPointsGetTotal(order.DocumentId, out decimal rewarded, out decimal used);
                order.PointsRewarded = rewarded;
                order.PointsUsedInOrder = used;
            }

            if (includeLines)
            {
                order.OrderLines = OrderLinesGet(order.DocumentId);
                order.OrderPayments = OrderPayGet(order.DocumentId);
                order.OrderDiscountLines = OrderDiscGet(order.DocumentId);

                List<OrderLine> list = new List<OrderLine>();
                foreach (OrderLine line in order.OrderLines)
                {
                    OrderLine exline = list.Find(l => l.Id.Equals(line.Id) && l.ItemId.Equals(line.ItemId) && l.VariantId.Equals(line.VariantId) && l.UomId.Equals(line.UomId));
                    if (exline == null)
                    {
                        list.Add(line);
                        continue;
                    }

                    OrderDiscountLine dline = order.OrderDiscountLines.Find(l => l.LineNumber >= line.LineNumber && l.LineNumber < line.LineNumber + 10000);
                    if (dline != null)
                    {
                        // update discount line number to match existing record, as we will sum up the orderlines
                        dline.LineNumber = exline.LineNumber + dline.LineNumber / 100;
                    }

                    exline.Amount += line.Amount;
                    exline.NetAmount += line.NetAmount;
                    exline.DiscountAmount += line.DiscountAmount;
                    exline.TaxAmount += line.TaxAmount;
                    exline.Quantity += line.Quantity;
                }
                order.OrderLines = list;
            }
            return order;
        }

        private OrderLine ReaderToOrderLine(SqlDataReader reader)
        {
            return new OrderLine()
            {
                VariantId = SQLHelper.GetString(reader["Variant Code"]),
                UomId = SQLHelper.GetString(reader["Unit of Measure Code"]),
                Quantity = SQLHelper.GetDecimal(reader, "Quantity"),
                LineNumber = SQLHelper.GetInt32(reader["Line No_"]),
                LineType = (LineType)SQLHelper.GetInt32(reader["Line Type"]),
                ItemId = SQLHelper.GetString(reader["Number"]),
                NetPrice = SQLHelper.GetDecimal(reader, "Net Price"),
                Price = SQLHelper.GetDecimal(reader, "Price"),
                DiscountAmount = SQLHelper.GetDecimal(reader, "Discount Amount"),
                DiscountPercent = SQLHelper.GetDecimal(reader, "Discount Percent"),
                NetAmount = SQLHelper.GetDecimal(reader, "Net Amount"),
                TaxAmount = SQLHelper.GetDecimal(reader, "Vat Amount"),
                Amount = SQLHelper.GetDecimal(reader, "Amount"),
                ItemDescription = SQLHelper.GetString(reader["Item Description"]),
                VariantDescription = SQLHelper.GetString(reader["Variant Description"])
            };
        }

        private OrderPayment ReaderToOrderPay(SqlDataReader reader)
        {
            return new OrderPayment()
            {
                LineNumber = SQLHelper.GetInt32(reader["Line No_"]),
                TenderType = SQLHelper.GetString(reader["Tender Type"]),
                AuthorisationCode = SQLHelper.GetString(reader["Authorisation Code"]),
                CardNumber = SQLHelper.GetString(reader["Card or Customer number"]),
                CardType = SQLHelper.GetString(reader["Card Type"]),
                CurrencyCode = SQLHelper.GetString(reader["Currency Code"]),
                CurrencyFactor = SQLHelper.GetDecimal(reader, "Currency Factor"),
                FinalizedAmount = SQLHelper.GetDecimal(reader, "Finalised Amount"),
                PreApprovedAmount = SQLHelper.GetDecimal(reader, "Pre Approved Amount"),
                PreApprovedValidDate = SQLHelper.GetDateTime(reader["Pre Approved Valid Date"])
            };
        }

        private OrderDiscountLine ReaderToOrderDisc(SqlDataReader reader)
        {
            return new OrderDiscountLine()
            {
                LineNumber = SQLHelper.GetInt32(reader["Line No_"]),
                Description = SQLHelper.GetString(reader["Description"]),
                No = SQLHelper.GetString(reader["Entry No_"]),
                OfferNumber = SQLHelper.GetString(reader["Offer No_"]),
                DiscountAmount = SQLHelper.GetDecimal(reader, "Discount Amount"),
                DiscountPercent = SQLHelper.GetDecimal(reader, "Discount Percent"),
                DiscountType = (DiscountType)SQLHelper.GetInt32(reader["Discount Type"]),
                PeriodicDiscGroup = SQLHelper.GetString(reader["Periodic Disc_ Group"]),
                PeriodicDiscType = (PeriodicDiscType)SQLHelper.GetInt32(reader["Periodic Disc_ Type"])
            };
        }
		
        private Order ReaderToOrderOld(SqlDataReader reader, bool includeLines)
        {
            Order order = new Order
            {
                Posted = true,

                DocumentId = SQLHelper.GetString(reader["Order No_"]),
                StoreId = SQLHelper.GetString(reader["Store No_"]),
                Id = SQLHelper.GetString(reader["Web Transaction GUID"]),
                DocumentRegTime = SQLHelper.GetDateTime(reader["Trans_ Date"]),
                SourceType = (SourceType)SQLHelper.GetInt32(reader["Source Type"]),
                ClickAndCollectOrder = SQLHelper.GetBool(reader["Order Collect"]),
                AnonymousOrder = false,

                ReceiptNo = SQLHelper.GetString(reader["Receipt No_"]),
                TransStore = SQLHelper.GetString(reader["Store No_"]),
                TransTerminal = SQLHelper.GetString(reader["POS Terminal No_"]),
                TransId = SQLHelper.GetString(reader["Transaction No_"]),
                CardId = SQLHelper.GetString(reader["Member Card No_"]),
                ContactName = SQLHelper.GetString(reader["Full Name"]),

                PhoneNumber = SQLHelper.GetString(reader["Phone No_"]),
                Email = SQLHelper.GetString(reader["Email"]),
                MobileNumber = SQLHelper.GetString(reader["Mobile Phone No_"]),
                DayPhoneNumber = SQLHelper.GetString(reader["Daytime Phone No_"]),

                ShipToName = SQLHelper.GetString(reader["Ship-to First Name"]) + " " + SQLHelper.GetString(reader["Ship-to Last Name"]),
                ShipToPhoneNumber = SQLHelper.GetString(reader["Ship-to Phone No_"]),

                ContactAddress = new Address
                {
                    Address1 = SQLHelper.GetString(reader["Address"]),
                    Address2 = SQLHelper.GetString(reader["Address 2"]),
                    HouseNo = SQLHelper.GetString(reader["House_Apartment No_"]),
                    City = SQLHelper.GetString(reader["City"]),
                    StateProvinceRegion = SQLHelper.GetString(reader["County"]),
                    PostCode = SQLHelper.GetString(reader["Post Code"]),
                    Country = SQLHelper.GetString(reader["Country_Region Code"])
                },

                ShipToAddress = new Address
                {
                    Address1 = SQLHelper.GetString(reader["Ship-to Address 1"]),
                    Address2 = SQLHelper.GetString(reader["Ship-to Address 2"]),
                    HouseNo = SQLHelper.GetString(reader["Ship-to House_Apartment No_"]),
                    City = SQLHelper.GetString(reader["Ship-to City"]),
                    StateProvinceRegion = SQLHelper.GetString(reader["Ship-to County"]),
                    PostCode = SQLHelper.GetString(reader["Ship-to Post Code"]),
                    Country = SQLHelper.GetString(reader["Ship-to Country_Region Code"])
                }
            };

            if (includeLines)
            {
                order.OrderLines = OrderOldLinesGet(order.StoreId, order.TransTerminal, order.TransId);
            }
            return order;
        }

        private OrderLine ReaderToOrderOldLine(SqlDataReader reader)
        {
            OrderLine line = new OrderLine
            {
                LineNumber = SQLHelper.GetInt32(reader["Line No_"]),
                VariantId = SQLHelper.GetString(reader["Variant Code"]),
                UomId = SQLHelper.GetString(reader["Unit of Measure"]),
                Quantity = SQLHelper.GetDecimal(reader, "Quantity"),

                LineType = LineType.Item,
                ItemId = SQLHelper.GetString(reader["Item No_"]),
                NetPrice = SQLHelper.GetDecimal(reader, "Net Price"),
                Price = SQLHelper.GetDecimal(reader, "Price"),
                DiscountAmount = SQLHelper.GetDecimal(reader, "Discount Amount"),
                NetAmount = SQLHelper.GetDecimal(reader, "Net Amount"),
                TaxAmount = SQLHelper.GetDecimal(reader, "VAT Amount")
            };

            line.Amount = line.NetAmount + line.TaxAmount;
            return line;
        }

        private int SaleOrderGetStatus(string id)
        {
            int status = 1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT [Status] FROM [" + navCompanyName + "Sales Header] WHERE [No_]=@id";
                    command.Parameters.AddWithValue("@id", id);
                    TraceSqlCommand(command);
                    connection.Open();
                    var value = command.ExecuteScalar();
                    if (value != null)
                        status = (int)value;
                }
                connection.Close();
            }
            return status;
        }
    }
}
