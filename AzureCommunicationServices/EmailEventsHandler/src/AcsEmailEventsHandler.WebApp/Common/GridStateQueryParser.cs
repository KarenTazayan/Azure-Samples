using Microsoft.Data.SqlClient;
using MudBlazor;
using System.Data;

namespace AcsEmailEventsHandler.WebApp.Common;

public class GridStateQueryParser
{
    public static DynamicSqlQuery ParseGridStateToSql(GridState<EmailEvent> gridState)
    {
        var whereClauses = new List<string>();
        var parameters = new List<SqlParameter>();
        int paramIndex = 0; // Ensures unique parameter names

        // Handle Filtering
        foreach (var filter in gridState.FilterDefinitions)
        {
            var columnName = filter.Column?.PropertyName;
            //if (string.IsNullOrWhiteSpace(columnName)) continue;

            var parameterName = $"@p{paramIndex++}";

            // Check filter type
            if (filter.Value is string strValue)
            {
                switch (filter.Operator)
                {
                    case FilterOperator.String.Equal:
                        whereClauses.Add($"{columnName} = {parameterName}");
                        break;
                    case FilterOperator.String.NotEqual:
                        whereClauses.Add($"{columnName} <> {parameterName}");
                        break;
                    case FilterOperator.String.Contains:
                        whereClauses.Add($"{columnName} LIKE {parameterName}");
                        strValue = $"%{strValue}%";
                        break;
                    case FilterOperator.String.NotContains:
                        whereClauses.Add($"{columnName} NOT LIKE {parameterName}");
                        strValue = $"%{strValue}%";
                        break;
                    case FilterOperator.String.StartsWith:
                        whereClauses.Add($"{columnName} LIKE {parameterName}");
                        strValue = $"{strValue}%";
                        break;
                    case FilterOperator.String.EndsWith:
                        whereClauses.Add($"{columnName} LIKE {parameterName}");
                        strValue = $"%{strValue}";
                        break;
                    case FilterOperator.String.Empty:
                        whereClauses.Add($"({columnName} = '' OR {columnName} IS NULL)");
                        continue;
                    case FilterOperator.String.NotEmpty:
                        whereClauses.Add($"({columnName} <> '' AND {columnName} IS NOT NULL)");
                        continue;
                    default:
                        throw new NotSupportedException($"The filter operator '{filter.Operator}' is not supported.");
                }
                parameters.Add(new SqlParameter(parameterName, SqlDbType.NVarChar) { Value = strValue });
            }
            else if (filter.Value is DateTime dateValue)
            {
                switch (filter.Operator)
                {
                    case FilterOperator.DateTime.Is:
                        whereClauses.Add($"{columnName} = {parameterName}");
                        break;
                    case FilterOperator.DateTime.IsNot:
                        whereClauses.Add($"{columnName} <> {parameterName}");
                        break;
                    case FilterOperator.DateTime.After:
                        whereClauses.Add($"{columnName} > {parameterName}");
                        break;
                    case FilterOperator.DateTime.OnOrAfter:
                        whereClauses.Add($"{columnName} >= {parameterName}");
                        break;
                    case FilterOperator.DateTime.Before:
                        whereClauses.Add($"{columnName} < {parameterName}");
                        break;
                    case FilterOperator.DateTime.OnOrBefore:
                        whereClauses.Add($"{columnName} <= {parameterName}");
                        break;
                    case FilterOperator.DateTime.Empty:
                        whereClauses.Add($"{columnName} IS NULL");
                        continue;
                    case FilterOperator.DateTime.NotEmpty:
                        whereClauses.Add($"{columnName} IS NOT NULL");
                        continue;
                    default:
                        throw new NotSupportedException($"The filter operator '{filter.Operator}' is not supported.");
                }
                parameters.Add(new SqlParameter(parameterName, SqlDbType.DateTime) { Value = dateValue });
            }
            else if (filter.Value is int intValue)
            {
                switch (filter.Operator)
                {
                    case FilterOperator.Number.Equal:
                        whereClauses.Add($"{columnName} = {parameterName}");
                        break;
                    case FilterOperator.Number.NotEqual:
                        whereClauses.Add($"{columnName} <> {parameterName}");
                        break;
                    case FilterOperator.Number.GreaterThan:
                        whereClauses.Add($"{columnName} > {parameterName}");
                        break;
                    case FilterOperator.Number.GreaterThanOrEqual:
                        whereClauses.Add($"{columnName} >= {parameterName}");
                        break;
                    case FilterOperator.Number.LessThan:
                        whereClauses.Add($"{columnName} < {parameterName}");
                        break;
                    case FilterOperator.Number.LessThanOrEqual:
                        whereClauses.Add($"{columnName} <= {parameterName}");
                        break;
                    default:
                        throw new NotSupportedException($"The filter operator '{filter.Operator}' is not supported.");
                }
                parameters.Add(new SqlParameter(parameterName, SqlDbType.Int) { Value = intValue });
            }
        }

        // Construct WHERE Clause
        var whereClause = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : "";

        // Handle Sorting
        var orderByClauses = new List<string>();
        foreach (var sort in gridState.SortDefinitions)
        {
            var sortColumn = sort.SortBy;
            var sortDirection = sort.Descending ? "DESC" : "ASC";
            orderByClauses.Add($"{sortColumn} {sortDirection}");
        }
        var orderByClause = orderByClauses.Count > 0 ? "ORDER BY " + string.Join(", ", orderByClauses) : "ORDER BY EventDateTime DESC";

        return new DynamicSqlQuery(whereClause, orderByClause, parameters);
    }

}