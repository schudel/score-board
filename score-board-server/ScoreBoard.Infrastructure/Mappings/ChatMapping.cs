using FluentNHibernate.Mapping;
using ScoreBoard.Infrastructure.Models;

namespace ScoreBoard.Infrastructure.Mappings
{
    // ReSharper disable once UnusedMember.Global
    public class ChatMapping : ClassMap<ChatEntity>
    {
        public ChatMapping()
        {
            Schema(@"dbo");
            Table(@"Chat");
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
            Map(x => x.UserName)
                .Column("UserName")
                .CustomType("String")
                .Access.Property()
                .Not.Nullable()
                .Generated.Never().CustomSqlType("nvarchar(255)")
                .Length(255);
            Map(x => x.Message)
                .Column("Name")
                .CustomType("String")
                .Access.Property()
                .Not.Nullable()
                .Generated.Never().CustomSqlType("nvarchar(max)");
            Map(x => x.Room)
                .Column("Room")
                .CustomType("String")
                .Access.Property()
                .Not.Nullable()
                .Generated.Never().CustomSqlType("nvarchar(255)")
                .Length(255);
            Map(x => x.TimeStamp)
                .Column("TimeStamp")
                .CustomType("DateTime")
                .Access.Property()
                .Generated.Never().CustomSqlType("datetime");
        }
    }
}
