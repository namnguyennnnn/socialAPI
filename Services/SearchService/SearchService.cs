using DoAn4.DTOs.UserDTO;
using DoAn4.Helper;
using DoAn4.Interfaces;
using DoAn4.Services.AuthenticationService;
using Microsoft.Extensions.Options;



namespace DoAn4.Services.SearchService
{
    public class SearchService : ISearchService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticationService _authenticationService; 
        public SearchService(IAuthenticationService authenticationService,IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _authenticationService = authenticationService;
        }

        public async Task<List<InfoUserDTO>> Search( string token , string keyword)
        {
            try
            {
                var curUser = await _authenticationService.GetIdUserFromAccessToken(token);
                var listUsers = await _userRepository.GetUsersByKeyWord(curUser.UserId,keyword);
                return listUsers;
                
            }catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        } 
    }

}

