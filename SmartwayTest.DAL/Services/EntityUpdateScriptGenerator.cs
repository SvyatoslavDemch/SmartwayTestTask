using Dapper;
using SmartwayTest.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartwayTest.DAL.Services
{
    public class EntityUpdateScriptGenerator
    {
        public (string script, DynamicParameters parameters) CreateUpdateScript<T>(T currentObject, T updatedObject)
            where T : IEntity
        {
            var parameters = new DynamicParameters();
            var changedFields = new List<string>();
            var properties = typeof(T).GetProperties();
            var tableName = GetTableName<T>();
            var updateScript = new StringBuilder($"UPDATE {tableName} SET ");

            foreach (var property in properties)
            {
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string)
                    || property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    continue;

                object currentValue = property.GetValue(currentObject);
                object updatedValue = property.GetValue(updatedObject);

                if (!Equals(currentValue, updatedValue))
                {
                    parameters.Add($"@{property.Name}", updatedValue);
                    changedFields.Add(property.Name);
                }
            }

            if (changedFields.Count == 0)
                throw new Exception("Не найдены поля для обновления");

            var idProperty = properties.FirstOrDefault(p => p.Name == "Id");
            if (idProperty != null)
            {
                object idValue = idProperty.GetValue(currentObject);
                parameters.Add("@Id", idValue);
            }
            updateScript.Append(string.Join(", ", changedFields.Select(field => $"{field} = @{field}")));
            updateScript.Append(" WHERE Id = @Id");

            return (updateScript.ToString(), parameters);
        }
        private string GetTableName<T>()
        {
            var tableAttribute = typeof(T).GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;
            return tableAttribute.Name;
        }
    }
}
