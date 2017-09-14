﻿using Dotmim.Sync.Builders;
using System.Text;
using Dotmim.Sync.Data;
using System.Data.Common;

namespace Dotmim.Sync.MySql
{

    /// <summary>
    /// The MySqlBuilder class is the MySql implementation of DbBuilder class.
    /// In charge of creating tracking table, stored proc, triggers and adapters.
    /// </summary>
    public class MySqlBuilder : DbBuilder
    {

        MySqlObjectNames sqlObjectNames;
       
        public MySqlBuilder(DmTable tableDescription, DbBuilderOption option = DbBuilderOption.CreateOrUseExistingSchema)
            : base(tableDescription, option)
        {
            sqlObjectNames = new MySqlObjectNames(tableDescription);
        }

        internal static (ObjectNameParser tableName, ObjectNameParser trackingName) GetParsers(DmTable tableDescription)
        {
            string tableAndPrefixName = tableDescription.TableName;
            var originalTableName = new ObjectNameParser(tableAndPrefixName.ToLowerInvariant(), "`", "`");
            var trackingTableName = new ObjectNameParser($"{tableAndPrefixName.ToLowerInvariant()}_tracking", "`", "`");

            return (originalTableName, trackingTableName);
        }
        public static string WrapScriptTextWithComments(string commandText, string commentText)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder1 = new StringBuilder("\n");

            string str = stringBuilder1.ToString();
            stringBuilder.AppendLine("DELIMITER $$ ");
            stringBuilder.Append(string.Concat("-- BEGIN ", commentText, str));
            stringBuilder.Append(commandText);
            stringBuilder.Append(string.Concat("-- END ", commentText, str, "\n"));
            stringBuilder.AppendLine("$$ ");
            stringBuilder.AppendLine("DELIMITER ;");
            return stringBuilder.ToString();
        }

         public override IDbBuilderProcedureHelper CreateProcBuilder(DbConnection connection, DbTransaction transaction = null)
        {
            return new MySqlBuilderProcedure(TableDescription, connection, transaction);
        }

        public override IDbBuilderTriggerHelper CreateTriggerBuilder(DbConnection connection, DbTransaction transaction = null)
        {
            return new MySqlBuilderTrigger(TableDescription, connection, transaction);
        }

        public override IDbBuilderTableHelper CreateTableBuilder(DbConnection connection, DbTransaction transaction = null)
        {
            return new MySqlBuilderTable(TableDescription, connection, transaction);
        }

        public override IDbBuilderTrackingTableHelper CreateTrackingTableBuilder(DbConnection connection, DbTransaction transaction = null)
        {
            return new MySqlBuilderTrackingTable(TableDescription, connection, transaction);
        }

        public override DbSyncAdapter CreateSyncAdapter(DbConnection connection, DbTransaction transaction = null)
        {
            return new MySqlSyncAdapter(TableDescription, connection, transaction);
        }
    }
}
