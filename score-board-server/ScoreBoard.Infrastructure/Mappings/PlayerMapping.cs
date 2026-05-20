using FluentNHibernate.Mapping;
using ScoreBoard.Infrastructure.Models;

namespace ScoreBoard.Infrastructure.Mappings
{
    // ReSharper disable once UnusedMember.Global
    public class PlayerMapping : ClassMap<PlayerEntity>
    {
        public PlayerMapping()
        {
            Schema(@"dbo");
            Table(@"Player");
            Id(x => x.Id)
                .Column("Id")
                .CustomType("Guid")
                .Access.Property()
                .CustomSqlType("uniqueidentifier")
                .Not.Nullable()
                .GeneratedBy.Assigned();
            Map(x => x.PlayerName)
                .Column("PlayerName")
                .CustomType("String")
                .Access.Property()
                .Not.Nullable()
                .Generated.Never().CustomSqlType("nvarchar(255)")
                .Length(255);
            Map(x => x.FirstName)
                .Column("FirstName")
                .CustomType("String")
                .Access.Property()
                .Nullable()
                .Generated.Never().CustomSqlType("nvarchar(255)")
                .Length(255);
            Map(x => x.LastName)
                .Column("LastName")
                .CustomType("String")
                .Access.Property()
                .Nullable()
                .Generated.Never().CustomSqlType("nvarchar(255)")
                .Length(255);
            Map(x => x.Email)
                .Column("Email")
                .CustomType("String")
                .Access.Property()
                .Nullable()
                .Generated.Never().CustomSqlType("nvarchar(255)")
                .Length(255);
            Map(x => x.PasswordHash)
                .Column("PasswordHash")
                .Access.Property()
                .Generated.Never().CustomSqlType("varbinary(MAX)")
                .Length(int.MaxValue)
                .Not.Nullable();
            Map(x => x.PasswordSalt)
                .Column("PasswordSalt")
                .Access.Property()
                .Generated.Never().CustomSqlType("varbinary(MAX)")
                .Length(int.MaxValue)
                .Not.Nullable();
            Map(x => x.IsActive)
                .Column("IsActive")
                .CustomType("Boolean")
                .Access.Property()
                .Generated.Never().CustomSqlType("bit");
            Map(x => x.MustChangePassword)
                .Column("MustChangePassword")
                .CustomType("Boolean")
                .Access.Property()
                .Generated.Never().CustomSqlType("bit");
            Map(x => x.Role)
                .Column("Role")
                .CustomType("Int32")
                .Access.Property()
                .Generated.Never().CustomSqlType("int")
                .Not.Nullable()
                .Precision(10);
            Map(x => x.LastLogin)
                .Column("LastLogin")
                .CustomType("DateTime")
                .Access.Property()
                .Nullable()
                .Generated.Never().CustomSqlType("datetime");
            Map(x => x.RegistrationDate)
                .Column("RegistrationDate")
                .CustomType("DateTime")
                .Access.Property()
                .Nullable()
                .Generated.Never().CustomSqlType("datetime");
            Map(x => x.ActivationDate)
                .Column("ActivationDate")
                .CustomType("DateTime")
                .Access.Property()
                .Nullable()
                .Generated.Never().CustomSqlType("datetime");
            Map(x => x.Image)
                .Column("Image")
                .CustomType("String")
                .Access.Property()
                .Generated.Never().CustomSqlType("nvarchar(MAX)");
            References(x => x.Settings)
                .Class<SettingsEntity>()
                .Access.Property()
                .Cascade.All()
                .LazyLoad()
                .Nullable()
                .Columns("SettingsId");
        }
    }
}
