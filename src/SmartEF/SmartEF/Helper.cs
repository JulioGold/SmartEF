using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SmartEF
{
    /// <summary>
    /// Helper do dominio.
    /// </summary>
    public sealed class Helper
    {
        /// <summary>
        /// Get the table name according with passed entity.
        /// </summary>
        /// <typeparam name="TTableEntity">Entity with data annotation.</typeparam>
        /// <returns></returns>
        public static string GetDBTableName<TTableEntity>() where TTableEntity : class
        {
            var tableAttribute = (typeof(TTableEntity)
                .GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute), inherit: true)?
                .Where(p => p.GetType() == typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute))?
                .FirstOrDefault() as System.ComponentModel.DataAnnotations.Schema.TableAttribute)?.Name;

            return tableAttribute;
        }

        /// <summary>
        /// Get the column name according with passed entity and wanted property.
        /// </summary>
        /// <typeparam name="TTableEntity">Entity with data annotation.</typeparam>
        /// <param name="propertyName">Property name of entity where will take the table name.</param>
        /// <returns></returns>
        public static string GetDBColumnName<TTableEntity>(string propertyName) where TTableEntity : class
        {
            var columnAttribute = (typeof(TTableEntity)
                .GetProperties()?
                .Where(p => p.Name == propertyName && p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute), inherit: true).Any())?
                .FirstOrDefault()?
                .CustomAttributes?
                .FirstOrDefault(w => w.AttributeType == typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute))?
                .ConstructorArguments.FirstOrDefault().Value) as string;

            return columnAttribute;
        }

        /// <summary>
        /// Get the columns name according with passed entity.
        /// </summary>
        /// <typeparam name="TTableEntity">Entity with data annotation.</typeparam>
        /// <returns></returns>
        public static List<string> GetDBColumnsName<TTableEntity>() where TTableEntity : class
        {
            var columnsName = (typeof(TTableEntity)
                .GetProperties()?
                .Where(p => p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute), inherit: true).Any())?
                .ToList()
                .SelectMany(ff => ff.CustomAttributes?
                    .Where(w => w.AttributeType == typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute))?
                    .Select(sss => sss.ConstructorArguments.Select(s => s.Value as string).ToList()))
                .SelectMany(s => s.ToList())
                .ToList());

            return columnsName;
        }

        /// <summary>
        /// Generate the SQL statement for deletion of one or many records.
        /// </summary>
        /// <param name="ids">Id list.</param>
        /// <param name="dbSchema">Schema of database.</param>
        /// <param name="dbTableName">Database table name.</param>
        /// <param name="dbColumnName">Database column name.</param>
        /// <returns></returns>
        public static string DeleteSQLCommand(List<int> ids, string dbSchema, string dbTableName, string dbColumnName)
        {
            string cmd = default(string);

            if (ids.Count > 0 && !String.IsNullOrEmpty(dbSchema) && !String.IsNullOrEmpty(dbTableName) && !String.IsNullOrEmpty(dbColumnName))
            {
                cmd = $"DELETE FROM [{dbSchema}].[{dbTableName}] WHERE [{dbColumnName}] IN ({String.Join(",", ids)});";
            }

            return cmd;
        }

        /// <summary>
        /// Generate the SQL statement for deletion of one or many records.
        /// </summary>
        /// <typeparam name="TTableEntity">Entity with data annotation.</typeparam>
        /// <param name="ids">Id list.</param>
        /// <param name="dbSchema">Schema of database.</param>
        /// <param name="entityPropertyName">Entity property name.</param>
        /// <returns></returns>
        public static string DeleteSQLCommand<TTableEntity>(List<int> ids, string dbSchema, string entityPropertyName) where TTableEntity : class
        {
            string dbTableName = GetDBTableName<TTableEntity>();
            string dbColumnName = GetDBColumnName<TTableEntity>(entityPropertyName);

            return DeleteSQLCommand(ids, dbSchema, dbTableName, dbColumnName);
        }

        /// <summary>
        /// Generate the SQL statement for deletion of one or many records.
        /// </summary>
        /// <typeparam name="TTableEntity">Entity with data annotation.</typeparam>
        /// <param name="ids">Id list.</param>
        /// <param name="dbSchema">Schema of database.</param>
        /// <param name="entityPropertyName">Expression to get the entity property name.</param>
        /// <returns></returns>
        public static string DeleteSQLCommand<TTableEntity>(List<int> ids, string dbSchema, Expression<Func<TTableEntity, int>> entityPropertyName) where TTableEntity : class
        {
            MemberExpression body = entityPropertyName.Body as MemberExpression;

            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)entityPropertyName.Body;
                body = ubody.Operand as MemberExpression;
            }

            string targetPropertyName = body.Member.Name;
            string dbTableName = GetDBTableName<TTableEntity>();
            string dbColumnName = GetDBColumnName<TTableEntity>(targetPropertyName);

            return DeleteSQLCommand(ids, dbSchema, dbTableName, dbColumnName);
        }

        /// <summary>
        /// Generate the SQL statement for deletion of one or many records.
        /// </summary>
        /// <typeparam name="TTableEntity">Entity with data annotation.</typeparam>
        /// <typeparam name="TColumn">Column with data annotation.</typeparam>
        /// <param name="ids">Id list.</param>
        /// <param name="dbSchema">Schema of database.</param>
        /// <param name="entityPropertyName">Expression to get the entity property name.</param>
        /// <returns></returns>
        public static string DeleteSQLCommand<TTableEntity, TColumn>(List<int> ids, string dbSchema, Expression<Func<TTableEntity, TColumn>> entityPropertyName) where TTableEntity : class
        {
            MemberExpression body = entityPropertyName.Body as MemberExpression;

            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)entityPropertyName.Body;
                body = ubody.Operand as MemberExpression;
            }

            string targetPropertyName = body.Member.Name;
            string dbTableName = GetDBTableName<TTableEntity>();
            string dbColumnName = GetDBColumnName<TTableEntity>(targetPropertyName);

            return DeleteSQLCommand(ids, dbSchema, dbTableName, dbColumnName);
        }
    }
}
