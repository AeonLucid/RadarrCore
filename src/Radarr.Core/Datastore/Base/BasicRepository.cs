using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NLog;
using Radarr.Common.Extensions;
using Radarr.Core.Datastore.Exceptions;
using Radarr.Core.Datastore.Extensions;
using Radarr.Core.Messaging.Events.Interfaces;

namespace Radarr.Core.Datastore.Base
{
    public class BasicRepository<TModel> : IBasicRepository<TModel> where TModel : ModelBase, new()
    {
        private readonly DbContext _database;

        private readonly IEventAggregator _eventAggregator;

        private readonly ILogger _logger;

        public BasicRepository(DbContext database, IEventAggregator eventAggregator, ILogger logger)
        {
            _database = database;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        protected void Delete(Expression<Func<TModel, bool>> filter)
        {
            // TODO: Improve (?)
            // https://stackoverflow.com/questions/2519866/how-do-i-delete-multiple-rows-in-entity-framework-without-foreach

            _database.RemoveRange(_database.Set<TModel>().Where(filter));
            _database.SaveChanges();
        }

        public IEnumerable<TModel> All()
        {
            return _database.Set<TModel>().AsNoTracking();
        }

        public int Count()
        {
            return _database.Set<TModel>().Count();
        }

        public TModel Get(int id)
        {
            var model = _database.Set<TModel>().SingleOrDefault(x => x.Id == id);
            if (model == null)
            {
                throw new ModelNotFoundException(typeof(TModel), id);
            }

            return model;
        }

        public IEnumerable<TModel> Get(IEnumerable<int> ids)
        {
            var idList = ids.ToList();
            var result = _database.Set<TModel>().Where(x => idList.Contains(x.Id)).ToList();
            if (result.Count != idList.Count())
            {
                throw new ApplicationException("Expected query to return {0} rows but returned {1}.".Inject(idList.Count(), result.Count));
            }

            return result;
        }

        public TModel SingleOrDefault()
        {
            return All().SingleOrDefault();
        }

        public TModel Single()
        {
            return All().Single();
        }

        public TModel Insert(TModel model)
        {
            if (model.Id != 0)
            {
                throw new InvalidOperationException("Can't insert model with existing ID " + model.Id);
            }

            _database.Set<TModel>().Add(model);
            _database.SaveChanges();

            ModelCreated(model);

            return model;
        }

        public TModel Update(TModel model)
        {
            if (model.Id == 0)
            {
                throw new InvalidOperationException("Can't update model with ID 0");
            }

            _database.Set<TModel>().Update(model);
            _database.SaveChangesAsync();

            ModelUpdated(model);

            return model;
        }

        public void Delete(TModel model)
        {
            Delete(model.Id);
        }

        public void InsertMany(IList<TModel> models)
        {
            using (var transaction = _database.Database.BeginTransaction())
            {
                _database.AddRange(models);

                transaction.Commit();
            }
        }

        public void UpdateMany(IList<TModel> models)
        {
            using (var transaction = _database.Database.BeginTransaction())
            {
                foreach (var model in models)
                {
                    if (model.Id == 0)
                    {
                        throw new InvalidOperationException("Can't update model with ID 0");
                    }

                    _database.Update(model);
                }

                _database.SaveChanges();

                transaction.Commit();
            }
        }

        public void DeleteMany(List<TModel> models)
        {
            DeleteMany(models.Select(m => m.Id));
        }

        public TModel Upsert(TModel model)
        {
            if (model.Id == 0)
            {
                Insert(model);
                return model;
            }

            Update(model);
            return model;
        }

        public void Delete(int id)
        {
            _database.Remove(_database.Set<TModel>().Where(x => x.Id == id));
            _database.SaveChanges();
        }

        public void DeleteMany(IEnumerable<int> ids)
        {
            using (var transaction = _database.Database.BeginTransaction())
            {
                var idList = ids.ToList();

                _database.RemoveRange(_database.Set<TModel>().Where(x => idList.Contains(x.Id)));
                _database.SaveChanges();

                transaction.Commit();
            }
        }

        public void Purge(bool vacuum = false)
        {
            // TODO: Improve (?)
            // https://stackoverflow.com/questions/15220411/entity-framework-delete-all-rows-in-table

            _database.RemoveRange(_database.Set<TModel>());
            _database.SaveChanges();
        }

        protected void Vacuum()
        {
            _logger.Info("Vacuuming main database");
            _database.Database.ExecuteSqlCommand("Vacuum;");
            _logger.Info("main database compressed");
        }

        public bool HasItems()
        {
            return Count() > 0;
        }

        public void SetFields(TModel model, params Expression<Func<TModel, object>>[] properties)
        {
            throw new NotImplementedException();
        }

        public virtual PagingSpec<TModel> GetPaged(PagingSpec<TModel> pagingSpec)
        {
            pagingSpec.Records = GetPagedQuery(pagingSpec)
                .Skip(pagingSpec.PagingOffset())
                .Take(pagingSpec.PageSize)
                .ToList();

            pagingSpec.TotalRecords = GetPagedQuery(pagingSpec).Count();

            return pagingSpec;
        }

        protected virtual IOrderedQueryable<TModel> GetPagedQuery(PagingSpec<TModel> pagingSpec)
        {
            var queryWhere = _database.Set<TModel>().Where(pagingSpec.FilterExpression);

            return pagingSpec.SortDirection == SortDirection.Ascending ? 
                queryWhere.OrderBy(pagingSpec.OrderByClause()) : 
                queryWhere.OrderByDescending(pagingSpec.OrderByClause());
        }

        protected void ModelCreated(TModel model)
        {
            PublishModelEvent(model, ModelAction.Created);
        }

        protected void ModelUpdated(TModel model)
        {
            PublishModelEvent(model, ModelAction.Updated);
        }

        protected void ModelDeleted(TModel model)
        {
            PublishModelEvent(model, ModelAction.Deleted);
        }

        private void PublishModelEvent(TModel model, ModelAction action)
        {
            if (PublishModelEvents)
            {
                _eventAggregator.PublishEvent(new ModelEvent<TModel>(model, action));
            }
        }

        protected virtual bool PublishModelEvents => false;
    }
}
