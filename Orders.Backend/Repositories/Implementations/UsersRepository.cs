using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Implementations
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DataContext _context;

        public UsersRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ActionResponse<User>> GetAsync(string email)
        {
            var user = await _context.Users
                .Include(u => u.City!)
                .ThenInclude(c => c.State!)
                .ThenInclude(s => s.Country)
                .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                return new ActionResponse<User>
                {
                    WasSuccess = false,
                    Message = "Usuario no encontrado"
                };
            }

            return new ActionResponse<User>
            {
                WasSuccess = true,
                Result = user
            };
        }
    }
}