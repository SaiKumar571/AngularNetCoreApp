using System.Threading.Tasks;
using API.Entities;

namespace API.IServices
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser appUser);
    }
}