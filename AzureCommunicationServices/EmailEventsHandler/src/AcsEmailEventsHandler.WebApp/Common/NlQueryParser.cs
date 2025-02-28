using CSharpFunctionalExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Liquid;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AcsEmailEventsHandler.WebApp.Common;

public class NlQueryParser(Kernel kernel, ILogger<EmailEventsRepository> logger)
{
    public async Task<string> ParseNlToSql(string nlQuery)
    {
        var result = await ParseAsync(nlQuery);

        return result;
    }

    public async Task<Result<DynamicSqlQuery>> ParseNlToDynamicSqlQuery(string nlQuery)
    {
        var result = await ParseAsync(nlQuery);

        var query = DeserializeAiResult(result);

        if (query.IsSuccess)
        {
            return Result.Success(new DynamicSqlQuery(query.Value.Where, query.Value.OrderBy, new List<SqlParameter>()));
        }


        return Result.Failure<DynamicSqlQuery>(query.Error);
    }

    private async Task<string> ParseAsync(string userInput)
    {
        var template =
            """
            <message role="system">
                You are an AI agent for returning the part of Azure SQL query. The Azure SQL table have the following format:
                
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EmailEvents')
                BEGIN
                CREATE TABLE EmailEvents (
                    EventDateTime DATETIME2 NOT NULL,
                    EventType NVARCHAR(100) NOT NULL,
                    EventPayload JSON NOT NULL
                )
                
                CREATE INDEX IDX_EmailEvents_EventDateTime
                ON dbo.EmailEvents (EventDateTime DESC)
                END"
                
                The program creates the following ADO.NET SQL query:
                
                var sqlCommandText = $@"
                SELECT EventDateTime, EventType, EventPayload
                FROM EmailEvents
                {dynamicSqlQuery.WhereClause}
                {dynamicSqlQuery.OrderByClause}
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                
                You should use JSON_VALUE built-in function.
                You must return dynamicSqlQuery.OrderByClause and dynamicSqlQuery.WhereClause in the following valid JSON format:
                
                {
                 orderBy: "your data",
                 where: "your data"
                }
                           
                # Safety              
                - You must only return the part of the SQL query that is safe to execute.              
                          
                # User Input              
                {{userInput}}
            </message>
            """;

        // Input data for the prompt rendering and execution
        var arguments = new KernelArguments()
        {
            { "userInput", userInput}
        };

        // Create the prompt template using liquid format
        var templateFactory = new LiquidPromptTemplateFactory();
        var promptTemplateConfig = new PromptTemplateConfig()
        {
            Template = template,
            TemplateFormat = "liquid",
            Name = "ParseNlQueryToSqlPrompt",
        };

        // Render the prompt
        var promptTemplate = templateFactory.Create(promptTemplateConfig);
        var renderedPrompt = await promptTemplate.RenderAsync(kernel, arguments);

        // Invoke the prompt function
        var function = kernel.CreateFunctionFromPrompt(promptTemplateConfig, templateFactory);
        var response = await kernel.InvokeAsync(function, arguments);

        return response.ToString();
    }

    private Result<QueryParameters> DeserializeAiResult(string aiResponse)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        try
        {
            string pattern = @"```json\s*\n(?<jsonContent>[\s\S]*?)\n```";
            var match = Regex.Match(aiResponse, pattern, RegexOptions.Multiline);

            if (!match.Success || string.IsNullOrWhiteSpace(match.Groups["jsonContent"].Value.Trim()))
            {
                return Result.Failure<QueryParameters>($"Invalid response from AI: {aiResponse}");
            }

            var json = match.Groups["jsonContent"].Value.Trim();
            var queryParameters = JsonSerializer.Deserialize<QueryParameters>(json, options);

            if (queryParameters != null)
            {
                return Result.Success(queryParameters);
            }
        }
        catch (JsonException e)
        {
            logger.LogError(e, e.Message);
            return Result.Failure<QueryParameters>($"Invalid response from AI: {aiResponse}");
        }

        return Result.Failure<QueryParameters>("Invalid response from AI.");
    }

    public class QueryParameters
    {
        public string? OrderBy { get; set; }
        public string? Where { get; set; }
    }
}