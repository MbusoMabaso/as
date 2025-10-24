using System.Collections.Generic;
using System.Threading.Tasks;
using CMCS.Models;
using Microsoft.AspNetCore.Http;

namespace ClaimApp.Services
{
    public interface IClaimService
    {
        Task<Claim> CreateAsync(Claim claim, IFormFile file);
        Task<Claim> GetAsync(int id);
        Task ApproveAsync(int id, string approverId);
        Task RejectAsync(int id, string approverId, string reason = null);
        Task<IEnumerable<Claim>> GetPendingAsync();
    }
}
