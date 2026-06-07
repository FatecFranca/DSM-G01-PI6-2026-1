using FluentMigrator;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Sleep.Infrastructure.Migrations;

namespace Sleep.Infrasctructure.Migrations.Versions
{
    [Migration(DatabaseVersions.SLEEP_RECORDS_NEW_FIELDS_FOR_ANALYSES, "Adding column screen_before_sleep, caffeine, alcohol and mental_fatigue to sleep_record's table")]
    public class Version0000007 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("sleep_record")
                .AddColumn("caffeine").AsBoolean().NotNullable()
                .AddColumn("alcohol").AsBoolean().NotNullable()
                .AddColumn("screen_before_sleep").AsBoolean().NotNullable()
                .AddColumn("mental_fatigue").AsInt16().NotNullable();
        }
    }
}
