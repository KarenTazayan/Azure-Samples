using Microsoft.Data.SqlClient;

namespace AcsEmailEventsHandler;

public record EmailEvent(DateTime EventDateTime, string EventType, string EventPayload);

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

    public async IAsyncEnumerable<EmailEvent> GetEvents()
    {
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();

        await using var command = new SqlCommand("SELECT EventDateTime, EventType, EventPayload FROM EmailEvents", connection);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            yield return new EmailEvent(reader.GetDateTime(0), reader.GetString(1), reader.GetString(2));
            await Task.Delay(300);
        }
    }
}