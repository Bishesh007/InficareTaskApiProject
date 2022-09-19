using InficareTaskProject.Data;
using InficareTaskProject.Entities;
using InficareTaskProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InficareTaskProject.Interfaces;

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
                                                }).ToListAsync();
            return Ok(getCustomerQuery);
        }


    }
}
