using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace ScoreBoard.Infrastructure
{
    public class NHibernateSessionManager
    {
        private static string _dbConnectionString;
        private static ISessionFactory _sessionFactory;
        private static bool _createSchema;
        private static bool _updateSchema;

        private static ISessionFactory SessionFactory => _sessionFactory ??= InitializeSessionFactory(_dbConnectionString, _createSchema, _updateSchema);

        private static ISessionFactory InitializeSessionFactory(string connectionString, bool create = false, bool update = false)
        {
            return Fluently.Configure()
                           .Database(MsSqlConfiguration.MsSql2012.ConnectionString(connectionString))
                           .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHibernateSessionManager>())
                           .CurrentSessionContext("call")
                           .ExposeConfiguration(cfg => BuildSchema(cfg, create, update))
                           .BuildSessionFactory();
        }

        public static ISession OpenSession(string connectionString, bool create = false, bool update = false)
        {
            _dbConnectionString = connectionString;
            _createSchema = create;
            _updateSchema = update;
            return SessionFactory.OpenSession();
        }

        private static void BuildSchema(Configuration config, bool create = false, bool update = false)
        {
            if (create)
            {
                new SchemaExport(config).Create(false, true);
            }
            else
            {
                new SchemaUpdate(config).Execute(false, update);
            }
        }
    }
}
