using FluentNHibernate.Mapping;
using ScoreBoard.Infrastructure.Models;

namespace ScoreBoard.Infrastructure.Mappings
{
    // ReSharper disable once UnusedMember.Global
    public class RatingMapping : ClassMap<RatingEntity>
    {
        public RatingMapping()
        {
            Schema(@"dbo");
            Table(@"Rating");
            Id(x => x.Id)
                .Column("Id")
                .CustomType("Guid")
                .Access.Property()
                .CustomSqlType("uniqueidentifier")
                .Not.Nullable()
                .GeneratedBy.Assigned();
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
            References(x => x.Game)
                .Class<GameEntity>()
                .Access.Property()
                .Cascade.None()
                .LazyLoad()
                .Nullable()
                .Columns("GameId");
            References(x => x.Player)
                .Class<PlayerEntity>()
                .Access.Property()
                .Cascade.None()
                .LazyLoad()
                .Nullable()
                .Columns("PlayerId");
        }
    }
}
