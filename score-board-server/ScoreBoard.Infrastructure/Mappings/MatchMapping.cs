using FluentNHibernate.Mapping;
using ScoreBoard.Infrastructure.Models;

namespace ScoreBoard.Infrastructure.Mappings
{
    // ReSharper disable once UnusedMember.Global
    public class MatchMapping : ClassMap<MatchEntity>
    {
        public MatchMapping()
        {
            Schema(@"dbo");
            Table(@"Match");
            Id(x => x.Id)
                .Column("Id")
                .CustomType("Guid")
                .Access.Property()
                .CustomSqlType("uniqueidentifier")
                .Not.Nullable()
                .GeneratedBy.Assigned();
            Map(x => x.Score1)
                .Column("Score1")
                .CustomType("Int32")
                .Access.Property()
                .Generated.Never().CustomSqlType("int")
                .Precision(10);
            Map(x => x.Score2)
                .Column("Score2")
                .CustomType("Int32")
                .Access.Property()
                .Generated.Never().CustomSqlType("int")
                .Precision(10);
            Map(x => x.StartTime)
                .Column("StartTime")
                .CustomType("DateTime")
                .Access.Property()
                .Generated.Never().CustomSqlType("datetime");
            Map(x => x.StopTime)
                .Column("StopTime")
                .CustomType("DateTime")
                .Access.Property()
                .Generated.Never().CustomSqlType("datetime");
            Map(x => x.State)
                .Column("State")
                .CustomType("Int32")
                .Access.Property()
                .Generated.Never().CustomSqlType("int")
                .Not.Nullable()
                .Precision(10);
            Map(x => x.MatchQuality)
                .Column("MatchQuality")
                .CustomType("Double")
                .Access.Property();
            References(x => x.Game)
                .Class<GameEntity>()
                .Access.Property()
                .Cascade.None()
                .Not.LazyLoad()
                .Columns("GameId");
            //HasOne(x => x.Team1).Cascade.All();
            References(x => x.Team1)
                .Class<TeamEntity>()
                .Access.Property()
                .Cascade.All()
                .Not.LazyLoad()
                .Nullable()
                .Columns("Team1Id");
            //HasOne(x => x.Team2).Cascade.All();
            References(x => x.Team2)
                .Class<TeamEntity>()
                .Access.Property()
                .Cascade.All()
                .Not.LazyLoad()
                .Nullable()
                .Columns("Team2Id");
        }
    }
}
