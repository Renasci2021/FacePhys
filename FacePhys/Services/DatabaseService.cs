using SQLite;
using FacePhys.Models;

namespace FacePhys.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<User>().Wait();
        _database.CreateTableAsync<HealthMetric>().Wait();
    }

    // User methods
    public async Task<List<User>> GetUsersAsync()
    {
        return await _database.Table<User>().ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _database.Table<User>().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<int> SaveUserAsync(User user)
    {
        if (user.Id != 0)
        {
            return await _database.UpdateAsync(user);
        }
        else
        {
            return await _database.InsertAsync(user);
        }
    }

    public async Task<int> DeleteUserAsync(User user)
    {
        return await _database.DeleteAsync(user);
    }

    // HealthMetric methods
    public async Task<List<HealthMetric>> GetHealthMetricsAsync()
    {
        return await _database.Table<HealthMetric>().ToListAsync();
    }

    public async Task<HealthMetric> GetHealthMetricByIdAsync(int id)
    {
        return await _database.Table<HealthMetric>().FirstOrDefaultAsync(hm => hm.Id == id);
    }

    public async Task<int> SaveHealthMetricAsync(HealthMetric healthMetric)
    {
        if (healthMetric.Id != 0)
        {
            return await _database.UpdateAsync(healthMetric);
        }
        else
        {
            return await _database.InsertAsync(healthMetric);
        }
    }

    public async Task<int> DeleteHealthMetricAsync(HealthMetric healthMetric)
    {
        return await _database.DeleteAsync(healthMetric);
    }
}
