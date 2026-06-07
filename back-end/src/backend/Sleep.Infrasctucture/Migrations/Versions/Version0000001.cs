using FluentMigrator;
using Sleep.Infrastructure.Migrations;

namespace Sleep.Infrasctructure.Migrations.Versions
{
    [Migration(DatabaseVersions.TABLE_USER, "Create table to save the user's information")]
    public class Version0000001 : VersionBase
    { 
        public override void Up()
        {
            CreateTable("users")
                .WithColumn("Birth_Date").AsDateTime().NotNullable()
                .WithColumn("Name").AsString(60).NotNullable()
                .WithColumn("Gender").AsCustom("ENUM('F','M')").NotNullable()
                .WithColumn("Email").AsString(100).NotNullable()
                .WithColumn("Password").AsString().NotNullable()
                .WithColumn("height_cm").AsDecimal(6,2).NotNullable()
                .WithColumn("weight_kg").AsDecimal(6,2).NotNullable()
                .WithColumn("Status").AsInt64().NotNullable().WithDefaultValue(1);
        }
    }
}
