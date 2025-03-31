using System;
using System.Threading.Tasks;
using Dapper;
using IpoApp.Models.Entities;

namespace IpoApp.Repository
{
    public class AuthRepository
    {
        private readonly DatabaseContext _context;

        public AuthRepository(DatabaseContext context)
        {
            _context = context;
        }
        //public async Task SaveUserAsync(UserInformation user)
        //{
        //    if (user == null)
        //    {
        //        throw new ArgumentNullException(nameof(user), "User cannot be null.");
        //    }
        //    // Simulate saving user to a database
        //    try
        //    {
        //        using (var connection = _context.CreateConnection())
        //        {
        //            var sql = "INSERT INTO Users (Name, Email, UserName, Phone, PasswordHash, OrgShortCode, UserRole) VALUES (@Name, @Email, @UserName, @Phone, @Password, @OrgShortCode, @UserRole)";
        //            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password); // Hash the password before saving
        //            await connection.ExecuteAsync(sql, user);
        //            Console.WriteLine($"User {user.UserName} saved successfully.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());

        //    }
        //    Console.WriteLine($"User {user.UserName} saved successfully.");

        //}
        //public async Task<UserInformation> GetUserByUsernameAsync(string username, string OrgShortCode)
        //{
        //    using (var connection = _context.CreateConnection())
        //    {
        //        var sql = "SELECT * FROM Users WHERE UserName = @UserName and OrgShortCode = @OrgShortCode";
        //        return await connection.QuerySingleOrDefaultAsync<UserInformation>(sql, new { UserName = username, OrgShortCode = OrgShortCode });
        //    }
        //}
    }
}