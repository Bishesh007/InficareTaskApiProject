using InficareTaskProject.Data;
using InficareTaskProject.Entities;
using InficareTaskProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InficareTaskProject.Interfaces;
using System.Buffers.Text;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace InficareTaskProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly UserManager<Customer> _userManager;
        private readonly SignInManager<Customer> _signInManager;
        private readonly IJwtTokenManager _jwtTokenManager;


        private readonly ApplicationDbContext _context;
        public CustomerController(
                                    UserManager<Customer> userManger,
                                    SignInManager<Customer> signInManager,
                                    ApplicationDbContext context,
                                    IJwtTokenManager jwtTokenManager)
        {
            _userManager = userManger;
            _signInManager = signInManager;
            _context = context;
            _jwtTokenManager = jwtTokenManager;
        }

        [HttpPost("/api/customers/createCustomer")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> CreateCustomer(CustomersViewModel user)
        {
            var customer = new Customer
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };


            IdentityResult result = await _userManager.CreateAsync(customer, user.Password);

            if (!result.Succeeded)
                return BadRequest();

            return Ok(customer);
        }


        [HttpPost("/api/customers/authenticateCustomer")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateUser request)
        {
            var identityUser = await _userManager.FindByNameAsync(request.UserName);

            if (identityUser == null)
                return Unauthorized();


            var result = await _signInManager.CheckPasswordSignInAsync(identityUser, request.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Unauthorized();

            var token = _jwtTokenManager.GenerateToken(identityUser);

            return Ok(token);
        }

        [HttpGet]
        [Route("/api/customers/getCustomers")]
        [Authorize]
        public async Task<ActionResult<List<CustomersViewModel>>> GetAllCustomers()
        {
            var getCustomerQuery = await _context
                                                .Customers
                                                .Select(x => new CustomersViewModel
                                                {
                                                    UserName = x.UserName,
                                                    Email = x.Email,
                                                    PhoneNumber = x.PhoneNumber,
                                                    Password = x.PasswordHash,
                                                    
                                                }).ToListAsync();
            return Ok(getCustomerQuery);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("/api/customer/post")]

        public async Task<ActionResult> postCustomer([FromBody]InitiatePaymentRequest initiatePaymentRequest)
        {
            var requestObj = new InitiatePaymentRequest
            {
                Header = new Header
                {
                    ReqId = "15MON31082020",
                    ClientCode = "XYZ",
                    UserId = "corp8",
                    Password = "F35A2B3E6816CBE1618811ECC25B4F106",
                    ReservedFieldH1 = "RESERVED FIELD IN THE API REQ HEADER",
                    ReservedFieldH2 = "RESERVED FIELD IN THE API REQ HEADER",
                    ReservedFieldH3 = "RESERVED FIELD IN THE API REQ HEADER"
                },
                TransactionData = new TransactionData
                {
                    CorporateRefNo = initiatePaymentRequest.TransactionData.CorporateRefNo,
                    PaymentType = initiatePaymentRequest.TransactionData.PaymentType,
                    TransferAmount = initiatePaymentRequest.TransactionData.TransactionDate,
                    TransactionDate = initiatePaymentRequest.TransactionData.TransactionDate,
                    BeneficiaryIFSC = initiatePaymentRequest.TransactionData.BeneficiaryIFSC,
                    BeneficiaryName = initiatePaymentRequest.TransactionData.BeneficiaryName,
                    BeneficiaryAddress1 = initiatePaymentRequest.TransactionData.BeneficiaryAddress1,
                    BeneficiaryAddress2 = initiatePaymentRequest.TransactionData.BeneficiaryAddress2,
                    BeneficiaryAddress3 = initiatePaymentRequest.TransactionData.BeneficiaryAddress3,
                    BeneficiaryZIPCode = initiatePaymentRequest.TransactionData.BeneficiaryZIPCode,
                    BeneficiaryEmail = initiatePaymentRequest.TransactionData.BeneficiaryEmail,
                    BeneficiaryMobileNo = initiatePaymentRequest.TransactionData.BeneficiaryMobileNo,
                    ShipmentDate = initiatePaymentRequest.TransactionData.ShipmentDate,
                    VpaAddress = initiatePaymentRequest.TransactionData.VpaAddress,
                    IECode = initiatePaymentRequest.TransactionData.IECode,
                    PanCard = initiatePaymentRequest.TransactionData.PanCard,
                    PurposeID = "2",
                    InvoiceNumber = initiatePaymentRequest.TransactionData.InvoiceNumber,
                    ServiceUtilizeCntry = initiatePaymentRequest.TransactionData.ServiceUtilizeCntry,
                    RemitterName = initiatePaymentRequest.TransactionData.RemitterName,
                    RemitterID = initiatePaymentRequest.TransactionData.RemitterID,
                    RemitterAddress1 = initiatePaymentRequest.TransactionData.RemitterAddress1,
                    RemitterAddress2 = initiatePaymentRequest.TransactionData.RemitterAddress2,
                    RemitterAddress3 = initiatePaymentRequest.TransactionData.RemitterAddress3,
                    RemitterZIPCode = initiatePaymentRequest.TransactionData.RemitterZIPCode,
                    RemitterEmail   = initiatePaymentRequest.TransactionData.RemitterEmail,
                    RemitterMobileNo = initiatePaymentRequest.TransactionData.RemitterMobileNo,
                    RemitterCountry = initiatePaymentRequest.TransactionData.RemitterCountry,
                    ReservedFieldD1 = "CORPORATE USER WHICH IS REQUESTING FOR INITIATE PAYMENT API",
                    ReservedFieldD2 = "CORPORATE USER WHICH IS REQUESTING FOR INITIATE PAYMENT API",
                    ReservedFieldD3 = "CORPORATE USER WHICH IS REQUESTING FOR INITIATE PAYMENT API",
                    ReservedFieldD4 = "CORPORATE USER WHICH IS REQUESTING FOR INITIATE PAYMENT API",
                    ReservedFieldD5 = "CORPORATE USER WHICH IS REQUESTING FOR INITIATE PAYMENT API"
                }
            };
            var jsonstr = JsonConvert.SerializeObject(requestObj);
            var convertionData = _jwtTokenManager.SignDataPKCS8(jsonstr);


            return Ok(convertionData);

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
}
