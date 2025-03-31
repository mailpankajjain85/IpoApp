using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IpoApp.Repository;

namespace IpoApp.Core.Services
{
    public interface IClientService
    {
        Task<ClientResponse> CreateClientAsync(ClientCreateRequest request);
        Task<ClientResponse> GetClientAsync(Guid id);
    }
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepo;
        private readonly ITenantOrgRepository _orgRepo;

        public ClientService(IClientRepository clientRepo, ITenantOrgRepository orgRepo)
        {
            _clientRepo = clientRepo;
            _orgRepo = orgRepo;
        }

        public async Task<ClientResponse> CreateClientAsync(ClientCreateRequest request)
        {
            // Validate org exists
            if (!await _orgRepo.ExistsByShortCodeAsync(request.OrgShortCode))
                throw new KeyNotFoundException("Organization not found");

            // Validate client short code unique
            if (await _clientRepo.ShortCodeExistsAsync(request.OrgShortCode, request.ClientShortCode))
                throw new InvalidOperationException("Client short code already exists");

            // Manual mapping
            var client = new Client
            {
                ClientID = Guid.NewGuid(),
                OrgShortCode = request.OrgShortCode,
                ClientShortCode = request.ClientShortCode,
                FullName = request.FullName,
                Email = request.Email,
                Mobile = request.Mobile,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _clientRepo.CreateAsync(client);

            // Manual mapping to response
            return new ClientResponse
            {
                ClientID = client.ClientID,
                ClientShortCode = client.ClientShortCode,
                FullName = client.FullName,
                Email = client.Email,
                Mobile = client.Mobile,
                IsActive = client.IsActive,
                CreatedAt = client.CreatedAt
            };
        }

        public async Task<ClientResponse> GetClientAsync(Guid id)
        {
            var client = await _clientRepo.GetByIdAsync(id);
            if (client == null) return null;

            return new ClientResponse
            {
                ClientID = client.ClientID,
                ClientShortCode = client.ClientShortCode,
                FullName = client.FullName,
                Email = client.Email,
                Mobile = client.Mobile,
                IsActive = client.IsActive,
                CreatedAt = client.CreatedAt
            };
        }

        
    }
}
