using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace GVPS_SanalPOS
{
	public class GarantiPOS // GarantiBBVA için Sanal POS kütüphanesi
	{
		public GarantiPOS() // HashData için TerminalID başı 0 ile başlamalı 9 digit olmalı
		{
			this.TerminalID_ = "0" + this.TerminalID;
		}

		private string mode = "TEST"; // PROD yada TEST

		public string Mode
		{
			get { return mode; }
			set { mode = value; }
		}


		private string ChannelCode = "S"; // sabit

		private string Version = "v0.01"; // sabit

		private string ProvUserID = "PROVAUT"; // Satış işlemi için provision kullanıcısı

		private string ProvUserPassword = "123qweASD/"; // provizyon şifresi

		private string SecureKey = "12345678"; // 3D için gerekli olan HashData'yı oluşturmak için kullanılacak.

		private string hashData; // Normal ödeme için gerekli SHA1 hash

		public string HashData
		{
			get { return hashData; }
			set { hashData = value; }
		}

		private string userID; // İşlemi yapan kullanıcı. Banka tarafından zorunlu kılınmıyor. Üye işyeri tarafında ödemeyi alan kullanıcıyı temsil eder.

		public string UserID
		{
			get { return userID; }
			set { userID = value; }
		}

		private string TerminalID = "30691297"; // Üye işyerine tanımlanan ödeme tiplerine ait unique ID. 3D, 3D-PAY, 3D-FULL

		private string TerminalID_; // Hash için gerekli 

		private string MerchantID = "7000679"; // Üye işyeri no

		private string remoteIP; // Bankaya gönderilmek üzere POST request yapan aygıtın IP adresi

		public string RemoteIP
		{
			get { return remoteIP; }
			set { remoteIP = value; }
		}

		private string emailAddress; // Bankaya gönderilmek üzere POST request yapan aygıtın email adresi

		public string EmailAddress
		{
			get { return emailAddress; }
			set { emailAddress = value; }
		}

		private string orderID; // Her işlemde unique olması gereken iş emir numarası

		public string OrderID
		{
			get { return orderID; }
			set { orderID = value; }
		}

		private string cardNumber; // Kart no

		public string CardNumber
		{
			get { return cardNumber; }
			set { cardNumber = value; }
		}

		private string cardExpiryDate; // Kart son kullanma tarihi

		public string CardExpiryDate
		{
			get { return cardExpiryDate; }
			set { cardExpiryDate = value; }
		}

		private string cardCVV2; // Kart ccv

		public string CardCVV2
		{
			get { return cardCVV2; }
			set { cardCVV2 = value; }
		}

		private string transactionType = "sales"; // iade ve iptal gibi seçenekleri de bulunuyor fakat bu kütüphane için sadece satış tanımlı

		public string TransactionType
		{
			get { return transactionType; }
			set { transactionType = value; }
		}

		private string transactionAmount; // Karttan çekilecek miktar

		public string TransactionAmount
		{
			get { return transactionAmount; }
			set { transactionAmount = value; }
		}

		private string currencyCode; // Para birimi kodu : 949 TRY, 840 USD, 978 EURO, 826 GBP, 392 JPY 

		public string CurrencyCode
		{
			get { return currencyCode; }
			set { currencyCode = value; }
		}

		private string motoInd; // İşlermin Mail Order olup olmadığı belirler. Varsayılan olarak N yani kapalıdır.

		public string MotoInd
		{
			get { return motoInd; }
			set { motoInd = value; }
		}

		// BIN Query information

		private string cardScheme; // Kart Tipi

		public string CardScheme
		{
			get { return cardScheme; }
			set { cardScheme = value; }
		}

		private string bankName; // Kartın ait olduğu banka

		public string BankName
		{
			get { return bankName; }
			set { bankName = value; }
		}

		private string bankURL; // Kartın ait olduğu bankaya ait websitesi URL

		public string BankURL
		{
			get { return bankURL; }
			set { bankURL = value; }
		}

		private string cardCountry; // Bankanın menşei

		public string CardCountry
		{
			get { return cardCountry; }
			set { cardCountry = value; }
		}

		private string cardCountryCode; // Bankanın ülke kodu

		public string CardCountryCode
		{
			get { return cardCountryCode; }
			set { cardCountryCode = value; }
		}

		// 3D alanları

		private string successURL; // 3D işleminin başarılı dönüşünde bankanın POST request yapacağı adres

		public string SuccessURL
		{
			get { return successURL; }
			set { successURL = value; }
		}

		private string errorURL; // 3D işleminin başarısız dönüşünde bankanın POST request yapacağı adres

		public string ErrorURL
		{
			get { return errorURL; }
			set { errorURL = value; }
		}

		public string ProvisionURL = "https://sanalposprov.garanti.com.tr/VPServlet"; // Normal XML ödeme URL

		public string ProvisionTestURL = "https://sanalposprovtest.garanti.com.tr/VPServlet";  // Normal XML ödeme URL test ortamı

		public string Provision3DURL = "https://sanalposprov.garanti.com.tr/servlet/gt3dengine"; // 3D XML ödeme URL

		public string Provision3DTestURL = "https://sanalposprov.garanti.com.tr/servlet/gt3dengine"; // 3D XML ödeme URL test ortamı

		public void SetParameters(Dictionary<string, string> param) // Pay() methodu çağrılmadan önce bu method çağrılır. İçerisine Kart bilgileri ve müşteri bilgileri verilir.
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

		public XElement GetXML() // Bankaya yapılacak POST request verisi. veri tipi XML
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

		public JObject BINQuery() // Karta ait banka bilgilerine ulaşmak için kullanılan method
		{
			string cardNum = this.CardNumber.Substring(0, 6);

			var client = new RestClient();

			client.BaseUrl = new Uri("https://lookup.binlist.net/" + cardNum);

			var request = new RestRequest(Method.POST);

			IRestResponse response = client.Execute(request);

			JObject json = JObject.Parse(response.Content);

			return json;
		}

		public XElement Pay() // Bankaya POST request işlemini yapar
		{
			var client = new RestClient();

			client.BaseUrl = new Uri(this.ProvisionTestURL);

			var request = new RestRequest(Method.POST);

			request.AddParameter("application/xml", this.GetXML(), ParameterType.RequestBody);

			IRestResponse response = client.Execute(request);

			XElement xmlDoc = XElement.Parse(response.Content);

			return xmlDoc;
		}

		public string GenerateOrderID() // OrderID üretir.
		{
			Random rnd = new Random();

			string result;

			using (SHA1 enc = new SHA1CryptoServiceProvider())
			{
				byte[] keySecurity = enc.ComputeHash(Encoding.UTF8.GetBytes(this.HashData + rnd.Next(int.MaxValue).ToString()));

				string keySecStr = string.Concat(keySecurity.Select(b => b.ToString("X2")));

				result = keySecStr.Substring(0, 9);

			}

			return result;
		}
	}
}
