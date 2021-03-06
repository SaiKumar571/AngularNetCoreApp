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
            CreateMap<RegisterDTO,AppUser>();
            CreateMap<Message,MessageDTO>()
                    .ForMember(dest=>dest.SenderPhotoUrl,memberOptions=>memberOptions.MapFrom(src=>src.Sender.Photos.FirstOrDefault(x=>x.IsMain).URL))
                    .ForMember(dest=>dest.RecipientPhotoUrl,memberOptions=>memberOptions.MapFrom(src=>src.Recipient.Photos.FirstOrDefault(x=>x.IsMain).URL));
        }
    }
}