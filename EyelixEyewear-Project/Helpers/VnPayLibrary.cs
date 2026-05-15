using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace EyelixEyewear_Project.Helpers
{
    public class VnPayLibrary
    {
        private SortedList<string, string> _requestData = new SortedList<string, string>(StringComparer.InvariantCultureIgnoreCase);

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                _requestData.Add(key, value);
        }

        public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
        {
            var data = new StringBuilder();
            foreach (var kv in _requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }

            string queryString = data.ToString().TrimEnd('&');
            string signData = queryString;
            string vnpSecureHash = HmacSHA512(vnpHashSecret, signData);

            return baseUrl + "?" + queryString + "&vnp_SecureHash=" + vnpSecureHash;
        }

        public bool ValidateSignature(IQueryCollection queryString, string vnpHashSecret)
        {
            var vnpSecureHash = queryString["vnp_SecureHash"];
            var inputData = new SortedList<string, string>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var key in queryString.Keys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_") && key != "vnp_SecureHash" && key != "vnp_SecureHashType")
                    inputData.Add(key, queryString[key]);
            }

            var data = new StringBuilder();
            foreach (var kv in inputData)
                data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");

            string rawData = data.ToString().TrimEnd('&');
            string checkSum = HmacSHA512(vnpHashSecret, rawData);
            return checkSum.Equals(vnpSecureHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string HmacSHA512(string key, string data)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
