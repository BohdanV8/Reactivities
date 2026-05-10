using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Intarfaces
{
    public interface IUserAccessor
    {
        string getUserId();
        Task<User> GetUserAsync();
        Task<User> GetUserWithPhotosAsync();
    }
}
