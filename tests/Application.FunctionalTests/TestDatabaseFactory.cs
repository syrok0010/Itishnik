namespace Itishnik.Application.FunctionalTests;

public static class TestDatabaseFactory
{
    public static async Task<ITestDatabase> CreateAsync()
    {
        // Testcontainers requires Docker. To use a local PostgreSQL database instead,
        // switch to `PostgreSQLTestDatabase` and update appsettings.json.
        var database = new PostgreSQLTestcontainersTestDatabase();

        await database.InitialiseAsync();

        return database;
    }
}
