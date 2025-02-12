using Microsoft.Data.SqlClient;

namespace AcsEmailEventsHandler;

public record DynamicSqlQuery(string WhereClause, string OrderByClause, List<SqlParameter> Parameters);