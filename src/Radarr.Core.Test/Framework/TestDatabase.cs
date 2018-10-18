using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using NLog;
using Radarr.Core.Datastore;
using Radarr.Core.Datastore.Base;
using Radarr.Core.Messaging.Events.Interfaces;
using Radarr.Core.Test.Framework.Interfaces;

namespace Radarr.Core.Test.Framework
{
    public class TestDatabase : ITestDatabase
    {
        private readonly DbContext _dbConnection;

        private readonly ILogger _logger;

        private readonly IEventAggregator _eventAggregator;

        public TestDatabase(DbContext dbConnection, ILogger logger)
        {
            _eventAggregator = new Mock<IEventAggregator>().Object;
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public void InsertMany<T>(IEnumerable<T> items) where T : ModelBase, new()
        {
            new BasicRepository<T>(_dbConnection, _eventAggregator, _logger).InsertMany(items.ToList());
        }

        public T Insert<T>(T item) where T : ModelBase, new()
        {
            return new BasicRepository<T>(_dbConnection, _eventAggregator, _logger).Insert(item);
        }

        public List<T> All<T>() where T : ModelBase, new()
        {
            return new BasicRepository<T>(_dbConnection, _eventAggregator, _logger).All().ToList();
        }

        public T Single<T>() where T : ModelBase, new()
        {
            return All<T>().SingleOrDefault();
        }

        public void Update<T>(T childModel) where T : ModelBase, new()
        {
            new BasicRepository<T>(_dbConnection, _eventAggregator, _logger).Update(childModel);
        }

        public void Delete<T>(T childModel) where T : ModelBase, new()
        {
            new BasicRepository<T>(_dbConnection, _eventAggregator, _logger).Delete(childModel);
        }
    }
}
