using FluentNHibernate.Mapping;
using ScoreBoard.Infrastructure.Models;

namespace ScoreBoard.Infrastructure.Mappings
{
    // ReSharper disable once UnusedMember.Global
    public class GameMapping : ClassMap<GameEntity>
    {
        public GameMapping()
        {
            Schema(@"dbo");
            Table(@"Game");
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
                .Not.Nullable()
                .Generated.Never().CustomSqlType("nvarchar(255)")
                .Length(255);
            Map(x => x.Type)
                .Column("Type")
                .CustomType("String")
                .Access.Property()
                .Generated.Never().CustomSqlType("nvarchar(255)")
                .Length(255);
            Map(x => x.Genre)
                .Column("Genre")
                .CustomType("String")
                .Access.Property()
                .Generated.Never().CustomSqlType("nvarchar(255)")
                .Length(255);
            Map(x => x.Image)
                .Column("Image")
                .CustomType("String")
                .Access.Property()
                .Generated.Never().CustomSqlType("nvarchar(MAX)");
            Map(x => x.Beta)
                .Column("Beta")
                .CustomType("Double")
                .Access.Property();
            Map(x => x.DrawProbability)
                .Column("DrawProbability")
                .CustomType("Double")
                .Access.Property();
            Map(x => x.DynamicsFactor)
                .Column("DynamicsFactor")
                .CustomType("Double")
                .Access.Property();
            Map(x => x.InitialConservativeRating)
                .Column("InitialConservativeRating")
                .CustomType("Double")
                .Access.Property();
            Map(x => x.InitialMean)
                .Column("InitialMean")
                .CustomType("Double")
                .Access.Property();
            Map(x => x.InitialStandardDeviation)
                .Column("InitialStandardDeviation")
                .CustomType("Double")
                .Access.Property();
        }
    }
}
