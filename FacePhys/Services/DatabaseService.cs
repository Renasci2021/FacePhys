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

    public async Task<int> GetTestUserAsync()
    {
        var user = await GetUserByIdAsync(10086);

        if (user != null)
        {
            return user.Id;
        }

        user = new User
        {
            Username = "Test233",
            Age = 25,
            Gender = Gender.Male,
            Id = 10086
        };
        await _database.InsertAsync(user);

        HealthMetric heartRate = new HeartRate(66)
        {
            UserId = user.Id,
        };
        HealthMetric bloodPressure = new BloodPressure(120, 80)
        {
            UserId = user.Id,
        };
        HealthMetric bloodOxygen = new BloodOxygen(98)
        {
            UserId = user.Id,
        };
        HealthMetric respiratoryRate = new RespiratoryRate(12)
        {
            UserId = user.Id,
        };

        await SaveHealthMetricAsync(heartRate);
        await SaveHealthMetricAsync(bloodPressure);
        await SaveHealthMetricAsync(bloodOxygen);
        await SaveHealthMetricAsync(respiratoryRate);

        return user.Id;
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

    public async Task<List<HealthMetric>> GetHealthMetricsByUserIdAsync(int userId)
    {
        return await _database.Table<HealthMetric>().Where(hm => hm.UserId == userId).ToListAsync();
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
