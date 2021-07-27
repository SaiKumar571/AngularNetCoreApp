using System.Linq;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser,MemberDTO>()
            .ForMember(dest=>dest.PhotoUrl,memberOptions=>memberOptions.MapFrom(sourceMember=>sourceMember.Photos.FirstOrDefault(x=>x.IsMain).URL))
            .ForMember(destinationMember=>destinationMember.Age,memberOptions=>memberOptions.MapFrom(sourceMember=>sourceMember.DateOfBirth.CalculateAge()));
            CreateMap<Photo,PhotoDTO>();
            CreateMap<MemberUpdateDTO,AppUser>();
        }
    }
}