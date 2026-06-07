using FluentMigrator;
using Sleep.Infrastructure.Migrations;

namespace Sleep.Infrasctructure.Migrations.Versions
{
    [Migration(DatabaseVersions.USER_ADD_SLEEP_GOAL, "Adding column Sleep Goal for table users")]
    public class Version0000008 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("users")
                .AddColumn("sleep_goal")
                .AsString(10);
        }
    }
}
