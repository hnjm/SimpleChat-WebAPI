using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleChat.ViewModel.User;
using SimpleChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Data.Service
{
    public class UserService : IUserService
    {
        private SimpleChatDbContext con;
        private readonly IMapper mapper;

        #region Ctor

        public UserService(SimpleChatDbContext _con, IMapper _mapper)
        {
            con = _con;
            mapper = _mapper;
        }

        #endregion

        #region Methods

        public List<UserVM> GetAll()
        {
            var userList = con.Set<User>().AsEnumerable()
                .Select(x => mapper.Map<UserVM>(x))
                .ToList();

            return userList;
        }

        public List<UserListVM> GetUserList()
        {
            var result = con.Set<User>().Select(a => new UserListVM()
            {
                Id = a.Id,
                DisplayName = a.DisplayName,
                UserName = a.UserName
            }).ToList();

            return result;
        }

        public Task<bool> AnyAsync(string userName)
        {
            var result = con.Set<User>().AnyAsync(a => a.UserName == userName);

            return result;
        }

        public User GetById(Guid id)
        {
            var rec = con.Set<User>().Where(a => a.Id == id).FirstOrDefault();

            if (rec == null)
            {
                return null;
            }

            rec.AccessFailedCount = 0;
            rec.ConcurrencyStamp = "";
            //rec.PasswordHash = "";
            rec.SecurityStamp = "";

            return rec;
        }

        #endregion
    }

    public interface IUserService
    {
        List<UserVM> GetAll();
        List<UserListVM> GetUserList();
        Task<bool> AnyAsync(string userName);
        User GetById(Guid id);
    }
}
