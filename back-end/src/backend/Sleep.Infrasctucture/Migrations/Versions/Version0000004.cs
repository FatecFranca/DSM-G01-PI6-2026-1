using FluentMigrator;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Sleep.Infrastructure.Migrations;

namespace Sleep.Infrasctructure.Migrations.Versions
{
    [Migration(DatabaseVersions.TABLE_SLEEP_RECORD, "Creating table for user save sleep record's")]
    public class Version0000004 : VersionBase
    {
        public override void Up()
        {
            CreateTable("sleep_record")
                .WithColumn("record_date").AsDateTime().NotNullable()
                .WithColumn("sleep_start").AsDateTime().NotNullable()
                .WithColumn("sleep_end").AsDateTime().NotNullable()
                .WithColumn("duration_hours").AsDecimal(3,1).NotNullable()
                .WithColumn("quality_of_sleep").AsInt16().NotNullable()
                .WithColumn("stress_level").AsInt16().NotNullable()
                .WithColumn("physical_activity_minutes").AsInt32().NotNullable()
                .WithColumn("bmi_category").AsInt32().NotNullable()
                .WithColumn("blood_pressure").AsDecimal(5, 2).NotNullable()
                .WithColumn("notes").AsString().Nullable()
                .WithColumn("age").AsInt16().NotNullable()
                .WithColumn("heart_rate").AsInt64().NotNullable()
                .WithColumn("daily_steps").AsInt64().NotNullable()
                .WithColumn("user_id").AsInt64().NotNullable()
                    .ForeignKey("users", "id");
        }
    }
}
