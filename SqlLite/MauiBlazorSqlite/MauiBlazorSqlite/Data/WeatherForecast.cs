using SQLite;
namespace MauiBlazorSqlite.Data;
public class WeatherForecast
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF { get; set; }
    [MaxLength(4000)]
    public string Summary { get; set; }
}
