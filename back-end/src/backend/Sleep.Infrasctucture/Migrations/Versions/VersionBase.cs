using FluentMigrator;
using FluentMigrator.Builders.Create.Table;

namespace Sleep.Infrasctructure.Migrations.Versions
{
    public abstract class VersionBase : ForwardOnlyMigration
    {
       protected ICreateTableColumnOptionOrWithColumnSyntax CreateTable(string table)
        {
            return Create.Table(table)
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Created_On").AsDateTime().NotNullable()
                .WithColumn("Last_Updated").AsDateTime();
        }
    }
}
