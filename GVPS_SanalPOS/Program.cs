using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GVPS_SanalPOS
{
    class Program
    {
        static void Main(string[] args)
        {
            GarantiPOS en = new GarantiPOS();

            en.UserID = "ada";
            en.CurrencyCode = "949";
            en.MotoInd = "N";

            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                { "IPAddress", "12.12.12.12" },
                { "EmailAddress", "email@email.com" },
                { "OrderID", "IN123141" },
                { "CardNumber", "5406697543211173" },
                { "CardExpiryDate", "0323" },
                { "CardCVV2", "465" },
                { "TransactionAmount", "1000" }
            };

            en.SetParameters(dict);

            Console.WriteLine(en.Pay());
            Console.ReadLine();

        }
    }
}
