using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace IpoApp.Repository
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByUsernameAsync(string username, string orgShortCode);
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> UsernameExistsAsync(string username);
    }

    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _context;

        public UserRepository(DatabaseContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(User user)
        {
            const string sql = @"INSERT INTO UserMaster (OrgShortCode, UserName, PasswordHash, FullName, Email, Role, ClientID) VALUES ( @OrgShortCode, @UserName, @PasswordHash, @FullName, @Email, @Role, @ClientID)";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(sql, user);
            }
        }

        // Other implementations...
        public async Task<User> GetByIdAsync(Guid id)
        {
            const string sql = @"SELECT * FROM UserMaster WHERE UserID = @UserID";
            using (var conn = _context.CreateConnection())
            {
                return await conn.QuerySingleOrDefaultAsync<User>(sql, new { UserID = id });
            }
        }
        public async Task<User> GetByUsernameAsync(string username, string orgShortCode)
        {
            const string sql = @"SELECT * FROM UserMaster WHERE UserName = @UserName and OrgShortCode = @OrgShortCode";
            using (var conn = _context.CreateConnection())
            {
                return await conn.QuerySingleOrDefaultAsync<User>(sql, new { UserName = username, OrgShortCode = orgShortCode });
            }
        }
        public async Task UpdateAsync(User user)
        {
            const string sql = @"UPDATE UserMaster SET ... WHERE UserID = @UserID";
            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(sql, user);
            }
        }
        public async Task<bool> UsernameExistsAsync(string username)
        {
            const string sql = @"SELECT COUNT(*) FROM UserMaster WHERE UserName = @UserName";
            using (var conn = _context.CreateConnection())
            {
                var count = await conn.ExecuteScalarAsync<int>(sql, new { UserName = username });
                return count > 0;
            }
        }
    }
}
