using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using IpoApp.Models.Entities;
using Npgsql;

namespace IpoApp.Repository
{
    public interface ITenantOrgRepository
    {
        Task<TenantOrg> GetByIdAsync(Guid id);
        Task<IEnumerable<TenantOrg>> GetAllAsync();
        Task CreateAsync(TenantOrgDto tenantOrg);
        Task UpdateAsync(TenantOrg tenantOrg);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsByShortCodeAsync(string shortCode);
    }

    public class TenantOrgRepository : ITenantOrgRepository
    {
        private readonly DatabaseContext _context;

        public TenantOrgRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<TenantOrg> GetByIdAsync(Guid id)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<TenantOrg>(
                "SELECT * FROM TenantOrg WHERE OrgID = @Id", new { Id = id });
            }
        }

        public async Task<IEnumerable<TenantOrg>> GetAllAsync()
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<TenantOrg>("SELECT * FROM TenantOrg");
            }
        }

        public async Task CreateAsync(TenantOrgDto tenantOrgDto)
        {
            const string sql = @"
                INSERT INTO TenantOrg (OrgName, OrgShortCode, OrgPhoneNumber)
                VALUES (@OrgName, @OrgShortCode, @OrgPhoneNumber)";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(sql, tenantOrgDto);
            }
        }

        public async Task UpdateAsync(TenantOrg tenantOrg)
        {
            const string sql = @"
                UPDATE TenantOrg 
                SET OrgName = @OrgName, 
                    OrgShortCode = @OrgShortCode, 
                    OrgPhoneNumber = @OrgPhoneNumber
                WHERE OrgID = @id";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(sql, tenantOrg);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync("DELETE FROM TenantOrg WHERE OrgID = @Id", new { Id = id });
            }
        }

        public async Task<bool> ExistsByShortCodeAsync(string shortCode)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<bool>(
                "SELECT 1 FROM TenantOrg WHERE OrgShortCode = @ShortCode LIMIT 1",
                new { ShortCode = shortCode });
            }
        }

    }
}