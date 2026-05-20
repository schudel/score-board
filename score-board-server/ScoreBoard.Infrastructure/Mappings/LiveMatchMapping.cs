using FluentNHibernate.Mapping;
using ScoreBoard.Infrastructure.Models;

namespace ScoreBoard.Infrastructure.Mappings
{
    // ReSharper disable once UnusedMember.Global
    public class LiveMatchMapping : ClassMap<LiveMatchEntity>
    {
        public LiveMatchMapping()
        {
            Schema(@"dbo");
            Table(@"LiveMatch");
            Id(x => x.Id)
                .Column("Id")
                .CustomType("Guid")
                .Access.Property()
                .CustomSqlType("uniqueidentifier")
                .Not.Nullable()
                .GeneratedBy.Assigned();
            Map(x => x.MatchId)
                .Column("MatchId")
                .CustomType("Guid")
                .Access.Property()
                .CustomSqlType("uniqueidentifier")
                .Not.Nullable();
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
            Map(x => x.State)
                .Column("State")
                .CustomType("Int32")
                .Access.Property()
                .Generated.Never().CustomSqlType("int")
                .Not.Nullable()
                .Precision(10);
            Map(x => x.TimeStamp)
                .Column("TimeStamp")
                .CustomType("DateTime")
                .Access.Property()
                .Generated.Never().CustomSqlType("datetime");
        }
    }
}
