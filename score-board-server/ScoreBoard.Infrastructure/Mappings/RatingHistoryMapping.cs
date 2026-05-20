using FluentNHibernate.Mapping;
using ScoreBoard.Infrastructure.Models;

namespace ScoreBoard.Infrastructure.Mappings
{
    // ReSharper disable once UnusedMember.Global
    public class RatingHistoryMapping : ClassMap<RatingHistoryEntity>
    {
        public RatingHistoryMapping()
        {
            Schema(@"dbo");
            Table(@"RatingHistory");
            Id(x => x.Id)
                .Column("Id")
                .CustomType("Guid")
                .Access.Property()
                .CustomSqlType("uniqueidentifier")
                .Not.Nullable()
                .GeneratedBy.Assigned();
            Map(x => x.GameId)
                .Column("GameId")
                .CustomType("Guid")
                .Access.Property()
                .CustomSqlType("uniqueidentifier")
                .Not.Nullable();
            Map(x => x.MatchId)
                .Column("MatchId")
                .CustomType("Guid")
                .Access.Property()
                .CustomSqlType("uniqueidentifier")
                .Not.Nullable();
            References(x => x.Player)
                .Class<PlayerEntity>()
                .Access.Property()
                .Cascade.None()
                .LazyLoad()
                .Nullable()
                .Columns("PlayerId");
            Map(x => x.DateTime)
                .Column("DateTime")
                .CustomType("DateTime")
                .Access.Property()
                .Generated.Never().CustomSqlType("datetime");
            Map(x => x.ConservativeRating)
                .Column("ConservativeRating")
                .CustomType("Double")
                .Access.Property();
            Map(x => x.Mean)
                .Column("Mean")
                .CustomType("Double")
                .Access.Property();
            Map(x => x.StandardDeviation)
                .Column("StandardDeviation")
                .CustomType("Double")
                .Access.Property();
        }
    }
}
