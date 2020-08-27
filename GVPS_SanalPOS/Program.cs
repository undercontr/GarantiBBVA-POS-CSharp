using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace GVPS_SanalPOS
{
    class Program
    {
        static void Main(string[] args)
        {
            GarantiPOS pos = new GarantiPOS();

            pos.UserID = "ada";
            pos.CurrencyCode = "949";
            pos.MotoInd = "N";

            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                { "IPAddress", "12.12.12.12" },
                { "EmailAddress", "email@email.com" },
                { "OrderID", pos.GenerateOrderID() },
                { "CardNumber", "5406697543211173" },
                { "CardExpiryDate", "0323" },
                { "CardCVV2", "465" },
                { "TransactionAmount", "1000" }
            };

            pos.SetParameters(dict);

            XElement result = pos.Pay();

            Console.WriteLine(result);
            Console.ReadLine();

        }
    }
}
