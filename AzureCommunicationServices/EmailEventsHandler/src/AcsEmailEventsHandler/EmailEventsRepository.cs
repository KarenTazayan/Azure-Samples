using CSharpFunctionalExtensions;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace AcsEmailEventsHandler;

public class EmailEventsRepository
{
    private readonly string _connectionString;

    public EmailEventsRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException($"Value for {nameof(connectionString)} is null or empty.");
        }
        _connectionString = connectionString;
    }

    public void Insert(EmailEvent emailEvent)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
            
        using var command = new SqlCommand("INSERT INTO EmailEvents (EventDateTime, EventType, EventPayload) VALUES (@EventDateTime, @EventType, @EventPayload)", connection);
            
        command.Parameters.AddWithValue("@EventDateTime", emailEvent.EventDateTime);
        command.Parameters.AddWithValue("@EventType", emailEvent.EventType);
        command.Parameters.AddWithValue("@EventPayload", emailEvent.EventPayload);

        command.ExecuteNonQuery();
    }

    public async IAsyncEnumerable<EmailEvent> GetEvents(bool formatJson = false)
    {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        var sqlCommandText = "SELECT EventDateTime, EventType, EventPayload FROM EmailEvents";
        await using var command = new SqlCommand(sqlCommandText, connection);
        await using var reader = await command.ExecuteReaderAsync();
        
        while (await reader.ReadAsync())
        {
            var eventPayload = reader.GetString(2);
            if (formatJson)
            {
                eventPayload = JsonSerializer.Serialize(eventPayload, new JsonSerializerOptions()
                    { WriteIndented = true });
            }

            yield return new EmailEvent(reader.GetDateTime(0), reader.GetString(1), eventPayload);
            //await Task.Delay(300);
        }
    }

    public async Task<PaginatedResult<EmailEvent>> QueryEmailEvents(int pageNumber = 0, int pageSize = 15)
    {
        await using var sqlConnection = new SqlConnection(_connectionString);
        await sqlConnection.OpenAsync();

        var sqlCommandText = @"
            SELECT EventDateTime, EventType, EventPayload
            FROM EmailEvents
            ORDER BY EventDateTime DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

            SELECT COUNT(*) FROM EmailEvents; ";

        await using var cmd = new SqlCommand(sqlCommandText, sqlConnection);
        
        cmd.Parameters.Add("@Offset", SqlDbType.Int).Value = pageNumber * pageSize;
        cmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize;

        var list = new List<EmailEvent>();
        var totalCount = 0;

        await using var reader = await cmd.ExecuteReaderAsync();
        
        // Read the paginated results
        while (await reader.ReadAsync())
        {
            list.Add(new EmailEvent(reader.GetDateTime(0), reader.GetString(1), 
                reader.GetString(2)));
        }

        // Move to the second result set (total row count)
        if (await reader.NextResultAsync() && await reader.ReadAsync())
        {
            totalCount = reader.GetInt32(0);
        }

        return new PaginatedResult<EmailEvent>(list, totalCount);
    }

    public Result<int> ReadRowsCountInEmailEvents()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var command = new SqlCommand("SELECT COUNT(*) AS TotalRowsCount FROM EmailEvents;", connection);
        var result = command.ExecuteScalar();
        
        return Result.Success(Convert.ToInt32(result));
    }
}