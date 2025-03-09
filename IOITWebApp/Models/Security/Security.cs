using IOITWebApp.Models.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

public class Security
{

    //static int ComparisonTwoKey(QueryRequest a, QueryRequest b)
    //{
    //    // Here we sort two times at once, first one the first item, then on the second.
    //    // ... Compare the first items of each element.
    //    var part1 = a.Key;
    //    var part2 = b.Key;
    //    var compareResult = part1.CompareTo(part2);
    //    // If the first items are equal (have a CompareTo result of 0) then compare on the second item.
    //    if (compareResult == 0)
    //    {
    //        return b.Key.CompareTo(a.Key);
    //    }
    //    // Return the result of the first CompareTo.
    //    return compareResult;
    //}

    //public static bool CheckSecureHash(string url)
    //{
    //    bool check = false;
    //    string secureHash = "";
    //    url = "https://localhost:44390/resuft-apply-evisa.html?vpc_OrderInfo=E679HIFWDLYHMRMSDK&vpc_3DSECI=05&vpc_Merchant=TESTONEPAY&vpc_Card=VC&vpc_AcqResponseCode=00&AgainLink=evisavietnam.org.vn%2Fapply-evisa.html&vpc_AuthorizeId=831000&vpc_3DSenrolled=Y&vpc_RiskOverallResult=ACCEPT&vpc_ReceiptNo=189852&vpc_TransactionNo=29140914&vpc_AVS_StateProv=Unknow&vpc_Locale=en&vpc_TxnResponseCode=0&vpc_VerToken=AAABAWFlmQAAAABjRWWZEEFgFz%2B%3D&vpc_Amount=109181000&vpc_BatchNo=20200103&vpc_Version=2&vpc_AVSResultCode=Y&vpc_VerStatus=Y&vpc_Command=pay&vpc_Message=Approved&Title=EVISA+VIET+NAM&vpc_3DSstatus=Y&vpc_SecureHash=B976A47F535DEE2998D7D0307BE2409DE9C019F25BF95911AF65048F41B026E3&vpc_CardNum=400000xxxxxxx002&vpc_AVS_PostCode=0&vpc_CSCResultCode=M&vpc_MerchTxnRef=4a8217d7-f6a7-4410-836c-40d6d0bda5f8&vpc_VerType=3DS&vpc_VerSecurityLevel=05&vpc_3DSXID=UmpsdlRNaURNY3BrTmJXMWNpRDA%3D";
    //    string query = url.Split('?')[1];
    //    var listQuery = query.Split('&');
    //    List<QueryRequest> listQ = new List<QueryRequest>();
    //    foreach (var item in listQuery)
    //    {
    //        QueryRequest queryRequest = new QueryRequest();
    //        queryRequest.Key = item.Split('=')[0];
    //        queryRequest.Value = item.Split('=')[1];
    //        if (queryRequest.Key.StartsWith("vpc_") && !queryRequest.Key.Equals("vpc_SecureHash"))
    //        {
    //            listQ.Add(queryRequest);
    //        }

    //        if (queryRequest.Key.Equals("vpc_SecureHash"))
    //        {
    //            secureHash = queryRequest.Value;
    //        }
    //    }
    //    //sort
    //    listQ.Sort(ComparisonTwoKey);


    //    //create key
    //    string key = "6D0870CDE5F24F34F3915FB0045120DB";
    //    string messege = "";
    //    foreach (var item in listQ)
    //    {
    //        messege += item.Key + "=" + item.Value + "&";
    //    }
    //    if (messege.Length > 0)
    //        messege = messege.Substring(0, messege.Length - 1);
    //    string secureHashNew = Security.HashHMACHex(key, messege).ToUpper();
    //    if (secureHash == secureHashNew)
    //        check = true;

    //    return check;
    //}



    private static byte[] HashHMAC(byte[] key, byte[] message)
    {
        var hash = new HMACSHA256(key);
        return hash.ComputeHash(message);
    }

