using API.Data;
using Microsoft.AspNetCore.Mvc;
using API.Entities;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using API.DTOs;

namespace API.Controllers
{
  public class AuthController : BaseApiController
  {
    private readonly DataContext _context;
    public AuthController(DataContext context)
    {
      _context = context;
    }

    [HttpPost("register")] // POST: api/auth/register
    public async Task<ActionResult<AppUser>> Register(RegisterDTO registerDTO)
    {
      if (await UserExists(registerDTO.UserName)) return BadRequest("UserName is taken");

      using var hmac = new HMACSHA512();
      var user = new AppUser
      {
        UserName = registerDTO.UserName.ToLower(),
        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
        PasswordSalt = hmac.Key,
      };
      _context.Users.Add(user);
      await _context.SaveChangesAsync();
      return user;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AppUser>> Login(LoginDTO loginDTO)
    {
      var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName);
      if (user == null) return Unauthorized();
      using var hmac = new HMACSHA512(user.PasswordSalt);
      var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
      for (int i = 0; i < computedHash.Length; i++)
      {
        if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
      }
      return user;
    }
    private async Task<bool> UserExists(string username)
    {
      return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
  }
}