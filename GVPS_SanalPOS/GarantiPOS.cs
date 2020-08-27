using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace GVPS_SanalPOS
{
	public class GarantiPOS
	{
		public GarantiPOS()
		{
			this.TerminalID_ = "0" + this.TerminalID;
		}

		private string mode = "TEST";

		public string Mode
		{
			get { return mode; }
			set { mode = value; }
		}


		private string ChannelCode = "S";

		private string Version = "v0.01";

		private string ProvUserID = "PROVAUT";

		private string ProvUserPassword = "123qweASD/";

		private string SecureKey = "12345678";

		private string hashData;

		public string HashData
		{
			get { return hashData; }
			set { hashData = value; }
		}

		private string userID;

		public string UserID
		{
			get { return userID; }
			set { userID = value; }
		}

		private string TerminalID = "30691297";

		private string TerminalID_;

		private string MerchantID = "7000679";

		private string remoteIP;

		public string RemoteIP
		{
			get { return remoteIP; }
			set { remoteIP = value; }
		}

		private string emailAddress;

		public string EmailAddress
		{
			get { return emailAddress; }
			set { emailAddress = value; }
		}

		private string orderID;

		public string OrderID
		{
			get { return orderID; }
			set { orderID = value; }
		}

		private string cardNumber;

		public string CardNumber
		{
			get { return cardNumber; }
			set { cardNumber = value; }
		}

		private string cardExpiryDate;

		public string CardExpiryDate
		{
			get { return cardExpiryDate; }
			set { cardExpiryDate = value; }
		}

		private string cardCVV2;

		public string CardCVV2
		{
			get { return cardCVV2; }
			set { cardCVV2 = value; }
		}

		private string transactionType = "sales";

		public string TransactionType
		{
			get { return transactionType; }
			set { transactionType = value; }
		}

		private string transactionAmount;

		public string TransactionAmount
		{
			get { return transactionAmount; }
			set { transactionAmount = value; }
		}

		private string currencyCode;

		public string CurrencyCode
		{
			get { return currencyCode; }
			set { currencyCode = value; }
		}

		private string motoInd;

		public string MotoInd
		{
			get { return motoInd; }
			set { motoInd = value; }
		}

		// BIN Query information

		private string cardScheme;

		public string CardScheme
		{
			get { return cardScheme; }
			set { cardScheme = value; }
		}

		private string bankName;

		public string BankName
		{
			get { return bankName; }
			set { bankName = value; }
		}

		private string bankURL;

		public string BankURL
		{
			get { return bankURL; }
			set { bankURL = value; }
		}

		private string cardCountry;

		public string CardCountry
		{
			get { return cardCountry; }
			set { cardCountry = value; }
		}

		private string cardCountryCode;

		public string CardCountryCode
		{
			get { return cardCountryCode; }
			set { cardCountryCode = value; }
		}

		// 3D Fields

		private string successURL;

		public string SuccessURL
		{
			get { return successURL; }
			set { successURL = value; }
		}

		private string errorURL;

		public string ErrorURL
		{
			get { return errorURL; }
			set { errorURL = value; }
		}

		public string ProvisionURL = "https://sanalposprov.garanti.com.tr/VPServlet";

		public string ProvisionTestURL = "https://sanalposprovtest.garanti.com.tr/VPServlet";

		public string Provision3DURL = "https://sanalposprov.garanti.com.tr/servlet/gt3dengine";

		public string Provision3DTestURL = "https://sanalposprov.garanti.com.tr/servlet/gt3dengine";

		public void SetParameters(Dictionary<string, string> param)
		{
			this.RemoteIP = param["IPAddress"];
			this.EmailAddress = param["EmailAddress"];
			this.OrderID = param["OrderID"];
			this.CardNumber = param["CardNumber"];
			this.CardExpiryDate = param["CardExpiryDate"];
			this.CardCVV2 = param["CardCVV2"];
			this.TransactionAmount = param["TransactionAmount"];

			using (SHA1 enc = new SHA1CryptoServiceProvider())
			{
				byte[] keySecurity = enc.ComputeHash(Encoding.UTF8.GetBytes(this.ProvUserPassword + this.TerminalID_));

				string keySecStr = string.Concat(keySecurity.Select(b => b.ToString("X2")));

				byte[] keyHash = enc.ComputeHash(Encoding.UTF8.GetBytes(this.OrderID + this.TerminalID + this.CardNumber + this.TransactionAmount + keySecStr));

				string keyHashStr = string.Concat(keyHash.Select(b => b.ToString("X2")));

				this.HashData = keyHashStr;

				param["HashData"] = this.HashData;

			}

			JObject binQuery = this.BINQuery();

			this.BankName = (string)binQuery["bank"]["name"];
			this.CardScheme = (string)binQuery["scheme"];
			this.BankURL = (string)binQuery["bank"]["url"];
			this.CardCountry = (string)binQuery["country"]["name"];
			this.CardCountryCode = (string)binQuery["bank"]["name"];


		}

		public XElement GetXML()
		{
			XElement xml = new XElement("GVPSRequest",
				new XElement("Mode", this.Mode),
				new XElement("ChannelCode", this.ChannelCode),
				new XElement("Version", this.Version),
				new XElement("Terminal",
					new XElement("ProvUserID", this.ProvUserID),
					new XElement("HashData", this.HashData),
					new XElement("UserID", this.UserID),
					new XElement("ID", this.TerminalID),
					new XElement("MerchantID", this.MerchantID)),
				new XElement("Customer",
					new XElement("IPAddress", this.RemoteIP),
					new XElement("EmailAddress", this.EmailAddress)),
				new XElement("Order",
					new XElement("OrderID", this.OrderID)),
				new XElement("Card",
					new XElement("Number", this.CardNumber),
					new XElement("ExpireDate", this.CardExpiryDate),
					new XElement("CVV2", this.CardCVV2)),
				new XElement("Transaction",
					new XElement("Type", this.TransactionType),
					new XElement("Amount", this.TransactionAmount),
					new XElement("CurrencyCode", this.CurrencyCode),
					new XElement("MotoInd", this.MotoInd)));

			return xml;

		}

		public JObject BINQuery()
		{
			string cardNum = this.CardNumber.Substring(0, 6);

			var client = new RestClient();

			client.BaseUrl = new Uri("https://lookup.binlist.net/" + cardNum);

			var request = new RestRequest(Method.POST);

			IRestResponse response = client.Execute(request);

			JObject json = JObject.Parse(response.Content);

			return json;
		}

		public XElement Pay()
		{
			var client = new RestClient();

			client.BaseUrl = new Uri(this.ProvisionTestURL);

			var request = new RestRequest(Method.POST);

			request.AddParameter("application/xml", this.GetXML(), ParameterType.RequestBody);

			IRestResponse response = client.Execute(request);

			XElement xmlDoc = XElement.Parse(response.Content);

			return xmlDoc;
		}

	}
}
