using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartEF
{
    public interface IVirtualDeletion
    {
        void Delete();
    }

    public abstract class BaseEntity<T>
    {
        public virtual T Id { get; protected set; }

        protected BaseEntity()
        {
        }

        protected BaseEntity(T id)
        {
            Id = id;
        }
    }

    public abstract class GenericRepository<TEntity> where TEntity : BaseEntity<int>
    {
        public readonly DbContext _context;

        public DbSet<TEntity> _entity => _context.Set<TEntity>();

        protected GenericRepository(DbContext context)
        {
            _context = context;
        }

        public TEntity Add(TEntity entity)
        {
            return (_entity.Add(entity)).Entity;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            return (await _entity.AddAsync(entity)).Entity;
        }

        public TEntity GetById(int id)
        {
            return _entity.FirstOrDefault(p => p.Id == id);
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _entity.FirstOrDefaultAsync(p => p.Id == id);
        }

        public List<TEntity> GetAll()
        {
            return _entity.ToList();
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _entity.ToListAsync();
        }

        public void Update(TEntity entity)
        {
            _entity.Update(entity);
        }

        public virtual IQueryable<TEntity> List(
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool readOnly = true,
            int skip = 0,
            int take = int.MaxValue
        )
        {
            var rows = readOnly ?
                _context.Set<TEntity>().AsNoTracking() :
                _context.Set<TEntity>();

            rows = filter != null ?
                rows.Where(filter).AsQueryable() :
                rows.AsQueryable();

            if (include != null)
            {
                rows = include(rows);
            }

            if (orderBy != null)
            {
                rows = orderBy(rows).AsQueryable();
            }

            rows = skip == 0 ? rows.Take(take) : rows.Skip(skip).Take(take);

            return rows.AsQueryable();
        }

        public void Delete(TEntity entity)
        {
            _entity.Remove(entity);
        }

        public void Delete(int id)
        {
            var entity = _entity.Find(id);

            _entity.Remove(entity);
        }

        public async Task DeleteAsync(int id)
        {
            if ((await _entity.FindAsync(id)) is var entity && entity is not null)
            {
                _entity.Remove(entity);
            }
        }

        public void DeleteVirtual<TRecord>(int id) where TRecord : BaseEntity<int>, IVirtualDeletion
        {
            if ((_context.Set<TRecord>().FirstOrDefault(p => p.Id == id)) is var record && record is null)
            {
                return;
            }

            record.Delete();

            _context.Set<TRecord>().Update(record);
        }

        public async Task DeleteVirtualAsync<TRecord>(int id) where TRecord : BaseEntity<int>, IVirtualDeletion
        {
            if ((await _context.Set<TRecord>().FirstOrDefaultAsync(p => p.Id == id)) is var record && record is null)
            {
                return;
            }

            record.Delete();

            _context.Set<TRecord>().Update(record);
        }
    }
}
