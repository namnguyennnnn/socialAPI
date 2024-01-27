using DoAn4.DTOs.UserDTO;


namespace DoAn4.Services.SearchService
{
    public interface ISearchService
    {
        Task<List<InfoUserDTO>> Search(string token ,string keyword);
    }
}
