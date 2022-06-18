using VKR.Models.AuthorizationModels;

namespace VKR.Repositories.Authorization
{
    public interface IAuthorizationRepository
    {
        //Task RegisterNewUser(User user);
        //Task<AspNetUser> GetUserByUserName(string userName);
        string GetHashPassword(string password);
    }
}
