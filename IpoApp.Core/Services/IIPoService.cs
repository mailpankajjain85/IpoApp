using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IpoApp.Models.Entities;
using IpoApp.Repository;

namespace IpoApp.Core.Services
{
    // Services/IIpoService.cs
    public interface IIpoService
    {
        Task<IpoResponse> CreateIpoAsync(IpoCreateRequest request);
        Task<IpoResponse> GetIpoAsync(Guid id);
        Task<IEnumerable<IpoResponse>> GetIposByOrgAsync();
    }

    // Services/IpoService.cs
    public class IpoService : IIpoService
    {
        private readonly IIpoMasterRepository _ipoRepo;
        private readonly ITenantOrgRepository _orgRepo;
        private readonly IUserRepository _userRepo;
        private readonly ICurrentContext _context;

        public IpoService(IIpoMasterRepository ipoRepo, ITenantOrgRepository orgRepo, IUserRepository userRepo, ICurrentContext context)
        {
            _ipoRepo = ipoRepo;
            _orgRepo = orgRepo;
            _userRepo = userRepo;
            _context = context;
        }

        public async Task<IpoResponse> CreateIpoAsync(IpoCreateRequest request)
        {
            // Validate organization exists
            if (!await _orgRepo.ExistsByShortCodeAsync(_context.OrgShortCode))
                throw new KeyNotFoundException("Organization not found");

            // Validate creator user exists
            if (await _userRepo.GetByUsernameAsync(_context.Username, _context.OrgShortCode) == null)
                throw new KeyNotFoundException("User not found");

            // Check for duplicate IPO
            if (await _ipoRepo.ExistsAsync(request.Name, request.ClosingDate))
                throw new InvalidOperationException("IPO with same name and closing date already exists");

            // Manual mapping
            var ipo = new IpoMaster
            {
                ID = Guid.NewGuid(),
                Name = request.Name,
                OrgShortCode = _context.OrgShortCode,
                ClosingDate = request.ClosingDate,
                ListingDate = request.ListingDate,
                Registrar = request.Registrar,
                IPOType = request.IPOType,
                CreatedBy = _context.Username
            };

            await _ipoRepo.CreateAsync(ipo);

            // Manual mapping to response
            return new IpoResponse
            {
                ID = ipo.ID,
                Name = ipo.Name,
                OrgShortCode = ipo.OrgShortCode,
                ClosingDate = ipo.ClosingDate,
                ListingDate = ipo.ListingDate,
                Registrar = ipo.Registrar,
                IPOType = ipo.IPOType,
                HisabDone = ipo.HisabDone,
                CreatedDate = ipo.CreatedDate
            };
        }

        public async Task<IpoResponse> GetIpoAsync(Guid id)
        {
            var ipo = await _ipoRepo.GetByIdAsync(id);
            if (ipo == null) return null;

            return new IpoResponse
            {
                ID = ipo.ID,
                Name = ipo.Name,
                OrgShortCode = ipo.OrgShortCode,
                ClosingDate = ipo.ClosingDate,
                ListingDate = ipo.ListingDate,
                Registrar = ipo.Registrar,
                IPOType = ipo.IPOType,
                HisabDone = ipo.HisabDone,
                CreatedDate = ipo.CreatedDate
            };
        }
        public async Task<IEnumerable<IpoResponse>> GetIposByOrgAsync()
        {
            var ipos = await _ipoRepo.GetByOrgAsync(_context.OrgShortCode);
            return ipos.Select(ipo => new IpoResponse
            {
                ID = ipo.ID,
                Name = ipo.Name,
                OrgShortCode = ipo.OrgShortCode,
                ClosingDate = ipo.ClosingDate,
                ListingDate = ipo.ListingDate,
                Registrar = ipo.Registrar,
                IPOType = ipo.IPOType,
                HisabDone = ipo.HisabDone,
                CreatedDate = ipo.CreatedDate
            });
        }
    }
}
