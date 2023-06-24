using ChatGptApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using MongoDB.Driver;

namespace ChatGptApi.Services
{
    public class AuthService
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IConfiguration _configuration;
        private string Token;

        public AuthService(IOptions<GptApiDatabaseSettings> gptApiDatabaseSettings)
        {
            var mongoClient = new MongoClient(gptApiDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(gptApiDatabaseSettings.Value.DatabaseName);

            _usersCollection = mongoDatabase.GetCollection<User>(
                gptApiDatabaseSettings.Value.UsersCollectionName
            );

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            IConfiguration config = builder.Build();
            Token = config["Token"];
        }

        public async Task<List<User>> GetAsync()
        {
            return await _usersCollection.Find(_ => true).ToListAsync();
        }

        public async Task CreateAsync(User createUser)
        {
            await _usersCollection.InsertOneAsync(createUser);
        }

        public async Task UpdateAsync(string id, User updateUser)
        {
            await _usersCollection.ReplaceOneAsync(x => x.Id == id, updateUser);
        }

        // public User Register(UserDto userDto)
        // {
        //     string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
        //     user.Username = userDto.Username;
        //     user.PasswordHash = hashedPassword;
        //     return user;
        // }

        // public string Login(UserDto userDto)
        // {
        //     if (userDto.Username != user.Username)
        //     {
        //         return "User not found.";
        //     }

        //     if (!BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
        //     {
        //         return "Wrong password.";
        //     }

        //     string token = CreateToken(user.Username);
        //     return token;
        // }

        public string CreateToken(string username)
        {
            List<Claim> claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!)
            );

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
