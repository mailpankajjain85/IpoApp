using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using IpoApp.Models.Entities;
using Npgsql;

namespace IpoApp.Repository
{
    // Repositories/IIpoMasterRepository.cs
    public interface IIpoMasterRepository
    {
        Task<IpoMaster> GetByIdAsync(Guid id);
        Task<IEnumerable<IpoMaster>> GetByOrgAsync(string orgShortCode);
        Task CreateAsync(IpoMaster ipo);
        Task UpdateAsync(IpoMaster ipo);
        Task<bool> ExistsAsync(string name, DateTime closingDate);
    }

    // Repositories/IpoMasterRepository.cs
    public class IpoMasterRepository : IIpoMasterRepository
    {
        private readonly DatabaseContext _context;

        public IpoMasterRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(IpoMaster ipo)
        {
            const string sql = @"
            INSERT INTO IpoMaster 
            (ID, Name, OrgShortCode, ClosingDate, ListingDate, Registrar, IPOType, HisabDone, CreatedBy, UpdatedBy)
            VALUES 
            (@ID, @Name, @OrgShortCode, @ClosingDate, @ListingDate, @Registrar, @IPOType, @HisabDone, @CreatedBy, @CreatedBy)";

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(sql, ipo);
            }
        }

        public async Task<IpoMaster> GetByIdAsync(Guid id)
        {
            const string sql = "SELECT * FROM IpoMaster WHERE ID = @id";
            using (var conn = _context.CreateConnection())
            {
                return await conn.QueryFirstOrDefaultAsync<IpoMaster>(sql, new { id });
            }


        }

        //implement other methods
        public async Task<IEnumerable<IpoMaster>> GetByOrgAsync(string orgShortCode)
        {
            const string sql = "SELECT * FROM IpoMaster WHERE OrgShortCode = @OrgShortCode";
            using (var conn = _context.CreateConnection())
            {
                return await conn.QueryAsync<IpoMaster>(sql, new { OrgShortCode = orgShortCode });
            }
        }
        public async Task UpdateAsync(IpoMaster ipo)
        {
            const string sql = @"
            UPDATE IpoMaster 
            SET Name = @Name, 
                OrgShortCode = @OrgShortCode, 
                ClosingDate = @ClosingDate, 
                ListingDate = @ListingDate, 
                Registrar = @Registrar, 
                IPOType = @IPOType, 
                HisabDone = @HisabDone, 
                ModifiedBy = @ModifiedBy, 
                ModifiedDate = @ModifiedDate
            WHERE ID = @ID";

            using (var conn = _context.CreateConnection())
            {
                await conn.ExecuteAsync(sql, ipo);
            }
        }
        public async Task<bool> ExistsAsync(string name, DateTime closingDate)
        {
            const string sql = "SELECT COUNT(*) FROM IpoMaster WHERE Name = @Name AND ClosingDate = @ClosingDate";
            using (var conn = _context.CreateConnection())
            {
                var count = await conn.ExecuteScalarAsync<int>(sql, new { Name = name, ClosingDate = closingDate });
                return count > 0;
            }
        }
    }
}
