using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eCommerceBlazor_Business.Repository.IRepository;
using eCommerceBlazor_DataAccess;
using eCommerceBlazor_DataAccess.Data;
using eCommerceBlazor_Models;

namespace eCommerceBlazor_Business.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoryRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CategoryDTO> Create(CategoryDTO categoryDTO)
        {
            var category = _mapper.Map<CategoryDTO, Category>(categoryDTO);
            category.CreatedDate = DateTime.UtcNow;
            var addedCategory = _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return _mapper.Map<Category, CategoryDTO>(addedCategory.Entity);
        }

        public async Task<int> Delete(int id)
        {
            var categoryToDelete = await _context.Categories.FirstOrDefaultAsync(category => category.Id == id);
            if (categoryToDelete != null)
            {
                _context.Categories.Remove(categoryToDelete);
                await _context.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<CategoryDTO> Get(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(category => category.Id == id);
            if (category != null)
            {
                return _mapper.Map<Category, CategoryDTO>(category);
            }
            return new CategoryDTO();
        }

        public async Task<IEnumerable<CategoryDTO>> GetAll()
        {
            return _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryDTO>>(_context.Categories);
        }

        public async Task<CategoryDTO> Update(CategoryDTO categoryDTO)
        {
            var categoryFromDb = await _context.Categories.FirstOrDefaultAsync(category => category.Id == categoryDTO.Id);
            if (categoryFromDb != null)
            {
                categoryFromDb.Name = categoryDTO.Name;
                _context.Categories.Update(categoryFromDb);
                await _context.SaveChangesAsync();
                return _mapper.Map<Category, CategoryDTO>(categoryFromDb);
            }
            return categoryDTO;
        }

    }
}
