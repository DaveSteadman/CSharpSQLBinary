using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace DBTestApp;

public class AsyncBinaryDataManager : IDisposable
{
    private readonly string _connectionString;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private bool _disposed = false;

    public AsyncBinaryDataManager(string dbFilePath)
    {
        _connectionString = $"Data Source={dbFilePath};Version=3;";
        // Ensure table exists on creation. Blocking here is acceptable during startup.
        Task.Run(() => CreateTableIfNeeded()).GetAwaiter().GetResult();
    }

    private async Task CreateTableIfNeeded()
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS BlobTable (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    DataName TEXT UNIQUE,
                    DataBytes BLOB
                )";
            using var command = new SQLiteCommand(createTableQuery, connection);
            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> Add(string dataName, byte[] dataBytes)
    {
        if (dataBytes == null || dataBytes.Length == 0 || string.IsNullOrEmpty(dataName))
            return false;

        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            string insertQuery = "INSERT OR REPLACE INTO BlobTable (DataName, DataBytes) VALUES (@dataName, @dataBytes)";
            using var command = new SQLiteCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@dataName", dataName);
            command.Parameters.AddWithValue("@dataBytes", dataBytes);
            int result = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            return result > 0;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> Delete(string dataName)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            string deleteQuery = "DELETE FROM BlobTable WHERE DataName = @dataName";
            using var command = new SQLiteCommand(deleteQuery, connection);
            command.Parameters.AddWithValue("@dataName", dataName);
            int result = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            return result > 0;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<byte[]> Get(string dataName)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            string selectQuery = "SELECT DataBytes FROM BlobTable WHERE DataName = @dataName";
            using var command = new SQLiteCommand(selectQuery, connection);
            command.Parameters.AddWithValue("@dataName", dataName);
            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            if (await reader.ReadAsync().ConfigureAwait(false))
            {
                return (byte[])reader["DataBytes"];
            }
            return Array.Empty<byte>();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<List<string>> List()
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            var fileList = new List<string>();
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            string selectQuery = "SELECT DataName FROM BlobTable";
            using var command = new SQLiteCommand(selectQuery, connection);
            using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                fileList.Add(reader.GetString(0));
            }
            return fileList;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> DataExists(string dataName)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            using var connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            string selectQuery = "SELECT COUNT(*) FROM BlobTable WHERE DataName = @dataName";
            using var command = new SQLiteCommand(selectQuery, connection);
            command.Parameters.AddWithValue("@dataName", dataName);

            object? result = await command.ExecuteScalarAsync().ConfigureAwait(false);

            long count = result is not null && long.TryParse(result.ToString(), out var parsedCount) ? parsedCount : 0;
            return (count > 0);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _semaphore.Dispose();
            _disposed = true;
        }
    }
}
