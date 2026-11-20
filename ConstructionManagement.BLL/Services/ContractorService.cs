using ConstructionManagement.DAL.Context;
using ConstructionManagement.DAL.Entities;
using ConstructionManagement.BLL.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManagement.BLL.Services
{
    public class ContractorService
    {
        private readonly ConstructionDbContext _context;

        public ContractorService(ConstructionDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateContractorAsync(
            CreateContractorRequest request)
        {
            // Check whether user exists and is active

            var user = await _context.Users
        .FirstOrDefaultAsync(x =>
            x.UserId == request.UserId &&
            x.StatusId == 1 &&
            x.IsActive &&
            x.Role == "Contractor");
            if (user == null)
                throw new Exception("User not found.");

            // Check whether user is already a contractor
            var existingContractor = await _context.Contractors
                .AnyAsync(x => x.UserId == request.UserId);

            if (existingContractor)
                throw new Exception("Contractor already exists.");

            var contractor = new Contractor
            {
                UserId = request.UserId,
                ContractorCompany = request.ContractorCompany,
                IsActive = true
            };

            _context.Contractors.Add(contractor);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateContractorAsync(int contractorId)
        {
            var contractor = await _context.Contractors
                .FirstOrDefaultAsync(x =>
                    x.ContractorId == contractorId &&
                    x.IsActive);

            if (contractor == null)
                return false;

            contractor.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<object>> GetAllContractorsAsync()
        {
            var contractors = await _context.Contractors
                     .Where(x =>
           x.IsActive &&
           x.User.IsActive &&
           x.User.StatusId == 1 &&
           x.User.Role == "Contractor")
       .Select(x => new
       {
           x.ContractorId,
           x.UserId,
           x.User.FullName,
           x.User.Phone,
           x.User.Email,
           x.ContractorCompany
       })
       .ToListAsync();

            return contractors.Cast<object>().ToList();
        }

        public async Task<bool> UpdateContractorAsync(
    int contractorId,
    UpdateContractorRequest request)
        {
            var contractor = await _context.Contractors
                .FirstOrDefaultAsync(x =>
                    x.ContractorId == contractorId &&
                    x.IsActive);

            if (contractor == null)
                return false;

            contractor.ContractorCompany = request.ContractorCompany;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}