using AutoMapper;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using ProductShop.Dtos.Export;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            //Import
            this.CreateMap<ImportUserDto, User>();
            this.CreateMap<ImportProductDto, Product>();
            this.CreateMap<ImportCategoryDto, Category>();
            this.CreateMap<ImportCategoriesProductsDto, CategoryProduct>();
            
        }
    }
}
