using FluentMigrator;
using Sleep.Infrastructure.Migrations;

namespace Sleep.Infrasctructure.Migrations.Versions
{
    [Migration(DatabaseVersions.TABLE_SLEEP_ANALYSIS, "Creating table for sleep analysis with AI")]
    public class Version0000009 : VersionBase
    {
        public override void Up()
        {
            CreateTable("sleep_analysis")
                .WithColumn("problem").AsString().NotNullable()
                .WithColumn("score").AsDecimal(4,2).NotNullable()
                .WithColumn("sleep_record_id").AsInt64().NotNullable()
                    .ForeignKey("sleep_record", "id");
        }
    }
}