    private static byte[] HashSHA(byte[] innerKey, byte[] outerKey, byte[] message)
    {
        var hash = new SHA256Managed();

        // Compute the hash for the inner data first
        byte[] innerData = new byte[innerKey.Length + message.Length];
        Buffer.BlockCopy(innerKey, 0, innerData, 0, innerKey.Length);
        Buffer.BlockCopy(message, 0, innerData, innerKey.Length, message.Length);
        byte[] innerHash = hash.ComputeHash(innerData);

        // Compute the entire hash
        byte[] data = new byte[outerKey.Length + innerHash.Length];
        Buffer.BlockCopy(outerKey, 0, data, 0, outerKey.Length);
        Buffer.BlockCopy(innerHash, 0, data, outerKey.Length, innerHash.Length);
        byte[] result = hash.ComputeHash(data);

        return result;
    }

    private static byte[] StringEncode(string text)
    {
        var encoding = new ASCIIEncoding();
        return encoding.GetBytes(text);
    }

    private static string HashEncode(byte[] hash)
    {
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    private static byte[] HexDecode(string hex)
    {
        var bytes = new byte[hex.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
        }
        return bytes;
    }

    public static string HashHMACHex(string keyHex, string message)
    {
        byte[] hash = HashHMAC(HexDecode(keyHex), StringEncode(message));
        return HashEncode(hash);
    }

    public static string HashSHAHex(string innerKeyHex, string outerKeyHex, string message)
    {
        byte[] hash = HashSHA(HexDecode(innerKeyHex), HexDecode(outerKeyHex), StringEncode(message));
        return HashEncode(hash);
    }

    public static string getMd5(string pass)
    {
        return
            BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(pass))).
                ToLower().Replace("-", "");
    }

    public static string Encrypt(string key, string data)
    {
        data = data.Trim();

        byte[] keydata = Encoding.ASCII.GetBytes(key);

        string md5String = BitConverter.ToString(new
                                                        MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-",
                                                                                                                "").
            ToLower();

        byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));

        TripleDES tripdes = TripleDES.Create();

        tripdes.Mode = CipherMode.ECB;

        tripdes.Key = tripleDesKey;

        tripdes.GenerateIV();

        var ms = new MemoryStream();

        var encStream = new CryptoStream(ms, tripdes.CreateEncryptor(),
                                            CryptoStreamMode.Write);

        encStream.Write(Encoding.ASCII.GetBytes(data), 0, Encoding.ASCII.GetByteCount(data));

        encStream.FlushFinalBlock();

        byte[] cryptoByte = ms.ToArray();

        ms.Close();

        encStream.Close();

        return Convert.ToBase64String(cryptoByte, 0, cryptoByte.GetLength(0)).Trim();
    }

    public static string Decrypt(string key, string data)
    {
        byte[] keydata = Encoding.ASCII.GetBytes(key);

        string md5String = BitConverter.ToString(new
                                                        MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-",
                                                                                                                "").
            Replace(" ", "+").ToLower();

        byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));

        TripleDES tripdes = TripleDES.Create();

        tripdes.Mode = CipherMode.ECB;

        tripdes.Key = tripleDesKey;

        byte[] cryptByte = Convert.FromBase64String(data);

        var ms = new MemoryStream(cryptByte, 0, cryptByte.Length);

        ICryptoTransform cryptoTransform = tripdes.CreateDecryptor();

        var decStream = new CryptoStream(ms, cryptoTransform,
                                            CryptoStreamMode.Read);

        var read = new StreamReader(decStream);

        return (read.ReadToEnd());
    }

    //public static string GetIP()
    //{
    //    string IP = "";
    //    if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
    //    {
    //        IP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
    //    }
    //    if (IP == "")
    //    {
    //        IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
    //    }
    //    return IP;
    //}

    public static bool IsValidURIAddress(string uri)
    {
        bool success = false;
        try
        {
            //ServicePointManager.CertificatePolicy aa = new TrustAllCertificatePolicy();
            var myRequest = (HttpWebRequest)WebRequest.Create(uri);
            myRequest.Method = "GET";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.KeepAlive = false;
            myRequest.AllowAutoRedirect = false;
            using (var myResponse = (HttpWebResponse)myRequest.GetResponse())
            {
                if (myResponse.StatusCode == HttpStatusCode.OK)
                {
                    var sb = new StringBuilder();
                    var buf = new byte[8192];
                    Stream resStream = myResponse.GetResponseStream();
                    string tempString = null;
                    int count = 0;

                    do
                    {
                        count = resStream.Read(buf, 0, buf.Length);
                        if (count != 0)
                        {
                            tempString = Encoding.ASCII.GetString(buf, 0, count);
                            sb.Append(tempString);
                        }
                    } while (count > 0);
                    if (sb.ToString() ==
                        string.Format("pagate-site-verification: {0}", uri.Split('/')[uri.Split('/').Length - 1]))
                        success = true;
                    else success = false;
                }
                else
                    success = false;
            }
        }
        catch
        {
        }

        return success;
    }

    public static string RandomString(int size, bool lowerCase)
    {
        var builder = new StringBuilder();
        var random = new Random();
        char ch;
        for (int i = 0; i < size; i++)
        {
            ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
            builder.Append(ch);
        }
        if (lowerCase)
            return builder.ToString().ToLower();
        return getMd5(builder.ToString());
    }

    public static int RandomNumber(int min, int max)
    {
        var random = new Random();
        return random.Next(min, max);
    }

    public static string EncryptRSA(string publickey, string data)
    {
        data = data.Trim();
        string encryptedValue = string.Empty;
        var csp = new CspParameters(1);

        var rsa = new RSACryptoServiceProvider(csp);
        rsa.FromXmlString(publickey);

        byte[] bytesToEncrypt = Encoding.UTF8.GetBytes(data);
        byte[] bytesEncrypted = rsa.Encrypt(bytesToEncrypt, false);
        encryptedValue = Convert.ToBase64String(bytesEncrypted);
        return encryptedValue;
    }

    public static string DecryptRSA(string privateKey, string data)
    {
        data = data.Trim();
        string decryptedValue = string.Empty;
        var csp = new CspParameters(1);

        var rsa = new RSACryptoServiceProvider(csp);
        rsa.FromXmlString(privateKey);
        byte[] valueToDecrypt = Convert.FromBase64String(data);
        byte[] plainTextValue = rsa.Decrypt(valueToDecrypt, false);

        // Extract our decrypted byte array into a string value to return to our user
        decryptedValue = Encoding.UTF8.GetString(plainTextValue);
        return decryptedValue;
    }

    public static string CreateSignRSA(string data, string privateKey)
    {
        //RSACryptoServiceProvider rsaCryptoIPT = new RSACryptoServiceProvider(1024);

        CspParameters _cpsParameter;
        RSACryptoServiceProvider rsaCryptoIPT;
        _cpsParameter = new CspParameters();
        _cpsParameter.Flags = CspProviderFlags.UseMachineKeyStore;
        rsaCryptoIPT = new RSACryptoServiceProvider(1024, _cpsParameter);

        rsaCryptoIPT.FromXmlString(privateKey);
        return
            Convert.ToBase64String(rsaCryptoIPT.SignData(new ASCIIEncoding().GetBytes(data),
                                                            new SHA1CryptoServiceProvider()));
    }

    public static bool CheckSignRSA(string data, string sign, string publicKey)
    {
        try
        {
            var rsacp = new RSACryptoServiceProvider();
            rsacp.FromXmlString(publicKey);
            return rsacp.VerifyData(Encoding.UTF8.GetBytes(data), "SHA1", Convert.FromBase64String(sign));
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public static bool Checkkey(string key)
    {
        try
        {
            var rsacp = new RSACryptoServiceProvider();
            rsacp.FromXmlString(key);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static String sha256_hash(String value)
    {
        StringBuilder Sb = new StringBuilder();

        using (SHA256 hash = SHA256Managed.Create())
        {
            Encoding enc = Encoding.UTF8;
            Byte[] result = hash.ComputeHash(enc.GetBytes(value));

            foreach (Byte b in result)
                Sb.Append(b.ToString("x2"));
        }

        return Sb.ToString();
    }

    public static string HmacSha256Digest(string message, string secret)
    {
        ASCIIEncoding encoding = new ASCIIEncoding();
        byte[] keyBytes = encoding.GetBytes(secret);
        byte[] messageBytes = encoding.GetBytes(message);
        HMACSHA256 cryptographer = new HMACSHA256(keyBytes);

        byte[] bytes = cryptographer.ComputeHash(messageBytes);

        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}

//public class TrustAllCertificatePolicy : ICertificatePolicy
//{
//    #region ICertificatePolicy Members

//    public bool CheckValidationResult(ServicePoint sp,
//                                        X509Certificate cert, WebRequest req, int problem)
//    {
//        return true;
//    }

//    #endregion
//}
