using FluentMigrator;

namespace Homework7.Migrations;

[Migration(181020240001)]
public class AddPlayersTable : Migration
{
    public override void Up()
    {
        if (Schema.Table("players").Exists()) return;
        
        Create.Table("players")
            .WithColumn("name")
                .AsString(40)
            .WithColumn("player_id")
                .AsString(10)
                .Unique()
            .WithColumn("country_id")
                .AsString(3)
                .ForeignKey("countries", "country_id")
            .WithColumn("birthdate")
                .AsDate();
    }

    public override void Down()
    {
        if (Schema.Table("players").Exists() is false) return;
        
        Delete.Table("players");
    }
}