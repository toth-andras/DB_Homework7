using FluentMigrator;

namespace Homework7.Migrations;

[Migration(181020240000)]
public class AddOlympicsTable : Migration 
{
    public override void Up()
    {
        if (Schema.Table("olympics").Exists()) return;
        
        Create.Table("olympics")
            .WithColumn("olympic_id")
                .AsString(7)
                .Unique()
            .WithColumn("country_id")
                .AsString(3)
                .ForeignKey("countries", "country_id")
            .WithColumn("city")
                .AsString(50)
                .Nullable()
            .WithColumn("year")
                .AsInt32()
                .Nullable()
            .WithColumn("startdate")
                .AsDate()
                .Nullable()
            .WithColumn("enddate")
                .AsDate()
                .Nullable();
    }

    public override void Down()
    {
        if (Schema.Table("olympics").Exists()) return;

        Delete.Table("olympics");
    }
}