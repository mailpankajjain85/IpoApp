using IpoApp.Core.Services;
using IpoApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IpoApp.API.Controller
{
    [ApiController]
    [Authorize]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<TransactionResponse>> CreateTransaction(
            [FromBody] CreateTransactionRequest request)
        {
            try
            {
                var transaction = await _transactionService.CreateTransactionAsync(request);
                return CreatedAtAction(
                    nameof(GetTransaction),
                    new { id = transaction.TransactionID },
                    transaction);
            }
            catch (Exception ex) when (ex is KeyNotFoundException or InvalidOperationException)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionResponse>> GetTransaction(Guid id)
        {
            var transaction = await _transactionService.GetTransactionAsync(id);
            return transaction != null ? Ok(transaction) : NotFound();
        }

        [Authorize]
        [HttpGet("client/{clientShortCode}")]
        public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetClientTransactions(
            string clientShortCode)
        {
            var transactions = await _transactionService.GetClientTransactionsAsync(clientShortCode);
            return Ok(transactions);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelTransaction(Guid id)
        {
            try
            {
                await _transactionService.DeleteTransactionAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
