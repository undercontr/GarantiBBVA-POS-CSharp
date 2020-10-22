# GarantiBBVA-POS-CSharp
GarantiBBVA PoS integration to C# language.

This tiny library is created for getting provision without 3D Secure integration. For educational and commercial purposes.

# Usage:

Little example:

```csharp
GarantiPOS pos = new GarantiPOS();

pos.UserID = "PersonnelX";
pos.CurrencyCode = "949"; // for ₺ Turkish Lira
pos.MotoInd = "N"; // Mail Order payment, default false

Dictionary<string, string> dict = new Dictionary<string, string>
{
    { "IPAddress", "12.12.12.12" }, // IP Address must be gathered by programatically!
    { "EmailAddress", "email@email.com" },
    { "OrderID", pos.GenerateOrderID() },
    { "CardNumber", "XXXXXXXXXXXXXXXX" },
    { "CardExpiryDate", "0323" },
    { "CardCVV2", "465" },
    { "TransactionAmount", "1000" }
};

pos.SetParameters(dict); // This must be set before the pos.Pay() method.

XElement result = pos.Pay();
```

result variable contains the XML data from GarantiBBVA provision post. 

# Essentials:
For production pos.Mode should be "PROD" and below properties must be set before the pos.Pay() method runs: 
+ pos.TerminalID
+ pos.MerchandID
+ pos.ProvUserID
+ pos.ProvUserPassword
+ pos.SecureKey 

# Bin Query:
```pos.BINQuery(string cardNumber)``` method makes a bin query for the provided card. Method takes one parameter and it should be the first 6 digits of the card. It will return a JObject and it contains:
+ BankName -> bank name of the provided card
+ CardSceme -> Visa, MasterCard or AMEX
+ BankURL -> URL of the card's bank
+ CardCountry -> origin country of the card's bank
+ CardCountryCode-> country code of the bank's country from WITS (World Integrated Trade Solution)
