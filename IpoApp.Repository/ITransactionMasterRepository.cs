using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using IpoApp.Models.Entities;

namespace IpoApp.Repository
{
    // Repositories/ITransactionMasterRepository.cs
    public interface ITransactionMasterRepository
    {
        Task<TransactionMaster> GetByIdAsync(Guid id, string orgShortCode);
        Task<IEnumerable<TransactionMaster>> GetByClientAsync(string orgShortCode, string clientShortCode);
        Task CreateAsync(TransactionMaster transaction);
        Task DeleteTransactionAsync(Guid transactionId, string orgShortCode);
        Task UpdateAsync(TransactionMaster transaction);
        Task<IEnumerable<TransactionMaster>> GetByOrgAsync(string orgShortCode);
    }

    // Repositories/TransactionMasterRepository.cs
    public class TransactionMasterRepository : ITransactionMasterRepository
    {
        private readonly DatabaseContext _context;

        public TransactionMasterRepository(DatabaseContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(TransactionMaster transaction)
        {
            const string sql = @"
            INSERT INTO TransactionMaster 
            (TransactionID, OrgShortCode, ClientShortCode, IPOId, SaudaType, TransactionType, AppType,
             Quantity, Price, CreatedBy)
            VALUES 
            (@TransactionID, @OrgShortCode, @ClientShortCode, @IPOId, @SaudaType, @TransactionType, @AppType,
             @Quantity, @Price, @CreatedBy)";

            var parameters = new
            {
                transaction.TransactionID,
                transaction.OrgShortCode,
                transaction.ClientShortCode,
                transaction.IPOId,
                SaudaType = transaction.SaudaType.ToString(), // Convert enum to string
                TransactionType = transaction.TransactionType.ToString(), // Convert enum to string
                AppType = transaction.AppType.ToString(), // Convert enum to string
                transaction.Quantity,
                transaction.Price,
                transaction.CreatedBy
            };

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(sql, parameters);
            }
        }
        public async Task UpdateAsync(TransactionMaster transaction)
        {
            // Update the transaction in the database
            const string sql = @"
            UPDATE TransactionMaster 
            SET IPOId = @IPOId, 
                SaudaType = @SaudaType, TransactionType = @TransactionType, AppType = @AppType,
                Quantity = @Quantity, Price = @Price, ModifiedBy = @ModifiedBy
            WHERE TransactionID = @TransactionID AND OrgShortCode = @OrgShortCode AND clientShortCode = @ClientShortCode";

            var parameters = new
            {
                transaction.TransactionID,
                transaction.OrgShortCode,
                transaction.ClientShortCode,
                transaction.IPOId,
                SaudaType = transaction.SaudaType.ToString(), // Convert enum to string
                TransactionType = transaction.TransactionType.ToString(), // Convert enum to string
                AppType = transaction.AppType.ToString(), // Convert enum to string
                transaction.Quantity,
                transaction.Price,
                transaction.ModifiedBy
            };

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(sql, parameters);
            }
        }
        public async Task DeleteTransactionAsync(Guid transactionId, string orgShortCode)
        {
            const string sql = @"
            Delete TransactionMaster 
            WHERE TransactionID = @transactionId and OrgShortCode = @orgShortCode";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(sql, new { transactionId, orgShortCode });
            }
        }

        public async Task<TransactionMaster> GetByIdAsync(Guid id, string orgShortCode)
        {
            const string sql = @"
            SELECT * 
            FROM TransactionMaster 
            WHERE TransactionID = @id";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<TransactionMaster>(sql, new { id, orgShortCode });
            }
        }

        public async Task<IEnumerable<TransactionMaster>> GetByClientAsync(string orgShortCode, string clientShortCode)
        {
            const string sql = @"
            SELECT * 
            FROM TransactionMaster 
            WHERE OrgShortCode = @orgShortCode AND ClientShortCode = @clientShortCode";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<TransactionMaster>(sql, new { orgShortCode, clientShortCode });
            }
        }

        public async Task<IEnumerable<TransactionMaster>> GetByOrgAsync(string orgShortCode)
        {
            const string sql = @"
            SELECT * 
            FROM TransactionMaster 
            WHERE OrgShortCode = @orgShortCode";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<TransactionMaster>(sql, new { orgShortCode });
            }
        }
    }
}
