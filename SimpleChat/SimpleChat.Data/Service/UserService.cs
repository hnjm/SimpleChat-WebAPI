using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleChat.Core.Helper;
using SimpleChat.Core.Validation;
using SimpleChat.Core.ViewModel;
using SimpleChat.Data.SubStructure;
using SimpleChat.Data.ViewModel;
using SimpleChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SimpleChat.Data.Service
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly SimpleChatDbContext _con;

        #region Ctor

        public UserService(IMapper mapper,
            ILogger<UserService> logger,
            SimpleChatDbContext context,
            ISuggestionService suggestionService,
            ISuggestionCommentService suggestionCommentService,
            ISuggestionReactionService suggestionReactionService)
        {
            _con = context;
            _mapper = mapper;
            _logger = logger;
            _suggestionService = suggestionService;
            _suggestionCommentService = suggestionCommentService;
            _suggestionReactionService = suggestionReactionService;
        }

        #endregion

        #region Methods

        public UserPaggingListVM GetList(string searchText = "", int pageNumber = 1, int pageItemCount = 10)
        {
            if (!searchText.IsNullOrEmpty())
                searchText = searchText.Trim().ToUpper();

            UserPaggingListVM result = new UserPaggingListVM();

            var query = _con.Set<User>().Where(a => searchText.IsNullOrEmpty() || a.UserName.Contains(searchText)).AsNoTracking().OrderBy(a => a.UserName).AsQueryable();

            if (pageNumber > 1)
                query = query.Skip((pageNumber - 1) * pageItemCount);

            result.Records = (from u in query
                            select new UserVM() {
                                Id = u.Id,
                                PictureUrl = u.PictureUrl,
                                Name = u.UserName,
                                SuggetionCount = (from s in _con.Set<Suggestion>().AsNoTracking()
                                                where s.CreateBy == u.Id
                                                select s).Count(),
                                CommentCount = (from s in _con.Set<SuggestionComment>().AsNoTracking()
                                                where s.CreateBy == u.Id
                                                select s).Count(),
                                ReactionCount = (from s in _con.Set<SuggestionReaction>().AsNoTracking()
                                                where s.CreateBy == u.Id
                                                select s).Count()
                            }).Take(pageItemCount).ToList();

            #region Next Page Check
            query = _con.Set<User>().AsNoTracking().AsQueryable();

            if (!searchText.IsNullOrEmpty())
                query = query.Where(a => a.UserName.ToUpper().Contains(searchText)).AsQueryable();

            result.Pagging = new PaggingVM();
            result.Pagging.IsNextPageExist = query.Skip((pageNumber * pageItemCount)).Take(1).Count() == 1;
            #endregion

            return result;
        }

        public async Task<APIResultVM> GetProfileData(Guid userId, Guid currentUserId, string sortOrder = "", int pageNumber = 1, int pageItemCount = 10, Guid? categoryId = null)
        {
            if (userId == null || userId == Guid.Empty)
                APIResult.CreateVM();

            var user = _con.Set<User>().AsNoTracking().Where(a => a.Id == userId).FirstOrDefault();
            if(user == null)
                APIResult.CreateVM();

            ProfileVM model = new ProfileVM();
            _mapper.Map<ProfileVM>(user);

            model.Suggestion = new SuggestionPaggingListVM();
            model.Suggestion.Pagging = new PaggingVM();

            model.Suggestion.Records = _suggestionService.GetAllAsync(a => a.CreateBy == userId, true, sortOrder, pageNumber, pageItemCount, categoryId).Result;
            model.Comments = await _suggestionCommentService.GetCommentsOfUser(userId);
            model.Reactions = await _suggestionReactionService.GetReactionsOfUser(userId);

            model.UserId = userId;
            model.PictureUrl = user.PictureUrl;
            model.CurrentUserId = currentUserId;
            model.EMail = user.Email;
            model.SuggetionCount = _con.Set<Suggestion>().Where(a => !a.IsDeleted && a.CreateBy == userId).Count();

            model.Suggestion.Pagging.ActionName = "Profile";
            model.Suggestion.Pagging.ControllerName = "User";
            var query = _con.Set<Suggestion>().Where(a=> !a.IsDeleted && a.CreateBy == userId).AsNoTracking().AsQueryable();
            model.Suggestion.Pagging.IsNextPageExist = query.Skip((pageNumber * pageItemCount)).Take(1).Count() == 1;

            foreach (var item in model.Suggestion.Records)
            {
                item.CreateByName = user.Email;
            }

            return APIResult.CreateVMWithRec<ProfileVM>(model, true, userId);
        }

        #endregion
    }

    public interface IUserService
    {
        Task<APIResultVM> GetProfileData(Guid userId, Guid currentUserId, string sortOrder = "", int pageNumber = 1, int pageItemCount = 10, Guid? categoryId = null);
        UserPaggingListVM GetList(string searchText = "", int pageNumber = 1, int pageItemCount = 10);
    }
}
