using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace IpoApp.Repository
{
    public interface IClientRepository
    {
        Task<Client> GetByIdAsync(Guid id);
        Task<Client> GetByShortCodeAsync(string orgShortCode, string clientShortCode);
        Task CreateAsync(Client client);
        Task UpdateAsync(Client client);
        Task<bool> ShortCodeExistsAsync(string orgShortCode, string clientShortCode);
    }

    public class ClientRepository : IClientRepository
    {
        private readonly DatabaseContext _context;

        public ClientRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Client client)
        {
            using (var connection = _context.CreateConnection())
            {

                // Example: const string sql = @"INSERT INTO ClientMaster (ClientID, OrgShortCode, ClientShortCode, ...) VALUES (@ClientID, @OrgShortCode, @ClientShortCode, ...)";


                const string sql = @"INSERT INTO ClientMaster (orgshortcode, clientshortcode, fullname, email, mobile) VALUES (@OrgShortCode, @ClientShortCode, @FullName, @Email, @Mobile)";

                await connection.ExecuteAsync(sql, client);
            }
        }

        // implementing the GetByIdAsync method
        public async Task<Client> GetByIdAsync(Guid id)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"SELECT * FROM ClientMaster WHERE ClientID = @ClientID";
                return await connection.QuerySingleOrDefaultAsync<Client>(sql, new { ClientID = id });
            }
        }

        public async Task<Client> GetByShortCodeAsync(string orgShortCode, string clientShortCode)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"SELECT * FROM ClientMaster WHERE OrgShortCode = @OrgShortCode AND ClientShortCode = @ClientShortCode";
                return await connection.QuerySingleOrDefaultAsync<Client>(sql, new { OrgShortCode = orgShortCode, ClientShortCode = clientShortCode });
            }
        }
        public async Task UpdateAsync(Client client)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"UPDATE ClientMaster SET ... WHERE ClientID = @ClientID";
                await connection.ExecuteAsync(sql, client);
            }
        }
        public async Task<bool> ShortCodeExistsAsync(string orgShortCode, string clientShortCode)
        {
            using (var connection = _context.CreateConnection())
            {
                const string sql = @"SELECT COUNT(*) FROM ClientMaster WHERE OrgShortCode = @OrgShortCode AND ClientShortCode = @ClientShortCode";
                var count = await connection.ExecuteScalarAsync<int>(sql, new { OrgShortCode = orgShortCode, ClientShortCode = clientShortCode });
                return count > 0;
            }
        }
    }
}
