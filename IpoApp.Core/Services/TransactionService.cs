using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using IpoApp.Models.Entities;
using IpoApp.Repository;

namespace IpoApp.Core.Services
{
    // Services/ITransactionService.cs
    public interface ITransactionService
    {
        Task<TransactionResponse> CreateTransactionAsync(CreateTransactionRequest request);
        Task<TransactionResponse> GetTransactionAsync(Guid id);
        Task<IEnumerable<TransactionResponse>> GetClientTransactionsAsync(string clientShortCode);
        Task DeleteTransactionAsync(Guid transactionId);
        Task<TransactionResponse> UpdateTransactionAsync(UpdateTransactionRequest request);
    }

    // Services/TransactionService.cs
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionMasterRepository _transactionRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IIpoMasterRepository _ipoRepo;
        private readonly ICurrentContext _context;

        public TransactionService(ITransactionMasterRepository transactionMasterRepository, IClientRepository clientRepo, IIpoMasterRepository ipoRepo, ICurrentContext context)
        {
            _transactionRepo = transactionMasterRepository;
            _clientRepo = clientRepo;
            _ipoRepo = ipoRepo;
            _context = context;
        }

        public async Task<TransactionResponse> CreateTransactionAsync(CreateTransactionRequest request)
        {

            // Validate client exists
            var client = await _clientRepo.GetByShortCodeAsync(_context.OrgShortCode, request.ClientShortCode);
            if (client == null) throw new KeyNotFoundException("Client not found");

            // Validate IPO exists
            var ipo = await _ipoRepo.GetByIdAsync(request.IPOId);
            if (ipo == null) throw new KeyNotFoundException("IPO not found");

            // Create transaction
            var transaction = new TransactionMaster
            {
                OrgShortCode = _context.OrgShortCode,
                ClientShortCode = request.ClientShortCode,
                IPOId = request.IPOId,
                SaudaType = Enum.Parse<SaudaType>(request.SaudaType),
                AppType = Enum.Parse<AppType>(request.AppType),
                TransactionType = Enum.Parse<TransactionType>(request.TransactionType),
                Quantity = request.Quantity,
                Price = request.Price,
                CreatedBy = _context.Username
            };

            await _transactionRepo.CreateAsync(transaction);

            return MapToResponse(transaction);
        }

        public async Task<TransactionResponse> UpdateTransactionAsync(UpdateTransactionRequest request)
        {
            // write code here

            // Get transaction
            var transaction = await _transactionRepo.GetByIdAsync(request.TransactionID, _context.OrgShortCode);
            if (transaction == null || transaction.OrgShortCode != _context.OrgShortCode)
                throw new KeyNotFoundException("Transaction not found");

            // Update transaction details
            transaction.ClientShortCode = request.ClientShortCode;
            transaction.IPOId = request.IPOId;
            transaction.SaudaType = Enum.Parse<SaudaType>(request.SaudaType);
            transaction.AppType = Enum.Parse<AppType>(request.AppType);
            transaction.TransactionType = Enum.Parse<TransactionType>(request.TransactionType);
            transaction.Quantity = request.Quantity;
            transaction.Price = request.Price;
            transaction.ModifiedBy = _context.Username;

            await _transactionRepo.UpdateAsync(transaction);

            return MapToResponse(transaction);

        }

        public async Task<TransactionResponse> GetTransactionAsync(Guid id)
        {
            // Get transaction with org validation
            var transaction = await _transactionRepo.GetByIdAsync(id, _context.OrgShortCode);

            if (transaction == null || transaction.OrgShortCode != _context.OrgShortCode)
                throw new KeyNotFoundException("Transaction not found");

            // Authorization - Clients can only see their own transactions
            if (_context.Role == "CLIENT" && transaction.ClientShortCode != _context.ClientShortCode)
                throw new UnauthorizedAccessException();

            return MapToResponse(transaction);
        }

        public async Task<IEnumerable<TransactionResponse>> GetClientTransactionsAsync(string clientShortCode)
        {
            // Validate client exists and belongs to org
            var client = await _clientRepo.GetByShortCodeAsync(_context.OrgShortCode, clientShortCode);
            if (client == null) throw new KeyNotFoundException("Client not found");

            // Authorization - Clients can only request their own data
            if (_context.Role == "CLIENT" && clientShortCode != _context.ClientShortCode)
                throw new UnauthorizedAccessException();

            var transactions = await _transactionRepo.GetByClientAsync(
                _context.OrgShortCode,
                clientShortCode);

            return transactions.Select(MapToResponse);
        }

        public async Task DeleteTransactionAsync(Guid transactionId)
        {

            var transaction = await _transactionRepo.GetByIdAsync(transactionId, _context.OrgShortCode);

            // Validation
            if (transaction == null || transaction.OrgShortCode != _context.OrgShortCode)
                throw new KeyNotFoundException("Transaction not found");

            // Authorization - Clients can only cancel their own transactions
            if (_context.Role == "CLIENT")
                throw new UnauthorizedAccessException();

            // Update status
            await _transactionRepo.DeleteTransactionAsync(transactionId, _context.OrgShortCode);
        }

        private TransactionResponse MapToResponse(TransactionMaster transaction)
        {
            return new TransactionResponse
            {
                TransactionID = transaction.TransactionID,
                ClientShortCode = transaction.ClientShortCode,
                IPOId = transaction.IPOId.ToString(),
                SaudaType = transaction.SaudaType,
                Quantity = transaction.Quantity,
                Price = transaction.Price,
                TransactionDate = transaction.TransactionDate
            };
        }

        // Other methods...
    }
}
