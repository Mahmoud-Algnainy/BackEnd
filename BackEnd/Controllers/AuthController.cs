using AuthenticationPlugin;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly AuthService _auth;
        private HcfiDBContext _dbContext;
        public AuthController(HcfiDBContext dbContext, IConfiguration configuration)
        {
            _configuration = configuration;
            _auth = new AuthService(_configuration);
            _dbContext = dbContext;
        }


        [HttpGet("GetAllUsers")]
        public async Task<IActionResult>GetAllUsers()
        {
            var users= await _dbContext.Accounts.ToListAsync();

            return Ok(users);
        }
        [HttpGet("GetUser/{sarf_id}")]
        public async Task<IActionResult> GetUser(string sarf_id)
        {
            var user = await _dbContext.Accounts.FirstOrDefaultAsync(x=>x.Sarf_Id==sarf_id);

            return Ok(user);
        }


        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(Account account)
        {
            var accountWithSameEmail = _dbContext.Accounts.SingleOrDefault(u => u.Email == account.Email || u.UserName == account.UserName);
            if (accountWithSameEmail != null) return BadRequest("Account with this  UserName already exists");
            var accountObj = new Account
            {
                Sarf_Id = account.Sarf_Id,
                UserName = account.UserName,

                Email = account.Email,
                Password = SecurePasswordHasherHelper.Hash(account.Password),
                FullName = account.FullName,
                PhoneNumber=account.PhoneNumber,
            };

            _dbContext.Accounts.Add(accountObj);
            await _dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet("CheckExist/{sarf_id}")]
        [AllowAnonymous]

        public async Task<IActionResult> CheckExist(string sarf_id)
        {
            var checkexistaccount =await _dbContext.Employees.FirstOrDefaultAsync(x=>x.Sarf_Id==sarf_id);

            if (checkexistaccount == null) return NotFound();
            return Ok(checkexistaccount);
        }
        [HttpPost("Login")]
        [AllowAnonymous]
        public IActionResult Login(Account account)
        {
            var AccountUser = _dbContext.Accounts.FirstOrDefault(u => u.UserName == account.UserName || u.Email == account.Email);
            if (AccountUser == null) return StatusCode(StatusCodes.Status404NotFound);
            var hashedPassword = AccountUser.Password;
            if (!SecurePasswordHasherHelper.Verify(account.Password, hashedPassword)) return Unauthorized();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, AccountUser.Email ), 
                new Claim(ClaimTypes.Name, AccountUser.UserName),
            };

            var token = _auth.GenerateAccessToken(claims);
            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                token_type = token.TokenType,
                user_Id = AccountUser.Id,
                user_name = AccountUser.UserName,
                expires_in = token.ExpiresIn,
                creation_Time = token.ValidFrom,
                expiration_Time = token.ValidTo,
                accountid = AccountUser.Id,
                email = AccountUser.Email,
                sarf_id=AccountUser.Sarf_Id

            });
        }





    }
}
