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
    public class ProductPriceRepository : IProductPriceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductPriceRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ProductPriceDTO> Create(ProductPriceDTO productPriceDTO)
        {
            var productPriceObj = _mapper.Map<ProductPriceDTO, ProductPrice>(productPriceDTO);

            var addedProductPriceObj = _context.ProductPrices.Add(productPriceObj);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductPrice, ProductPriceDTO>(addedProductPriceObj.Entity);
        }

        public async Task<int> Delete(int id)
        {
            var productPriceObj = await _context.ProductPrices.FirstOrDefaultAsync(price => price.Id == id);
            if (productPriceObj != null)
            {
                _context.ProductPrices.Remove(productPriceObj);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<ProductPriceDTO> Get(int id)
        {
            var productPriceObj = await _context.ProductPrices.FirstOrDefaultAsync(price => price.Id == id);
            if (productPriceObj != null)
            {
                return _mapper.Map<ProductPrice, ProductPriceDTO>(productPriceObj);
            }
            return new ProductPriceDTO();
        }

        public async Task<IEnumerable<ProductPriceDTO>> GetAll(int? id = null)
        {
            if (id!=null && id>0)
            {
                return _mapper.Map<IEnumerable<ProductPrice>, IEnumerable<ProductPriceDTO>>(_context.ProductPrices.Where(price => price.Id == id));
            }
            else
            {
                return _mapper.Map<IEnumerable<ProductPrice>, IEnumerable<ProductPriceDTO>>(_context.ProductPrices);
            }
        }

        public async Task<ProductPriceDTO> Update(ProductPriceDTO productPriceDTO)
        {
            var productPriceObjFromDb = await _context.ProductPrices.FirstOrDefaultAsync(price => price.Id == productPriceDTO.Id);
            if (productPriceObjFromDb != null)
            {
                productPriceObjFromDb.Price = productPriceDTO.Price;
                productPriceObjFromDb.Size = productPriceDTO.Size;
                productPriceObjFromDb.ProductId = productPriceDTO.ProductId;
                _context.ProductPrices.Update(productPriceObjFromDb);
                await _context.SaveChangesAsync();
                return _mapper.Map<ProductPrice, ProductPriceDTO>(productPriceObjFromDb);
            }
            return productPriceDTO;
        }
    }
}
