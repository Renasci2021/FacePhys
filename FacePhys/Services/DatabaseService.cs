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

    // FIXME: 数据初始化有问题 相似
    // hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh
    public async Task<int> GetTestUserAsync()
    {
        var user = await GetUserByIdAsync(12);

        if (user != null)
        {
            return user.Id;
        }

        user = new User
        {
            Username = "Test233",
            Age = 25,
            Gender = Gender.Male,
            Height = 169,
            Weight = 60,
        };
        await _database.InsertAsync(user);

        HealthMetric heartRate = new()
        {
            UserId = user.Id,
            Type = MetricType.HeartRate,
            HeartRate = 60
        };
        HealthMetric bloodPressure = new()
        {
            UserId = user.Id,
            Type = MetricType.BloodPressure,
            BloodPressure = new(120, 80)
        };
        HealthMetric bloodOxygen = new()
        {
            UserId = user.Id,
            Type = MetricType.BloodOxygen,
            BloodOxygen = 98
        };
        HealthMetric respiratoryRate = new()
        {
            UserId = user.Id,
            Type = MetricType.RespiratoryRate,
            RespiratoryRate = 12
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
