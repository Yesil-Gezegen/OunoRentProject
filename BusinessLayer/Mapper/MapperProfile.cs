using AutoMapper;
using Shared.DTO.Category.Response;
using EntityLayer.Entities;
using Shared.DTO.Address.Response;
using Shared.DTO.User.Response;
using Shared.DTO.Authentication.Response;
using Shared.DTO.Slider.Response;
using Shared.DTO.Blog.Response;
using Shared.DTO.Blog.Request;
using Shared.DTO.Brand.Response;
using Shared.DTO.Channel.Response;
using Shared.DTO.FAQ.Response;
using Shared.DTO.ContactForm.Response;
using Shared.DTO.Contract.Response;
using Shared.DTO.Feature.Response;
using Shared.DTO.FeaturedCategories.Response;
using Shared.DTO.FooterItem.Response;
using Shared.DTO.SubCategory.Response;
using Shared.DTO.MenuItem.Response;
using Shared.DTO.Price.Response;
using Shared.DTO.UserContracts.Response;
using Shared.DTO.Warehouse.Response;
using Shared.DTO.WarehouseConnection.Response;

namespace BusinessLayer.Mapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        #region Category

        CreateMap<Category, GetCategoriesResponse>();
        CreateMap<Category, GetCategoryResponse>();
        CreateMap<Category, CategoryResponse>();

        #endregion

        #region Slider

        CreateMap<Slider, SliderResponse>();
        CreateMap<Slider, GetSlidersResponse>();
        CreateMap<Slider, GetSliderResponse>();

        #endregion

        #region User

        CreateMap<User, UserResponse>();
        CreateMap<User, GetUserResponse>();
        CreateMap<User, GetUsersResponse>();
        CreateMap<User, UserDetailsResponse>();

        #endregion

        #region SubCategory

        CreateMap<SubCategory, SubCategoryResponse>();
        CreateMap<SubCategory, GetSubCategoriesResponse>();
        CreateMap<SubCategory, GetSubCategoryResponse>();

        #endregion

        #region Blog

        CreateMap<Blog, BlogResponse>();
        CreateMap<Blog, GetBlogResponse>();
        CreateMap<Blog, GetBlogsResponse>();

        #endregion

        #region MenuItem
        
        CreateMap<MenuItem, MenuItemResponse>();
        CreateMap<MenuItem, GetMenuItemResponse>();
        CreateMap<MenuItem, GetMenuItemsResponse>();

        #endregion

        #region FeaturedCategory

        CreateMap<FeaturedCategory, FeaturedCategoryResponse>();
        CreateMap<FeaturedCategory, GetFeaturedCategoryResponse>()
            .ForMember(dest => dest.GetCategoryResponse, opt => opt.MapFrom(src => src.Category));
        CreateMap<FeaturedCategory, GetFeaturedCategoriesResponse>()
            .ForMember(dest => dest.GetCategoryResponse, opt => opt.MapFrom(src => src.Category));

        #endregion

        #region FooterItem

        CreateMap<FooterItem, FooterItemResponse>();
        CreateMap<FooterItem, GetFooterItemsResponse>();
        CreateMap<FooterItem, GetFooterItemResponse>();

        #endregion

        #region ContactForm

        CreateMap<ContactForm, ContactFormResponse>();
        CreateMap<ContactForm, GetContactFormsResponse>();
        CreateMap<ContactForm, GetContactFormResponse>();

        #endregion

        #region Brand

        CreateMap<Brand, BrandResponse>();
        CreateMap<Brand, GetBrandsResponse>();
        CreateMap<Brand, GetBrandResponse>();

        #endregion

        #region FAQ

        CreateMap<FAQ, FAQResponse>();
        CreateMap<FAQ, GetFAQResponse>();
        CreateMap<FAQ, GetFAQsResponse>();

        #endregion

        #region Feature

        CreateMap<Feature, FeatureResponse>();
        CreateMap<Feature, GetFeatureResponse>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.SubCategory, opt => opt.MapFrom(src => src.SubCategory));
        CreateMap<Feature, GetFeaturesResponse>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.SubCategory, opt => opt.MapFrom(src => src.SubCategory));

        #endregion

        #region Price

        CreateMap<Price, PriceResponse>();
        CreateMap<Price, GetPriceResponse>();
        CreateMap<Price, GetPricesResponse>();

        #endregion

        #region Contract

        CreateMap<Contract, ContractResponse>();
        CreateMap<Contract, GetContractResponse>();
        CreateMap<Contract, GetContractsResponse>();

        #endregion

        #region Address

        CreateMap<Address, AddressResponse>();
        CreateMap<Address, GetAddressResponse>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
        CreateMap<Address, GetAddressesResponse>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

        #endregion

        #region Channel

        CreateMap<Channel, ChannelResponse>();
        CreateMap<Channel, GetChannelsResponse>();
        CreateMap<Channel, GetChannelResponse>();


        #endregion

        #region UserContract

        CreateMap<UserContract, UserContractResponse>();
        CreateMap<UserContract, GetUserContractResponse>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract));
        CreateMap<UserContract, GetUserContractsResponse>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract));

        #endregion

        #region Warehouse

        CreateMap<Warehouse, WarehouseResponse>();
        CreateMap<Warehouse, GetWarehousesResponse>();
        CreateMap<Warehouse, GetWarehouseResponse>();

        #endregion

        #region WarehouseConnection

        CreateMap<WarehouseConnection, WarehouseConnectionResponse>();

        #endregion
    }
}