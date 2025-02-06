namespace AcsEmailEventsHandler;

public class PaginatedResult<T>(List<T> items, int totalCount)
{
    public List<T> Items { get; private set; } = items;

    public int TotalCount { get; private set; } = totalCount;
}