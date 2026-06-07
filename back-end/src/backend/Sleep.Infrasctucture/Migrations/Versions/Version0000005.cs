using FluentMigrator;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Sleep.Infrastructure.Migrations;

namespace Sleep.Infrasctructure.Migrations.Versions
{
    [Migration(DatabaseVersions.SLEEP_RECORD_ADD_UK_RECORD_DATE, "Adding Unique Key to Record Date and User")]
    public class Version0000005 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Create.UniqueConstraint("sleep_record_date_user_UK")
                .OnTable("sleep_record")
                .Columns("record_date", "user_id");
        }
    }
}
