using CustomerRegistration.Application.DTO;
using CustomerRegistration.Application.Interface;
using CustomerRegistration.Domain.Entities;
using CustomerRegistration.Infrastructure.Data;

namespace CustomerRegistration.Infrastructure.Repository
{
    public class DummyService : IDummyService
    {

        private readonly AppDbContext _context;

        public DummyService(AppDbContext context)
        {
            _context = context;
        }
        public DummyDTO GetDummy()
        {
            return new DummyDTO
            {
                d_id = 1,
                d_name = "Test Customer"
            };
        }

        public DummyDTO CreateDummy(DummyDTO dto)
        {
            return new DummyDTO
            {
                d_id = 2,
                d_name = dto.d_name
            };
        }

        public void TestException()
        {
            throw new Exception("This is a dummy exception for testing.");
        }

    
    }
}
