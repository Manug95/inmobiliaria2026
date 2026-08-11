namespace inmobiliaria2026.Repositories;

public abstract class BaseRepository
{
    protected readonly IConfiguration _configuration;
    protected readonly string connectionString;

    public BaseRepository(IConfiguration config)
    {
        _configuration = config;
        connectionString = config["ConnectionStrings:MySql"]!;
    }
}