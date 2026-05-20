using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using ScoreBoard.Domain.Models;
using ScoreBoard.Infrastructure.Models;
using ScoreBoard.Services.Adapters;

namespace ScoreBoard.Infrastructure.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private string dbConnectionString;
        
        public void Initialize(string connectionString)
        {
            dbConnectionString = connectionString;
        }

        public async Task<Team> GetById(Guid id)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            TeamEntity teamEntity = await session.GetAsync<TeamEntity>(id).ConfigureAwait(false);
            return teamEntity?.GetTeam();
        }
        
        public async Task<ICollection<Team>> GetAll()
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            ICollection<TeamEntity> teamEntities = await session.Query<TeamEntity>().ToListAsync().ConfigureAwait(false);
            return ConvertTeamEntities(teamEntities);
        }

        public async Task<Team> GetByExistingTeam(Team team)
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            string p1 = "";
            if (team.Player1 != null)
            {
                p1 = team.Player1.Id.ToString().TrimStart('{').TrimEnd('}');
            }
            string p2 = "";
            if (team.Player2 != null)
            {
                p2 = team.Player2.Id.ToString().TrimStart('{').TrimEnd('}');
            }

            string sql;
            if (!string.IsNullOrEmpty(p1) && !string.IsNullOrEmpty(p2))
            {
                sql = "from TeamEntity t where (t.Player1.Id = '" + p1 + "' OR t.Player2.Id = '" + p1 + "') AND (t.Player1.Id = '" + p2 + "' OR t.Player2.Id = '" + p2 + "') AND (t.Name = '" + team.Name.Trim() + "')";
            }
            else if (!string.IsNullOrEmpty(p1) && string.IsNullOrEmpty(p2))
            {
                sql = "from TeamEntity t where (t.Player1.Id = '" + p1 + "' OR t.Player2.Id = '" + p1 + "') AND (t.Player1.Id IS NULL OR t.Player2.Id IS NULL) AND (t.Name = '" + team.Name.Trim() + "')";
            }
            else if (string.IsNullOrEmpty(p1) && !string.IsNullOrEmpty(p2))
            {
                sql = "from TeamEntity t where (t.Player1.Id = '" + p2 + "' OR t.Player2.Id = '" + p2 + "') AND (t.Player1.Id IS NULL OR t.Player2.Id IS NULL) AND (t.Name = '" + team.Name.Trim() + "')";
            }
            else
            {
                return null;
            }
            ICollection<TeamEntity> teamEntities = await session.CreateQuery(sql).ListAsync<TeamEntity>().ConfigureAwait(false);
            if (teamEntities == null || teamEntities.Count == 0)
            {
                return null;
            }
            return teamEntities.ElementAt(0).GetTeam(true);
        }

        public async Task<ICollection<string>> GetNames()
        {
            using ISession session = NHibernateSessionManager.OpenSession(dbConnectionString);
            ICriteria criteria = session.CreateCriteria(typeof(TeamEntity));
            criteria.SetProjection(Projections.Distinct(Projections.ProjectionList().Add(Projections.Property("Name").As("Name"))));
            criteria.SetResultTransformer(new AliasToBeanResultTransformer(typeof(TeamEntity)));
            IList<TeamEntity> teams = await criteria.ListAsync<TeamEntity>().ConfigureAwait(false);
            ICollection<string> names = new List<string>();
            if (teams == null)
            {
                return names;
            }
            foreach (TeamEntity teamEntity in teams)
            {
                names.Add(teamEntity.Name);
            }
            return names;
        }

        private static ICollection<Team> ConvertTeamEntities(ICollection<TeamEntity> teamEntities)
        {
            if (teamEntities == null)
            {
                return null;
            }
            ICollection<Team> teams = new List<Team>();
            foreach (TeamEntity teamEntity in teamEntities)
            {
                teams.Add(teamEntity.GetTeam());
            }
            return teams;
        }
    }
}
