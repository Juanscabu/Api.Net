using Application.DTOs.Users;
using Application.Enums;
using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Settings;
using identity.Helpers;
using identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTSettings _jwtSettings;
        private readonly IDateTimeService _dateService;

        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IOptions<JWTSettings> jWTSettings, IDateTimeService dateService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtSettings = jWTSettings.Value;
            _dateService = dateService;
        }

        public async Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAdress)
        {
            var usuario = await _userManager.FindByEmailAsync(request.Email);
            if (usuario == null)
            {
                throw new ApiException($"No hay una cuenta registrada con el email ${request.Email}.");
            }
            var result = await _signInManager.PasswordSignInAsync(usuario.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new ApiException($"Las credenciales del usuario no son validas ${request.Email}.");
            }
            JwtSecurityToken jwtSecurityToken = await GenerateJWTTOKEN(usuario);
            AuthenticationResponse response = new AuthenticationResponse();
            response.Id = usuario.Id;
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.Email = usuario.Email;
            response.UserName = usuario.UserName;

            var roleList = await _userManager.GetRolesAsync(usuario).ConfigureAwait(false);
            response.Roles = roleList.ToList();
            response.IsVerified = usuario.EmailConfirmed;

            var refreshToken = GenerateRefreshToken(ipAdress);
            response.RefreshToken = refreshToken.Token;
            return new Response<AuthenticationResponse>(response, $"Autenticado {usuario.UserName}");



        }

        public async Task<Response<string>> RegisterAsync(RegisterRequest request, string origin)
        {
            var usuarioConElMismoUserName = await _userManager.FindByNameAsync(request.UserName);
            if (usuarioConElMismoUserName != null) 
            {
                throw new ApiException($"Usuario {request.UserName} el nombre de usuario ya fue registrado previamente");
            }
            var usuario = new ApplicationUser
            {
                Email = request.Email,
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                UserName = request.UserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var usuarioConElMismoCorreo = await _userManager.FindByEmailAsync(request.Email);
            if (usuarioConElMismoCorreo != null)
            {
                throw new ApiException($"El email {request.Email} ya fue registrado previamente");
            }
            else 
            {
                var result = await _userManager.CreateAsync(usuario, request.Password);
                if (result.Succeeded)
                {
                     await _userManager.AddToRoleAsync(usuario, Roles.Basic.ToString());
                    return new Response<string>(usuario.Id, message: $"Usuario registrado exitosamente. {request.UserName}"); 
                }
                else
                {
                    throw new ApiException($"{result.Errors}.");
                }
            }
        }

        private async Task<JwtSecurityToken> GenerateJWTTOKEN(ApplicationUser usuario)
        {
            var userClaims = await _userManager.GetClaimsAsync(usuario); 
            var roles  = await _userManager.GetRolesAsync(usuario);
            var rolesClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                rolesClaims.Add(new Claim("roles", roles[i])); 
            }
            string ipAddress = IpHelper.GetIpAddress();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim("uid",usuario.Id),
                new Claim("ip ",ipAddress),
            }
            .Union(userClaims)
            .Union(rolesClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials
                );
            return jwtSecurityToken;
        }

        private RefreshToken GenerateRefreshToken(string ipAdress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now,
                CreatedByIp = ipAdress

            };
        }

        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }
    }




}
