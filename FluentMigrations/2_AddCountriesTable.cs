using FluentMigrator;

namespace Homework7.Migrations;

[Migration(171020242320)]
public class AddCountriesTable : Migration 
{
    public override void Up()
    {
        if (Schema.Table("countries").Exists()) return;
        
        Create.Table("countries")
            .WithColumn("name")
                .AsString(40)
            .WithColumn("country_id")
                .AsString(3)
                .Unique()
            .WithColumn("area_sqkm")
                .AsInt32()
                .Nullable()
            .WithColumn("population")
                .AsInt32()
                .Nullable();
    }

    public override void Down()
    {
        if (Schema.Table("countries").Exists() is false) return;

        Delete.Table("countries");
    }
}