using FluentNHibernate.Mapping;
using ScoreBoard.Infrastructure.Models;

namespace ScoreBoard.Infrastructure.Mappings
{
    // ReSharper disable once UnusedMember.Global
    public class TeamMapping : ClassMap<TeamEntity>
    {
        public TeamMapping()
        {
            Schema(@"dbo");
            Table(@"Team");
            Id(x => x.Id)
                .Column("Id")
                .CustomType("Guid")
                .Access.Property()
                .CustomSqlType("uniqueidentifier")
                .Not.Nullable()
                .GeneratedBy.Assigned();
            Map(x => x.Name)
                .Column("Name")
                .CustomType("String")
                .Access.Property()
                .Nullable()
                .Generated.Never().CustomSqlType("nvarchar(255)");
            References(x => x.Player1)
                .Class<PlayerEntity>()
                .Access.Property()
                .Cascade.None()
                .Not.LazyLoad()
                .Nullable()
                .Columns("Player1Id");
            References(x => x.Player2)
                .Class<PlayerEntity>()
                .Access.Property()
                .Cascade.None()
                .Not.LazyLoad()
                .Nullable()
                .Columns("Player2Id");
        }
    }
}
