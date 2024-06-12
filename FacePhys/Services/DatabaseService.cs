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
        _database.CreateTableAsync<BloodOxygen>().Wait();
        _database.CreateTableAsync<BloodPressure>().Wait();
        _database.CreateTableAsync<HeartRate>().Wait();
        _database.CreateTableAsync<RespiratoryRate>().Wait();
    }

    public async Task<int> GetTestUserAsync()
    {
        var user = await GetUserByIdAsync(50);

        if (user != null)
        {
            return user.Id;
        }

        user = new User
        {
            Username = "Test233",
            Age = 25,
            Gender = Gender.Male,
            Height = 189,
            Weight = 70,
        };
        await _database.InsertAsync(user);

        HeartRate heartRate = new()
        {
            UserId = user.Id,
            Timestamp = DateTime.Now,
            BeatsPerMinute = (float?)80.0,
        };

        await _database.InsertAsync(heartRate);

        heartRate = new()
        {
            UserId = user.Id,
            Timestamp = DateTime.Now.AddMinutes(-5),
            BeatsPerMinute = (float?)85.01,
        };

        await _database.InsertAsync(heartRate);

        BloodPressure bloodPressure = new()
        {
            UserId = user.Id,
            Timestamp = DateTime.Now,
            Systolic = 120,
            Diastolic = 80,
        };

        await _database.InsertAsync(bloodPressure);

        BloodOxygen bloodOxygen = new()
        {
            UserId = user.Id,
            Timestamp = DateTime.Now,
            OxygenLevel = 98,
        };

        await _database.InsertAsync(bloodOxygen);

        RespiratoryRate respiratoryRate = new()
        {
            UserId = user.Id,
            Timestamp = DateTime.Now,
            BreathsPerMinute = 16,
        };

        await _database.InsertAsync(respiratoryRate);

        return user.Id;
    }

    // User methods
    public async Task<User> InsertAsync(User newUser)
    {
        await _database.InsertAsync(newUser);
        return newUser;
    }
    public async Task<List<User>> GetUsersAsync()
    {
        return await _database.Table<User>().ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _database.Table<User>().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        return await _database.Table<User>().FirstOrDefaultAsync(u => u.Username == username);
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

    public async Task<List<T>> GetHealthMetricsAsync<T>(int userId) where T : HealthMetric, new()
    {
        return await _database.Table<T>().Where(hm => hm.UserId == userId).ToListAsync();
    }

    public async Task<T> GetLatestHealthMetricAsync<T>(int userId) where T : HealthMetric, new()
    {
        return await _database.Table<T>().Where(hm => hm.UserId == userId).OrderByDescending(hm => hm.Timestamp).FirstOrDefaultAsync();
    }

    public async Task<int> SaveHealthMetricAsync<T>(T healthMetric) where T : HealthMetric
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

    public async Task<int> DeleteHealthMetricAsync<T>(T healthMetric) where T : HealthMetric
    {
        return await _database.DeleteAsync(healthMetric);
    }

    public async Task<int> DeleteAllHealthMetricsAsync<T>(int userId) where T : HealthMetric, new()
    {
        return await _database.Table<T>().Where(hm => hm.UserId == userId).DeleteAsync();
    }
}
