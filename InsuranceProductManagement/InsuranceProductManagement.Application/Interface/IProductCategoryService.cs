
using InsuranceProductManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceProductManagement.Application.Interface
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategoryDTO>> GetAllAsync();

        Task<ProductCategoryDTO?> GetByIdAsync(int id);

        Task<ProductCategoryDTO> CreateAsync(ProductCategoryDTO dto);

        Task<bool> UpdateAsync(int id, ProductCategoryDTO dto);

        Task<bool> DeleteAsync(int id);
    }
}
