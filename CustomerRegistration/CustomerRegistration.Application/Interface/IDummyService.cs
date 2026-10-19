using CustomerRegistration.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerRegistration.Application.Interface
{
    public interface IDummyService
    {
        void AddDummy(DummyDTO dto);
    }
}
