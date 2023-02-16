using SQLite;
namespace MauiBlazorSqlite.Data;
public class WeatherForecastService
{
    string _dbPath;
    public string StatusMessage { get; set; }
    private SQLiteAsyncConnection conn;
    public WeatherForecastService(string dbPath)
    {
        _dbPath = dbPath;
    }
    private async Task InitAsync()
    {
        // Don't Create database if it exists
        if (conn != null)
            return;
        // Create database and WeatherForecast Table
        conn = new SQLiteAsyncConnection(_dbPath);
        await conn.CreateTableAsync<WeatherForecast>();
    }
    public async Task<List<WeatherForecast>> GetForecastAsync()
    {
        await InitAsync();
        return await conn.Table<WeatherForecast>().ToListAsync();
    }
    public async Task<WeatherForecast> CreateForecastAsync(
        WeatherForecast paramWeatherForecast)
    {
        // Insert
        await conn.InsertAsync(paramWeatherForecast);
        // return the object with the
        // auto incremented Id populated
        return paramWeatherForecast;
    }
    public async Task<WeatherForecast> UpdateForecastAsync(
        WeatherForecast paramWeatherForecast)
    {
        // Update
        await conn.UpdateAsync(paramWeatherForecast);
        // Return the updated object
        return paramWeatherForecast;
    }
    public async Task<WeatherForecast> DeleteForecastAsync(
        WeatherForecast paramWeatherForecast)
    {
        // Delete
        await conn.DeleteAsync(paramWeatherForecast);
        return paramWeatherForecast;
    }
}

