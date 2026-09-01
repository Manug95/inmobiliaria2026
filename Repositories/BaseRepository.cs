namespace inmobiliaria2026.Repositories;

public abstract class BaseRepository
{
    protected readonly IConfiguration _configuration;
    protected readonly string _connectionString;

    public BaseRepository(IConfiguration config)
    {
        _configuration = config;
        _connectionString = config["ConnectionStrings:MySql"]!;
    }
}