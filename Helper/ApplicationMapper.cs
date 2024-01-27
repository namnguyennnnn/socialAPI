using AutoMapper;
using DoAn4.DTOs.FriendShipDto;
using DoAn4.DTOs.PostDTO;
using DoAn4.DTOs.UserDTO;
using DoAn4.Models;

namespace DoAn4.Helper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper() {


            CreateMap<Friendship, FriendshipDto>().ReverseMap();
            CreateMap<User, InfoUserDTO>().ReverseMap();
            CreateMap<Post, PostDto>().ReverseMap();
            CreateMap<Post, InFoPostDto>().ReverseMap();

        }
    }
}
