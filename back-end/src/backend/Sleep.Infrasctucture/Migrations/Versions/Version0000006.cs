using FluentMigrator;
using FluentMigrator;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Sleep.Infrastructure.Migrations;

namespace Sleep.Infrasctructure.Migrations.Versions
{
    [Migration(DatabaseVersions.USER_ADD_SLEEP_DISORDER, "Adding column Sleep Disorder for table users")]
    public class Version0000006 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("users")
                .AddColumn("sleep_disorder")
                .AsInt16()
                .NotNullable();
        }
    }
}
