
using InficareTaskProject.Entities;
using InficareTaskProject.Interfaces;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace InficareTaskProject.Classes
{
    public class JwtTokenManager : IJwtTokenManager
    {
        private readonly string _key = "eyJ0eXAiOiJKV1QiLA0KICJhbGciOiJSUzI1NiJ9";
        private static string _privateKey = "MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDWP+LBrpSrGho6f0qSvRqTEjlFGU2pB+91NEs4HAqqWJa15ry04fAnRUXnqNotpS1w+R47LxFXSVVWfyLzFJt6qlAB++xcDN0mxBjhBZSaJjC68kkmiKrEaackqYPN5pw/mgr953r9vYzaxXTHJ2Yq5A6YjT88WHa16iQUNj4QWNor6/QjzRa2Le07m248jTVr179blnF4eSpmBC7380P0fnWQOJsOmtL9hskdYSZ2ey89+4lfMoJIjEdYmvgK/gdccewHCC2nLK+IiHJ5I7q45itaui6+8tMtspXpd8ZWmA+6KKGa6J9P968zjCofh5yryOSXU6Ftqb0eSLnSd3k5AgMBAAECggEBAKslXX5ad2g28bzI8klFxnS1mgoYrHDaZe66V1v7CooJOlsUdqoH0k+MhssHl0HfMO2OExg0ASY47nKqHMERNSJH6qGrHKRroj3VaApY6tw4pbSw1r5xbfh4bYxb3W4dSbOE2kL5JsaLJHmeQirUzkh05TP3oQwTnsCyyR9cNTfYS7zjvUfhD1X6+iAfKwsu5EoY8txVkFMwHT5rBvraIZfstcPtjmvjxl63hIibKQde4p7y5DbYzfoyW3dmfUbv21mXz9RckCYjnfLTtbCMoJkpZmmG/U3OzTmJPY/LqZo5TXtScK8ghSFrq/h/IwfEIW2hQxBk80Zfg+fYU2lBwgECgYEA9wsv73aJdF0aKTkaEQhy0PlB5VU+r7YrkIS8fQ2KNynCQGuATSa9ngxj72RSi5R+kSG4fvcnnbWFrVci4zkVJ9LSZpVUUi6woLGmUVLOxoomg0MqMXf/uMO2gA9IiFOWVbbhBnNWthnqfdv1bMaS/0dLOpojmI0xz8FKZw1luWMCgYEA3gRWBbchaH6U8FE/Rnai7E5LsgS8n8KdKy2Z+gYaC5u4srcDAvv1/2nyy6Oe8vfa7PyMQeiu3AgSEzq7HBvcL+v0cYMC23UaFP0iZpsN1BN47hooNdIoLIZapHBcBw8Km7YixlXNvlOLP9/T8eQH0RmuP1cfvi5NoKcGy+CBk7MCgYEAuDK2NiU7DfFRTUVPQz36ZghD8tvhlqAM0X+3DsLmuITBWskM6/mjzlFMvjUeOwaobEwpRTnBZxzupDVhROzN8liGGojjSzeW6OosqVxF4tElMCAHOMVRfZEn5Un/+e3hHN3z4JxlHfgq2FcGImnZNtAQrow5WEiGyHJFqJrbqyMCgYBsP8z5iGrfmihgbBzf7GRPt3Us3SZi9u7O6LMUtvjoBAswCA+N2u/8ktnw9FxBtxulRRXV5O+tFLpLae1lLShUrUVIO0qfWh9XcHzfbmZ1qvDhlxtX8pnn67IyTx5vyqOvfETEU323eOCCSk36uOYey9irxX0Sq1zqIcYsjdBgBQKBgQDPRVzarfG4pp8ywPQFZOiBCUUMhSd3OjmdFj79487jqJ+TE5FP/6vQHuWmDYcJy5GtyOdw0apqUr9EqS49qMphZQUgtfBvwIihzVSEiZIimNsq43GqEzsXC/IZvxZfb3vcHmOoCMpfsCCFgVL5GIbK98cf/4I8m1YvZJuj/5kmtg==";

        private readonly IConfiguration _configuration;
        public JwtTokenManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(Customer identityUser)
        {
            var userClaims = GetUserClaims(identityUser);

            return RSATest(userClaims);
        }
        
        private List<Claim> GetUserClaims(Customer identityUser)
        {
            var requestData = JsonConvert.SerializeObject(new InitiatePaymentRequest());
            var userClaims = new List<Claim>
            {
                new Claim("CheckPhoneNumberRequest", requestData),

            };

            return userClaims;
        }
         
        private string GenerateJwtToken(List<Claim> userClaims)
        {
            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

            var signingCred = new SigningCredentials(jwtKey, SecurityAlgorithms.RsaSha256);

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                SigningCredentials = signingCred,
                Expires = DateTime.UtcNow.AddHours(1),
                Audience = _configuration["JWT:JwtAudience"],
                Issuer = _configuration["JWT:JwtAudience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public string RSATest(List<Claim> userClaims)
        {
            var privateKeyBytes = Convert.FromBase64String("MIIEowIBAAKCAQEAyokLGrnqRUToy6kYNe/WyY3d0WEzXaUK+G2M8h4xtKGfPIYR\nTKPZvX93D6IgoyqZxfvjeyXDJGUfZqskBTwSq+j+cy3X0xDGe8pL+FUavvuV2BkN\ndPWKWxqoy0PKW0GaaT3wUQOQNQmxKTgryHIeT+n/97lmZNx4K2p5z6bhIpj6ZltQ\n4O5FWGswtUM0wWk7Svw/Tk9Br6W4OlEcudXPA99swy7JW0BmKgKgnTbt9V7hhUx2\n2BiQE19XGlFB5mKxRz1CaePuv3b9EuO16Ym/Dg60Ex4RtNpbu6nlKDFXffgCH09T\nj8bmDO/Dk7PMJHe9f5UULZekRBz7DeJg09I2/wIDAQABAoIBACaoY7s1MzcHgRum\nad2ZqriL4IPfdqtwHhju6BEqnDgrBTbLLDAhsiTOWI5eVrZuIi28912BBq9Cseyp\ny9VH8xRnA3I+lMxPjmMAaOG8dL9xS9eUaJIjb8YV35P0m4IxkmR2ExGTiYnmEK+a\nbhjzVz/PnTDObXHg9vrqdtegtaYhj9ikw2w48owciuBsD3Bg03xMUwHbynAUG3Av\njU5LD5bfuEpGumDNnPy6AXFp2NJEeeq9is2Y0eFxgnRJNAljjzMZPoRkwCVAsy8g\nbuXyMTiCYZEx6acF4HWwiMtrGurL2HildWzQkd2VLOrureHw6T4vgios9S65AjAL\nv4gJcCkCgYEA74KbMp2SYuAq0pQ0kPClL6roT7/4MZ0DHt0nUlviCdRm2D9K5fkC\ncIAQQnWT2e6bVHbhw3vkjmG0o0eUL6VyMNvfibGhn+yzDU1gpD32bNsoRU5IeuMQ\nodhg6aOPFlPvDTNmbidMTvL9QaD7RHfr4KJIHDkGh2+ElB6KBGgwUnMCgYEA2HrA\nTRFF2SgmB3We9rtVT4bJZSAOkZmiI2VpM8loZ3nwEs5x3OKpaBQEs4HClwWxjuoK\nZoKfyZKnaNz1LjJ1VGKZDugKONfyPZWCIIprfgVkviRQwJHnsqbppTeQTLVohidH\n6jY8m4TLrL0haxJDT0NoLiIj9srEoR3X8zu+ikUCgYEArDyXEPIhqDsecGql1qlH\nkRztjRQ3Dq6j5NkTAvYSehElmFMDsJe+elqN1s0o8urVBwuq1OJOfVmkBlteJFls\n4dfsS9/So+ga5vEDE3l/sc50ikp+cujBODIbl0jIiDz5xtt0yLg39vpkx4JVz2oR\n1Wu+QZV8rX6zr7S6eerW/SMCgYBfz8R416JAgKKEPqzCqxsQ/aj5VvzbuFGotOOh\nBg1tbuywhiqjBrbP17xU7qN/UAfMJw2/XST3hC8QHGtvrOl9Fb6EeHK9weX3F8rm\nOB1nQ1/ZQB11fZ481d8nPrZhHRFL/uq3YJXmhxnWNEcsKoMb+8uKT5X3TrtES/8e\nKl0kuQKBgBqsCtM2jyLJ88cF6T2kr2Zt+tcUCz2WJEJlOAYymj/nid/rIwQLEpak\nnI9LzIm6q7z3vb/dRb8nl56S5sN4z0pO3v8PCfsDofTQaSANIbOLMvKwfIW01ts9\nZXmKQ7kTqMt2GwN0uIXPt4W7rxcScaG1WivamJevWIrH/htpSL5s");

            using (var rsa = RSA.Create(256))
            {
                rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
                var key = new RsaSecurityKey(rsa);
                var handler = new JsonWebTokenHandler();
                var now = DateTime.UtcNow;
                var tokenHandler = new JwtSecurityTokenHandler();


                var descriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(userClaims),
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
                };

                var jwt = handler.CreateToken(descriptor);

                TokenValidationResult result = handler.ValidateToken(jwt,
                    new TokenValidationParameters
                    {
                        ValidIssuer = "me",
                        ValidAudience = "you",
                        IssuerSigningKey = new RsaSecurityKey(key.Rsa.ExportParameters(false))
                    });

                return (jwt);

            }


        }
        //private string Encrypt(string message, string key)
        //{
        //    if (message == null || key == null)
        //    {
        //        throw new IllegalArgumentException("Text to be encrypted and key should not be null");
        //    }
        //    Cipher cipher = Cipher.getInstance(ALGORITHM);
        //    byte[] messageArr = Encoding.ASCII.GetBytes(message);
        //    SecretKeySpec keySpec = new
        //    SecretKeySpec(Base64.getDecoder().decode(key), "AES");
        //    byte[] ivParams = new byte[16];
        //    byte[] encoded = new byte[messageArr.Length + 16];
        //    Array.Copy(ivParams, 0, encoded, 0, 16);
        //    Array.Copy(messageArr, 0, encoded, 16, messageArr.Length);
        //    cipher.init(Cipher.ENCRYPT_MODE, keySpec, new IvParameterSpec(ivParams));
        //    byte[] encryptedBytes = cipher.doFinal(encoded);
        //    encryptedBytes = Base64.getEncoder().encode(encryptedBytes);
        //    return Encoding.UTF8.GetString(encryptedBytes);
        //}

        public string SignDataPKCS8(string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            var pri = Convert.FromBase64String(_privateKey);
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportPkcs8PrivateKey(pri, out _);
                var sign = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(sign);
            }
        }
    }
    public class RequestDataModel
    {
        public string Name { get; set; }
        public Details Details { get; set; }
    }
    public class Details
    {
        public string Email { get; set; }
        public DataFields DataFields { get; set; }
        public string Organization { get; set; }
    }


    public class DataFields
    {
        public string FirstName { get; set; }
        public string Designation { get; set; }
    }


    public class InitiatePaymentRequest
    {
        public Header Header { get; set; }
        public TransactionData TransactionData { get; set; }
    }
    public class Header
    {
        public string ReqId { get; set; }
        public string ClientCode { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string ReservedFieldH1 { get; set; }
        public string ReservedFieldH2 { get; set; }
        public string ReservedFieldH3 { get; set; }
    }
    public class TransactionData
    {
        public string CorporateRefNo { get; set; }
        public string PaymentType { get; set; }
        public string TransferAmount { get; set; }
        public string TransactionDate { get; set; }
        public string BeneficiaryIFSC { get; set; }
        public string BeneficiaryAccountType { get; set; }
        public string BeneficiaryAccountNo { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryAddress1 { get; set; }
        public string BeneficiaryAddress2 { get; set; }
        public string BeneficiaryAddress3 { get; set; }
        public string BeneficiaryZIPCode { get; set; }
        public string BeneficiaryEmail { get; set; }
        public string BeneficiaryMobileNo { get; set; }
        public string ShipmentDate { get; set; }
        public string VpaAddress { get; set; }
        public string IECode { get; set; }
        public string PanCard { get; set; }
        public string PurposeID { get; set; }
        public string InvoiceNumber { get; set; }
        public string ServiceUtilizeCntry { get; set; }
        public string RemitterName { get; set; }
        public string RemitterID { get; set; }
        public string RemitterAddress1 { get; set; }
        public string RemitterAddress2 { get; set; }
        public string RemitterAddress3 { get; set; }
        public string RemitterZIPCode { get; set; }
        public string RemitterEmail { get; set; }
        public string RemitterMobileNo { get; set; }
        public string RemitterCountry { get; set; }
        public string ReservedFieldD1 { get; set; }
        public string ReservedFieldD2 { get; set; }
        public string ReservedFieldD3 { get; set; }
        public string ReservedFieldD4 { get; set; }
        public string ReservedFieldD5 { get; set; }
    }

}





