using FluentMigrator;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Sleep.Infrastructure.Migrations;

namespace Sleep.Infrasctructure.Migrations.Versions
{
    [Migration(DatabaseVersions.USER_ADD_OCCUPATION, "Adding column Occupation for table users")]
    public class Version0000003 : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table("users")
                .AddColumn("Occupation")
                .AsString()
                .NotNullable();
        }
    }
}
