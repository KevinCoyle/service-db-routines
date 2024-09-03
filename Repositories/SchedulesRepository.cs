using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using RoutinesDbService.Models;

namespace RoutinesDbService.Repositories;
public class SchedulesRepository
{
    private readonly IDbConnection _dbConnection;

    public SchedulesRepository(IOptions<ConnectionString> connectionString)
    {
        _dbConnection = new MySqlConnection(connectionString.Value.RoutinesConnection);
    }

    public async Task<bool> Create(Schedule schedule)
    {
        try
        {
            _dbConnection?.Open();

            string query = @"INSERT INTO schedule(id, routine_id, name, description) VALUES 
                                 (
                                  @Id, 
                                  @RoutineId,
                                  @Name, 
                                  @Description
                                 )
                            ;";

            await _dbConnection.ExecuteAsync(query, schedule);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            _dbConnection?.Close();
        }
    }

    public async Task<bool> Delete(string id)
    {
        try
        {
            _dbConnection?.Open();

            string selectQuery = $@"SELECT 
                                        * 
                                        FROM schedule 
                                        WHERE id = '{id}'
                                        ;";

            var entity = await _dbConnection.QueryAsync<Schedule>(selectQuery, id);

            if (!entity.Any())
                return false;

            string query = $@"DELETE FROM schedule WHERE id = '{id}';";

            await _dbConnection.ExecuteAsync(query);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            _dbConnection?.Close();
        }
    }

    public async Task<List<Schedule>> GetAll()
    {
        try
        {
            _dbConnection?.Open();

            string query = @"SELECT
                                id
                                , name
                                , description
                                FROM schedule
                            ;";
            
            var schedules = await _dbConnection.QueryAsync<Schedule>(query);
            return schedules.ToList();
        }
        catch (Exception)
        {
            return new List<Schedule>();
        }
        finally
        {
            _dbConnection?.Close();
        }
    }

    public async Task<Schedule?> GetById(string id)
    {
        try
        {
            _dbConnection?.Open();

            string query = $@"SELECT 
                                id
                                , routine_id
                                , name
                                , description
                                FROM schedule 
                                WHERE id = '{id}'
                            ;";
            
            var schedules = await _dbConnection.QueryAsync<Schedule>(query, id);
            return schedules.FirstOrDefault();
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            _dbConnection?.Close();
        }
    }

    public async Task<bool> Update(Schedule schedule)
    {
        try
        {
            _dbConnection?.Open();

            string selectQuery = $@"SELECT 
                                        * 
                                        FROM schedule 
                                        WHERE id = '{schedule.Id}'
                                    ;";

            var entity = await _dbConnection.QueryAsync<Schedule>(selectQuery, schedule.Id);

            if (!entity.Any())
                return false;

            string updateQuery = @"UPDATE schedule SET 
                                        routine_id = @RoutineId
                                        , name = @Name
                                        , description = @Description
                                        WHERE id = @Id
                                    ;";

            await _dbConnection.ExecuteAsync(updateQuery, schedule);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            _dbConnection?.Close();
        }
    }
}