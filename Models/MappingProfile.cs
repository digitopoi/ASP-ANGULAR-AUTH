using AutoMapper;
using ASP_Angular_Auth.Models;
using ASP_Angular_Auth.Models.ResourceModels;

namespace ASP_Angular_Auth.Models
{
    public class MappingProfile : Profile
    {
       public MappingProfile()
       {
           CreateMap<User, UserResource>();
           CreateMap<UserResource, User>();
       } 
    }
}