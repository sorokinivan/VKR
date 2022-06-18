using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using VKR.Models;
using VKR.Models.AuthorizationModels;
using VKR.Core;
using System.Text;

namespace VKR.Repositories.Authorization
{
    public class AuthorizationRepository : IAuthorizationRepository
    {
        private readonly DBContext _dBContext;
        public AuthorizationRepository(DBContext dBContext)
        {
            _dBContext = dBContext;
        }

        //public async Task<AspNetUser> GetUserByUserName(string userName)
        //{
        //    return await _dBContext.AspNetUsers.Where(x => x.UserName == userName).FirstOrDefaultAsync();
        //}

        public string GetHashPassword(string password)
        {
            //byte[] salt = new byte[128 / 8];
            //using (var rngCsp = new RNGCryptoServiceProvider())
            //{
            //    rngCsp.GetNonZeroBytes(salt);
            //}
            //return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            //password: password,
            //salt: salt,
            //prf: KeyDerivationPrf.HMACSHA256,
            //iterationCount: 100000,
            //numBytesRequested: 256 / 8));
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));

            return Convert.ToBase64String(hash);
        }

        //public async Task RegisterNewUser(User user)
        //{
        //    var checkUser = await GetUserByUserName(user.UserName);
        //    if(checkUser == null)
        //    {
        //        var userModel = new AspNetUser
        //        {
        //            PhoneNumberConfirmed = false,
        //            AccessFailedCount = 0,
        //            LockoutEnabled = false,
        //            TwoFactorEnabled = false,
        //            EmailConfirmed = false,
        //            Email = user.Email,
        //            UserName = user.UserName,
        //            PasswordHash = GetHashPassword(user.Password)
        //        };
        //        _dBContext.AspNetUsers.Add(userModel);
        //        await _dBContext.SaveChangesAsync();
        //        _dBContext.AspNetUserRoles.Add(new AspNetUserRole
        //        {
        //            RoleId = Constants.COMMON_ROLE_ID,
        //            UserId = userModel.Id
        //        });
        //        await _dBContext.SaveChangesAsync();

        //    }
        //    else
        //    {
        //        throw new Exception();
        //    }
        //}
    }
}
