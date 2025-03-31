using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IpoApp.Models.Entities;
using IpoApp.Repository;

namespace IpoApp.Core.Services
{
    public interface ITenantOrgService
    {
        Task<TenantOrg> GetTenantOrgByIdAsync(Guid id);
        Task<IEnumerable<TenantOrg>> GetAllTenantOrgsAsync();
        Task CreateTenantOrgAsync(TenantOrgDto tenantOrg);
        Task UpdateTenantOrgAsync(Guid id, TenantOrgDto tenantOrg);
        Task DeleteTenantOrgAsync(Guid id);
    }

    public class TenantOrgService : ITenantOrgService
    {
        private readonly ITenantOrgRepository _repository;

        public TenantOrgService(ITenantOrgRepository repository)
        {
            _repository = repository;
        }

        public async Task<TenantOrg> GetTenantOrgByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<TenantOrg>> GetAllTenantOrgsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task CreateTenantOrgAsync(TenantOrgDto tenantOrgDto)
        {
            if (await _repository.ExistsByShortCodeAsync(tenantOrgDto.OrgShortCode))
            {
                throw new InvalidOperationException("Organization with this short code already exists");
            }

            
            await _repository.CreateAsync(tenantOrgDto);
            
        }

        public async Task UpdateTenantOrgAsync(Guid id, TenantOrgDto tenantOrg)
        {
            var existingOrg = await _repository.GetByIdAsync(id);
            if (existingOrg == null)
            {
                throw new KeyNotFoundException("Organization not found");
            }

            if (existingOrg.OrgShortCode != tenantOrg.OrgShortCode &&
                await _repository.ExistsByShortCodeAsync(tenantOrg.OrgShortCode))
            {
                throw new InvalidOperationException("Another organization already uses this short code");
            }
            existingOrg.OrgShortCode = tenantOrg.OrgShortCode;
            existingOrg.OrgName = tenantOrg.OrgName;
            existingOrg.OrgPhoneNumber = tenantOrg.OrgPhoneNumber;
            await _repository.UpdateAsync(existingOrg);
        }

        public async Task DeleteTenantOrgAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
