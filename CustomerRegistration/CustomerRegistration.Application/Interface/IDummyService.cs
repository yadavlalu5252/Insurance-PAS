using CustomerRegistration.Application.DTO;

namespace CustomerRegistration.Application.Interface
{
    public interface IDummyService
    {
        DummyDTO GetDummy();
        DummyDTO CreateDummy(DummyDTO dto);
        void TestException();
    }
}
