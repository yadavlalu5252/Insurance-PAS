using CustomerRegistration.Application.DTO;
using CustomerRegistration.Application.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerRegistration.Infrastructure.Repository
{
    public class DummyService : IDummyService
    {
        public void AddDummy(DummyDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
