using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        Task<ActionResponse<User>> GetAsync(string email);
    }
}