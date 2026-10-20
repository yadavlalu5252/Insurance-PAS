using AutoMapper;
using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;
using InsuranceProductManagement.Domain.Entities;
using InsuranceProductManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;

namespace InsuranceProductManagement.Infrastructure.Repositories
{
        public class ProductCategoryService : IProductCategoryService
        {
            private readonly AppDbContext db;
            private readonly IMapper mapper;

            public ProductCategoryService(AppDbContext db,IMapper mapper)
            {
                this.db = db;
                this.mapper = mapper;
            }

            public async Task<List<ProductCategoryDTO>> GetAllAsync()
            {
                var categories = await db.ProductCategories.ToListAsync();

                return mapper.Map<List<ProductCategoryDTO>>(categories);
            }

            public async Task<ProductCategoryDTO?> GetByIdAsync(int id)
            {
                var category = await db.ProductCategories.FirstOrDefaultAsync(c => c.category_id == id);

                if (category == null)
                    return null;

                return mapper.Map<ProductCategoryDTO>(category);
            }

            public async Task<ProductCategoryDTO> CreateAsync(ProductCategoryDTO dto)
            {
                var category = mapper.Map<ProductCategory>(dto);

                if (string.IsNullOrWhiteSpace(category.status))
                    category.status = "Active";

                db.ProductCategories.Add(category);
                await db.SaveChangesAsync();

                return mapper.Map<ProductCategoryDTO>(category);
            }

            public async Task<bool> UpdateAsync(int id, ProductCategoryDTO dto)
            {
                var category = await db.ProductCategories.FirstOrDefaultAsync(c => c.category_id == id);

                if (category == null)
                    return false;

                mapper.Map(dto, category);

                await db.SaveChangesAsync();

                return true;
            }

            public async Task<bool> DeleteAsync(int id)
            {
                var category = await db.ProductCategories.FirstOrDefaultAsync(c => c.category_id == id);

                if (category == null)
                    return false;

                category.status = "Inactive";

                await db.SaveChangesAsync();

                return true;
            }
        }
    }


