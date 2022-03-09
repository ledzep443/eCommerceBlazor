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
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductDTO> Create(ProductDTO productObjDTO)
        {
            var productObj = _mapper.Map<ProductDTO, Product>(productObjDTO);
            var addedProductObj = _context.Products.Add(productObj);
            await _context.SaveChangesAsync();

            return _mapper.Map<Product, ProductDTO>(addedProductObj.Entity);
        }

        public async Task<int> Delete(int id)
        {
            var productObj = await _context.Products.FirstOrDefaultAsync(price => price.Id == id);
            if (productObj != null)
            {
                _context.Products.Remove(productObj);
                await _context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<ProductDTO> Get(int id)
        {
            var productObj = await _context.Products.Include(product=>product.Category).Include(product=>product.ProductPrices).FirstOrDefaultAsync(product => product.Id == id);
            if (productObj != null)
            {
                return _mapper.Map<Product, ProductDTO>(productObj);
            }
            return new ProductDTO();
        }

        public async Task<IEnumerable<ProductDTO>> GetAll()
        {
            return _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(_context.Products.Include(product => product.Category).Include(product => product.ProductPrices));
        }

        public async Task<ProductDTO> Update(ProductDTO productDTO)
        {
            var productObjFromDb = await _context.Products.FirstOrDefaultAsync(product => product.Id == productDTO.Id);
            if (productObjFromDb != null)
            {
                productObjFromDb.Name = productDTO.Name;
                productObjFromDb.Description = productDTO.Description;
                productObjFromDb.ImageUrl = productDTO.ImageUrl;
                productObjFromDb.CategoryId = productDTO.CategoryId;
                productObjFromDb.Color = productDTO.Color;
                productObjFromDb.ShopFavorites = productDTO.ShopFavorites;
                productObjFromDb.CustomerFavorites = productDTO.CustomerFavorites;
                _context.Products.Update(productObjFromDb);
                await _context.SaveChangesAsync();
                return _mapper.Map<Product, ProductDTO>(productObjFromDb);
            }
            return productDTO;
        }
    }
}
