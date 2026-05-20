using FluentNHibernate.Mapping;
using ScoreBoard.Infrastructure.Models;

namespace ScoreBoard.Infrastructure.Mappings
{
    // ReSharper disable once UnusedMember.Global
    public class PasswordResetRequestMapping : ClassMap<PasswordResetRequestEntity>
    {
        public PasswordResetRequestMapping()
        {
            Schema(@"dbo");
            Table(@"PasswordResetRequest");
            Id(x => x.Id)
                .Column("Id")
                .CustomType("Guid")
                .Access.Property()
                .CustomSqlType("uniqueidentifier")
                .Not.Nullable()
                .GeneratedBy.Assigned();
            Map(x => x.PlayerId)
                .Column("PlayerId")
                .CustomType("Guid")
                .Access.Property()
                .CustomSqlType("uniqueidentifier")
                .Not.Nullable();
            Map(x => x.TimeStamp)
                .Column("TimeStamp")
                .CustomType("DateTime")
                .Access.Property()
                .Generated.Never().CustomSqlType("datetime");
        }
    }
}
