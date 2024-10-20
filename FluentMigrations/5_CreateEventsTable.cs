namespace Homework7.Migrations;

using FluentMigrator;

[Migration(181020240015)] 
public class CreateEventsTable : Migration
{
    public override void Up()
    {
        if (Schema.Table("events").Exists()) return;
        Create.Table("events")
            .WithColumn("event_id")
                .AsString(7)
                .Unique()
            .WithColumn("name")
                .AsString(40)
            .WithColumn("eventtype")
                .AsString(20)
            .WithColumn("olympic_id")
                .AsString(7)
                .ForeignKey("olympics", "olympic_id")
            .WithColumn("is_team_event")
                .AsInt32()
            .WithColumn("num_players_in_team")
                .AsInt32()
                .Nullable()
            .WithColumn("result_noted_in")
                .AsString(100)
                .Nullable();

        Execute.Sql("alter table events add constraint team_event_constraint CHECK (is_team_event = ANY (ARRAY [0, 1]))");
    }

    public override void Down()
    {
        if (Schema.Table("events").Exists() is false) return;
        Delete.Table("events");
    }
}
