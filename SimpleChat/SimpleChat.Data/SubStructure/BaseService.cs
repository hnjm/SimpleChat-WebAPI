using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SimpleChat.Core.ViewModel;
using SimpleChat.Core.Validation;
using SimpleChat.Core.Helper;
using SimpleChat.Core.EntityFramework;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SimpleChat.Data.SubStructure
{
    public interface IBaseService<A, U, L, D>
        where A : AddVM, IAddVM, new()
        where U : UpdateVM, IUpdateVM, new()
        where L : BaseVM, IBaseVM, new()
        where D : BaseEntity, IBaseEntity, new()
    {
        IQueryable<D> Query(bool showIsDeleted = false);
        Task<bool> AnyAsync(Guid id);
        Task<bool> AnyAsync(Expression<Func<D, bool>> expr);
        Task<L> GetByIdAsync(Guid id);
        Task<IEnumerable<L>> GetAllAsync(bool asNoTracking = true);
        Task<List<L>> GetAllAsync(Expression<Func<D, bool>> expr, bool asNoTracking = true);
        IQueryable<T> GetAllAsync<T>(Expression<Func<D, bool>> expr, Expression<Func<D, T>> selector, bool asNoTracking = true);
        Task<APIResultVM> AddAsync(A model, Guid? userId = null, bool isCommit = true);
        Task<APIResultVM> UpdateAsync(Guid id, U model, Guid? userId = null, bool isCommit = true);
        Task<APIResultVM> DeleteAsync(Guid id, Guid? userId = null, bool shouldBeOwner = false, bool isCommit = true);
        Task<APIResultVM> ReverseDeleteAsync(Guid id, Guid? userId, bool isCommit = true);
        Task<APIResultVM> CommitAsync();
    }

    public class BaseService<A, U, L, D> : IBaseService<A, U, L, D>
        where A : AddVM, IAddVM, new()
        where U : UpdateVM, IUpdateVM, new()
        where L : BaseVM, IBaseVM, new()
        where D : BaseEntity, IBaseEntity, new()
    {
        protected UnitOfWork _uow;
        protected readonly IMapper _mapper;
        //TODO: LOGGING
        //protected readonly ILogger<BaseService<S, L, D>> _logger;
        private readonly ILogger<IRepository<D>> _repositoryLogger;

        public BaseService(UnitOfWork uow, IMapper mapper, //ILogger<BaseService<S, L, D>> logger,
            ILogger<IRepository<D>> repositoryLogger)
        {
            _uow = uow;
            _mapper = mapper;
            //_logger = logger;
            _repositoryLogger = repositoryLogger;
        }

        protected IRepository<D> Repository
        {
            get
            {
                return _uow.Repository<D>(_repositoryLogger);
            }
        }

        public IQueryable<D> Query(bool showIsDeleted = false)
        {
            try
            {
                return Repository.Query(showIsDeleted);
            }
            catch (Exception ex)
            {
                //_logger.LogError("Repository.Query", ex);
                return null;
            }
        }
        public virtual async Task<bool> AnyAsync(Guid id)
        {
            try
            {
                if (id.IsNull())
                    return false;

                return await _uow.Repository<D>(_repositoryLogger).AnyAysnc(a => a.Id == id);
            }
            catch (Exception e)
            {
                //_logger.LogError("BaseService.AnyAsync", e);
                return false;
            }
        }
        public virtual async Task<bool> AnyAsync(Expression<Func<D, bool>> expr)
        {
            try
            {
                if (expr == null)
                    return false;

                return await _uow.Repository<D>(_repositoryLogger).AnyAysnc(expr);
            }
            catch (Exception e)
            {
                //_logger.LogError("BaseService.AnyAsync", e);
                return false;
            }
        }

        public virtual async Task<L> GetByIdAsync(Guid id)
        {
            try
            {
                if (id.IsNull())
                    return null;

                return _mapper.Map<L>(await _uow.Repository<D>(_repositoryLogger).GetByIDAysnc(id));
            }
            catch (Exception e)
            {
                //_logger.LogError("BaseService.GetByIdAsync", e);
                return null;
            }
        }
        public virtual async Task<IEnumerable<L>> GetAllAsync(bool asNoTracking = true)
        {
            try
            {
                var query = Repository.Query();

                if (asNoTracking)
                    query = query.AsNoTracking();

                return await _mapper.ProjectTo<L>(query).ToListAsync();
            }
            catch (Exception e)
            {
                //_logger.LogError("BaseService.GetAllAsync", e);
                return null;
            }
        }
        public virtual async Task<List<L>> GetAllAsync(Expression<Func<D, bool>> expr, bool asNoTracking = true)
        {
            try
            {
                var query = Repository.Query().Where(expr);

                if (asNoTracking)
                    query = query.AsNoTracking();

                return await _mapper.ProjectTo<L>(query).ToListAsync();
            }
            catch (Exception e)
            {
                //_logger.LogError("BaseService.GetAllAsync", e);
                return null;
            }
        }
        public virtual IQueryable<T> GetAllAsync<T>(Expression<Func<D, bool>> expr, Expression<Func<D, T>> selector, bool asNoTracking = true)
        {
            try
            {
                var query = Repository.Query().Where(expr);

                if (asNoTracking)
                    query = query.AsNoTracking();

                var results = query.Select(selector);

                return results;
            }
            catch (Exception e)
            {
                //_logger.LogError("BaseService.GetAllAsync", e);
                return null;
            }
        }


        public virtual async Task<APIResultVM> AddAsync(A model, Guid? userId = null, bool isCommit = true)
        {
            try
            {
                Guid _userId = userId == null ? Guid.Empty : userId.Value;

                D entity = _mapper.Map<A, D>(model);
                if (entity.Id == null || entity.Id == Guid.Empty)
                    entity.Id = Guid.NewGuid();

                if (entity is ITableEntity)
                {
                    (entity as ITableEntity).CreateBy = _userId;
                    (entity as ITableEntity).CreateDT = DateTime.Now;
                }

                Repository.Add(entity);

                if (isCommit)
                    await CommitAsync();

                return APIResult.CreateVMWithRec(entity, true, entity.Id);
            }
            catch (Exception e)
            {
                //_logger.LogError("BaseService.AddAsync", e);
                return APIResult.CreateVM();
            }
        }
        public virtual async Task<APIResultVM> UpdateAsync(Guid id, U model, Guid? userId = null, bool isCommit = true)
        {
            try
            {
                Guid _userId = userId == null ? Guid.Empty : userId.Value;

                D entity = await _uow.Repository<D>(_repositoryLogger).GetByIDAysnc(id);
                if (entity.IsNull())
                    return APIResult.CreateVM(false, id, ErrorMessages.RecordNotFound);

                entity = _mapper.Map<U, D>(model, entity);

                if (entity is ITableEntity)
                {
                    (entity as ITableEntity).UpdateBy = _userId;
                    (entity as ITableEntity).UpdateDT = DateTime.Now;
                }

                Repository.Update(entity);

                if (isCommit)
                    await CommitAsync();

                return APIResult.CreateVMWithRec(entity, true, entity.Id);
            }
            catch (Exception e)
            {
                //_logger.LogError("BaseService.UpdateAsync", e);
                return APIResult.CreateVM();
            }
        }
        public virtual async Task<APIResultVM> DeleteAsync(Guid id, Guid? userId = null, bool shouldBeOwner = false, bool isCommit = true)
        {
            try
            {
                Guid _userId = userId == null ? Guid.Empty : userId.Value;

                D entity = await _uow.Repository<D>(_repositoryLogger).GetByIDAysnc(id);
                if (entity.IsNull())
                    return APIResult.CreateVM(false, id, ErrorMessages.RecordNotFound);

                if (shouldBeOwner && (_userId == Guid.Empty || _userId != (entity as ITableEntity).CreateBy))
                    return APIResult.CreateVM(false, id, ErrorMessages.NotOwnerOfRecord);

                if (entity is ITableEntity)
                {
                    (entity as ITableEntity).UpdateBy = _userId;
                    (entity as ITableEntity).UpdateDT = DateTime.Now;
                }

                entity.IsDeleted = true;
                Repository.Update(entity);

                if (isCommit)
                    await CommitAsync();

                return APIResult.CreateVMWithRec(entity, true, entity.Id);
            }
            catch (Exception e)
            {
                //_logger.LogError("BaseService.DeleteAsync", e);
                return APIResult.CreateVM();
            }
        }
        public virtual async Task<APIResultVM> ReverseDeleteAsync(Guid id, Guid? userId, bool isCommit = true)
        {
            try
            {
                Guid _userId = userId == null ? Guid.Empty : userId.Value;

                D entity = await _uow.Repository<D>(_repositoryLogger).GetByIDAysnc(id);
                if (entity.IsNull())
                    return APIResult.CreateVM(false, id, ErrorMessages.RecordNotFound);

                if (entity is ITableEntity)
                {
                    (entity as ITableEntity).UpdateBy = _userId;
                    (entity as ITableEntity).UpdateDT = DateTime.Now;
                }

                entity.IsDeleted = false;
                Repository.Update(entity);

                if (isCommit)
                    await CommitAsync();

                return APIResult.CreateVMWithRec(entity, true, entity.Id);
            }
            catch (Exception e)
            {
                //_logger.LogError("BaseService.ReverseDeleteAsync", e);
                return APIResult.CreateVM();
            }
        }

        public virtual async Task<APIResultVM> CommitAsync()
        {
            try
            {
                await _uow.SaveChanges();

                return APIResult.CreateVM(true);
            }
            catch (Exception e)
            {
                //_logger.LogError("BaseService.CommitAsync", e);
                return APIResult.CreateVM();
            }
        }
    }
}
