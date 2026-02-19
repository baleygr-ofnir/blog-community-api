using AutoMapper;
using blog_community_api.Contracts.Category;
using blog_community_api.Data.Entities;
using blog_community_api.Contracts.BlogPosts;
using blog_community_api.Contracts.Comments;

namespace blog_community_api.Mapping;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        // BlogPosts
        CreateMap<BlogPost, BlogPostResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.AuthorUsername, opt => opt.MapFrom(src => src.User.Username));
        
        CreateMap<BlogPostCreateRequest, BlogPost>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore());

        CreateMap<BlogPostUpdateRequest, BlogPost>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore());
        
        // Comments
        CreateMap<Comment, CommentResponse>();
        
        CreateMap<CommentCreateRequest, Comment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.BlogPostId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.BlogPost, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());
        
        // Categories
        CreateMap<Category, CategoryResponse>();
    }
}