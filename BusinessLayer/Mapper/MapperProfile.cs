using AutoMapper;
using Shared.DTO.Category.Response;
using EntityLayer.Entities;
using Shared.DTO.User.Response;
using Shared.DTO.Authentication.Response;
using Shared.DTO.Slider.Response;
using Shared.DTO.Blog.Response;
using Shared.DTO.Blog.Request;
using Shared.DTO.FAQ.Response;
using Shared.DTO.ContactForm.Response;
using Shared.DTO.FeaturedCategories.Response;
using Shared.DTO.FooterItem.Response;
using Shared.DTO.SubCategory.Response;
using Shared.DTO.MenuItem.Response;

namespace BusinessLayer.Mapper;

public class MapperProfile : Profile
{
	public MapperProfile()
	{
		CreateMap<Category, GetCategoriesResponse>();
		CreateMap<Category, GetCategoryResponse>();
		CreateMap<Category, CategoryResponse>();

		CreateMap<Slider, SliderResponse>();
		CreateMap<Slider, GetSlidersResponse>();
		CreateMap<Slider, GetSliderResponse>();

		CreateMap<User, UserResponse>();
		CreateMap<User, GetUserResponse>();
		CreateMap<User, GetUsersResponse>();
		CreateMap<User, UserDetailsResponse>();

		CreateMap<SubCategory, SubCategoryResponse>();
		CreateMap<SubCategory, GetSubCategoriesResponse>();
		CreateMap<SubCategory, GetSubCategoryResponse>();

		CreateMap<Blog, BlogResponse>();
		CreateMap<Blog, GetBlogResponse>()
				.ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(src => src.SubCategory.Name));
		CreateMap<Blog, GetBlogsResponse>()
				.ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(src => src.SubCategory.Name));

		CreateMap<MenuItem, MenuItemResponse>();
		CreateMap<MenuItem, GetMenuItemResponse>();
		CreateMap<MenuItem, GetMenuItemsResponse>();
		
		CreateMap<FeaturedCategory, FeaturedCategoryResponse>();
		CreateMap<FeaturedCategory, GetFeaturedCategoryResponse>()
			.ForMember(dest => dest.GetCategoryResponse, opt => opt.MapFrom(src => src.Category));
		CreateMap<FeaturedCategory, GetFeaturedCategoriesResponse>()
			.ForMember(dest => dest.GetCategoryResponse, opt => opt.MapFrom(src => src.Category));

		CreateMap<FooterItem, FooterItemResponse>();
		CreateMap<FooterItem, GetFooterItemsResponse>();
		CreateMap<FooterItem, GetFooterItemResponse>();

		CreateMap<ContactForm, ContactFormResponse>();
		CreateMap<ContactForm, GetContactFormsResponse>();
		CreateMap<ContactForm, GetContactFormResponse>();

		CreateMap<FAQ, FAQResponse>();
		CreateMap<FAQ, GetFAQResponse>();
		CreateMap<FAQ, GetFAQsResponse>();
	}
}
