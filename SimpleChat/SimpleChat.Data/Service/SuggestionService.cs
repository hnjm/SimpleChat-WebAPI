using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleChat.Data.SubStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SuggestionBoard.Data.Service
{
    public class SuggestionService : BaseService<SuggestionSaveVM, SuggestionVM, Suggestion>,
    ISuggestionService
    {
        private SuggestionBoardDbContext _con;

        #region Ctor

        public SuggestionService(UnitOfWork uow, IMapper mapper,
            ILogger<SuggestionService> logger,
            ILogger<IRepository<Suggestion>> repositoryLogger,
            SuggestionBoardDbContext context)
            : base(uow, mapper, logger, repositoryLogger)
        {
            _con = context;
        }

        #endregion

        #region Methods

        public SuggestionPaggingListVM GetList(bool showIsDeleted = false, string searchText = "", string sortOrder = "", int pageNumber = 1, int pageItemCount = 10, Guid? categoryId = null)
        {
            if (!searchText.IsNullOrEmpty())
                searchText = searchText.Trim().ToUpper();

            SuggestionPaggingListVM result = new SuggestionPaggingListVM();

            result.Records = GetAllAsync(a => a.Title.ToUpper().Contains(searchText)
                    || a.Description.ToUpper().Contains(searchText),
                    true, sortOrder, pageNumber, pageItemCount, categoryId).Result;

            #region Next Page Check
            var query = Repository.Query(showIsDeleted).AsNoTracking().AsQueryable();

            if (!searchText.IsNullOrEmpty())
            {
                query = query.Where(a => a.Title.ToUpper().Contains(searchText)
                    || a.Description.ToUpper().Contains(searchText)).AsQueryable();
            }

            result.Pagging = new PaggingVM();
            result.Pagging.IsNextPageExist = query.Skip((pageNumber * pageItemCount)).Take(1).Count() == 1;
            #endregion

            var users = _con.Set<User>().AsNoTracking().Select(s => new { s.Id, s.UserName }).ToList();

            foreach (var item in result.Records)
            {
                item.CreateByName = users.Any(a => a.Id == item.CreateById) ? users.Where(a => a.Id == item.CreateById).Select(s => s.UserName).FirstOrDefault() : "";
            }

            return result;
        }

        public async Task<SuggestionDetailVM> GetWithAdditionalData(Guid? id)
        {
            SuggestionDetailVM vm = new SuggestionDetailVM();
            vm.Rec = new SuggestionSaveVM();

            vm.Categories = _con.Set<Category>().Where(a => !a.IsDeleted)
                .Select(s => new SelectListVM()
                {
                    Id = s.Id,
                    Text = s.Name
                }).OrderBy(o => o.Text).AsNoTracking().ToList();

            if (id.IsNullOrEmpty())
                return vm;

            var record = await Repository.GetByIDAysnc(id.Value);

            if (record != null)
            {
                var users = _con.Set<User>().AsNoTracking().Select(s => new { s.Id, s.UserName }).ToList();

                vm.Id = id.Value;
                vm.Rec = _mapper.Map<SuggestionSaveVM>(record);
                vm.Rec.CreateByName = users.Any(a => a.Id == record.CreateBy) ? users.Where(a => a.Id == record.CreateBy).Select(s => s.UserName).FirstOrDefault() : "";

                //Load Comments
                vm.SuggestionComments = await _mapper.ProjectTo<SuggestionCommentVM>(_con.Set<SuggestionComment>().AsNoTracking().Where(a => a.SuggestionId == id.Value)
                    .OrderByDescending(o => o.CreateDT)).ToListAsync();

                foreach (var item in vm.SuggestionComments)
                {
                    item.CreateByName = users.Any(a => a.Id == item.CreateBy) ? users.Where(a => a.Id == item.CreateBy).Select(s => s.UserName).FirstOrDefault() : "";
                }

                //Get Reactions
                vm.SuggestionReactions = await _mapper.ProjectTo<SuggestionReactionVM>(_con.Set<SuggestionReaction>().AsNoTracking().Where(a => a.SuggestionId == id.Value)
                    .OrderByDescending(o => o.CreateDT)).ToListAsync();

                foreach (var item in vm.SuggestionReactions)
                {
                    item.CreateByName = users.Any(a => a.Id == item.CreateBy) ? users.Where(a => a.Id == item.CreateBy).Select(s => s.UserName).FirstOrDefault() : "";
                }
            }

            return vm;
        }

        public async Task UpdateReactionCount(Guid? id, UserReaction reaction)
        {
            var record = await Repository.GetByIDAysnc(id.Value);

            if (record != null)
            {
                switch (reaction)
                {
                    case UserReaction.Like:
                        record.LikeAmount = record.LikeAmount + 1;
                        break;
                    case UserReaction.Dislike:
                        record.DislikeAmount = record.DislikeAmount + 1;
                        break;
                }

                Repository.Update(record);
                await CommitAsync();
            }
        }

        public async Task<List<SuggestionVM>> GetAllAsync(Expression<Func<Suggestion, bool>> expr = null, bool asNoTracking = true, string sortOrder = "", int pageNumber = 1, int pageItemCount = 10, Guid? categoryId = null)
        {
            try
            {
                var query = Repository.Query().Include(i => i.Category).AsQueryable();

                if (expr != null)
                    query = query.Where(expr);

                if (asNoTracking)
                    query = query.AsNoTracking();

                if (!categoryId.IsNullOrEmpty())
                    query = query.Where(a => a.CategoryId == categoryId.Value);

                switch (sortOrder)
                {
                    case "newest":
                        query = query.OrderByDescending(o => o.CreateDT);
                        break;
                    case "comment":
                        query = query.Include(i => i.SuggestionComments).OrderByDescending(o => o.SuggestionComments != null ? o.SuggestionComments.Count : 0);
                        break;
                    case "like":
                        query = query.OrderByDescending(o => o.LikeAmount);
                        break;
                    case "reaction":
                        query = query.Include(i => i.SuggestionComments).OrderByDescending(o => o.LikeAmount + o.DislikeAmount + (o.SuggestionComments != null ? o.SuggestionComments.Count : 0));
                        break;
                    default:
                        query = query.OrderBy(o => o.CreateDT);
                        break;
                }

                if (pageNumber > 1)
                    query = query.Skip((pageNumber - 1) * pageItemCount);

                return await _mapper.ProjectTo<SuggestionVM>(query.Take(pageItemCount)).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("BaseService.GetAllAsync", e);
                return null;
            }
        }

        #endregion
    }

    public interface ISuggestionService : IBaseService<SuggestionSaveVM, SuggestionVM, Suggestion>
    {
        SuggestionPaggingListVM GetList(bool showIsDeleted = false, string searchText = "", string sortOrder = "", int pageNumber = 1, int pageItemCount = 10, Guid? categoryId = null);
        Task<SuggestionDetailVM> GetWithAdditionalData(Guid? id);
        Task UpdateReactionCount(Guid? id, UserReaction reaction);
        Task<List<SuggestionVM>> GetAllAsync(Expression<Func<Suggestion, bool>> expr = null, bool asNoTracking = true, string sortOrder = "", int pageNumber = 1, int pageItemCount = 10, Guid? categoryId = null);
    }
}
