# GarantiBBVA-POS-CSharp
GarantiBBVA PoS integration to C# language.

This tiny library is created for getting provision without 3D Secure integration. For educational and commercial purposes.

# Usage:

Little example:

```csharp
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
```

result variable contains the XML data from GarantiBBVA provision post. 

# Essentials:
For production pos.Mode should be "PROD" and pos.TerminalID, pos.MerchandID, pos.ProvUserID, pos.ProvUserPassword and pos.SecureKey properties must be set before the pos.Pay() method runs.

# Bin Query:
```pos.BINQuery(string cardNumber)``` method makes a bin query for the provided card. Method takes one parameter and it should be the first 6 digits of the card. It will return a JObject and it contains:
BankName -> bank name of the provided card
CardSceme -> Visa, MasterCard or AMEX
BankURL -> URL of the card's bank
CardCountry -> origin country of the card's bank
CardCountryCode-> country code of the bank's country from WITS (World Integrated Trade Solution)
