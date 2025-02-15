﻿using Microsoft.Data.SqlClient;

namespace AcsEmailEventsHandler.Engine;

public class SqlDatabaseInitializer(string connectionString)
{
    public void Init()
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();
        var checkTableQuery = @"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EmailEvents')
                        BEGIN
                            CREATE TABLE EmailEvents (
                                EventDateTime DATETIME NOT NULL,
                                EventType NVARCHAR(100) NOT NULL,
                                EventPayload JSON NOT NULL
                            )
                        END";

        using var command = new SqlCommand(checkTableQuery, connection);
        command.ExecuteNonQuery();
    }
}