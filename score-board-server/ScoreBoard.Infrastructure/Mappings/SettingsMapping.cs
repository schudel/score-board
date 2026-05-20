using FluentNHibernate.Mapping;
using ScoreBoard.Infrastructure.Models;

namespace ScoreBoard.Infrastructure.Mappings
{
    // ReSharper disable once UnusedMember.Global
    public class SettingsMapping : ClassMap<SettingsEntity>
    {
        public SettingsMapping()
        {
            Schema(@"dbo");
            Table(@"Settings");
            Id(x => x.Id)
                .Column("Id")
                .CustomType("Guid")
                .Access.Property()
                .CustomSqlType("uniqueidentifier")
                .Not.Nullable()
                .GeneratedBy.Assigned();
            Map(x => x.LanguageCode)
                .Column("LanguageCode")
                .CustomType("String")
                .Access.Property()
                .Nullable()
                .Generated.Never().CustomSqlType("nvarchar(10)")
                .Length(10);
            Map(x => x.DarkMode)
                .Column("DarkMode")
                .CustomType("Boolean")
                .Access.Property()
                .Generated.Never().CustomSqlType("bit");
            Map(x => x.DashboardLayout)
                .Column("DashboardLayout")
                .CustomType("String")
                .Access.Property()
                .Nullable()
                .Generated.Never().CustomSqlType("nvarchar(max)");
        }
    }
}
