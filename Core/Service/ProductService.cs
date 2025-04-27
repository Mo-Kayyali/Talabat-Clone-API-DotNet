using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.ProductModule;
using Service.Specifications;
using ServiceAbstraction;
using Shared;
using Shared.DataTransferObjects.ProductModuleDtos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ProductService(IUnitOfWork _unitOfWork, IMapper _mapper) : IProductService
    {
        public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
        {
            var Repo = _unitOfWork.GetRepository<ProductBrand, int>();
            var Brands = await Repo.GetAllAsync();
            var BrandsDto = _mapper.Map<IEnumerable<ProductBrand>, IEnumerable<BrandDto>>(Brands);
            return BrandsDto;

        }

        public async Task<PaginatedResult<ProductDto>> GetAllProductsAsync(ProductQueryParams queryParams)
        {
            var Repo = _unitOfWork.GetRepository<Product, int>();
            var Specifications = new ProductWithBrandAndTypeSpecifications(queryParams);
            var AllProducts = await Repo.GetAllAsync(Specifications);
            var Data = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDto>>(AllProducts);
            var ProductCount = AllProducts.Count();
            var CountSpec = new ProductCountSpecification(queryParams);
            var TotalCount = await Repo.CountAsync(CountSpec);
            return new PaginatedResult<ProductDto>(queryParams.PageIndex, ProductCount, TotalCount, Data);
        }

        public async Task<IEnumerable<TypeDto>> GetAllTypesAsync()
        {
            var Types = await _unitOfWork.GetRepository<ProductType, int>().GetAllAsync();
            var TypesDto = _mapper.Map<IEnumerable<ProductType>, IEnumerable<TypeDto>>(Types);
            return TypesDto;
        }

        public async Task<ProductDto> GetProductByIdAsync(int Id)
        {
            var Specifications = new ProductWithBrandAndTypeSpecifications(Id);
            var Product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(Specifications);

            //Could Be Null

            if (Product is null)
            {
                throw new ProductNotFoundException(Id);
            }

            return _mapper.Map<Product, ProductDto>(Product);
        }
    }
}
