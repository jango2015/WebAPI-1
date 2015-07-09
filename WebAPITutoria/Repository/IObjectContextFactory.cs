using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPITutoria.Repository
{
    public interface IObjectContextFactory
    {
        InMemoryDatabaseObjectContext Create();
    }

    public class LazySingletonObjectContextFactory : IObjectContextFactory
    {
        public InMemoryDatabaseObjectContext Create()
        {
            return InMemoryDatabaseObjectContext.Instance;
        }
    }

}