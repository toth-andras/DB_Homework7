namespace Homework7.Migrations;

using FluentMigrator;

[Migration(181020240031)] 
public class CreateResultsTable : Migration
{
    public override void Up()
    {
        if (Schema.Table("results").Exists()) return;
        
        Create.Table("results")
            .WithColumn("event_id")
                .AsString(7)
                .NotNullable()
                .ForeignKey("events", "event_id")
            .WithColumn("player_id")
                .AsString(10)
                .NotNullable()
                .ForeignKey("players", "player_id")
            .WithColumn("medal")
                .AsString(7)
                .Nullable()
            .WithColumn("result")
                .AsDouble(); 
    }

    public override void Down()
    {
        if (Schema.Table("results").Exists() is false) return;
        
        Delete.Table("results");
    }
}
