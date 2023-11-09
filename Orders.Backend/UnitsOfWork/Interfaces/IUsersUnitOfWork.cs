using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Interfaces
{
    public interface IUsersUnitOfWork
    {
        Task<ActionResponse<User>> GetAsync(string email);
    }
}