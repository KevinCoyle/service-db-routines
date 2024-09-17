using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using RoutinesDbService.Models;

namespace RoutinesDbService.Repositories;
public class RoutinesRepository
{
    private readonly IDbConnection _dbConnection;

    public RoutinesRepository(IOptions<ConnectionString> connectionString)
    {
        _dbConnection = new MySqlConnection(connectionString.Value.RoutinesConnection);
    }

    public async Task<bool> Create(Routine routine)
    {
        try
        {
            _dbConnection.Open();

            string query = @"INSERT INTO routines(id, owner_id, name, description, follow_up_routine_id) VALUES 
                                 (@Id, @OwnerId, @Name, @Description, @FollowUpRoutineId)
                            ;";

            await _dbConnection.ExecuteAsync(query, routine);
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
            _dbConnection.Open();

            string selectQuery = $@"SELECT 
                                        * 
                                        FROM routines 
                                        WHERE id = '{id}'
                                        ;";

            var entity = await _dbConnection.QueryAsync<Routine>(selectQuery, id);

            if (!entity.Any())
                return false;

            string query = $@"DELETE FROM routines WHERE id = '{id}';";

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

    public async Task<List<Routine>> GetAll()
    {
        try
        {
            _dbConnection.Open();

            string query = @"SELECT
                                id
                                , owner_id
                                , name
                                , description
                                , follow_up_routine_id
                                FROM routines
                            ;";
            
            var routines = await _dbConnection.QueryAsync<Routine>(query);
            return routines.ToList();
        }
        catch (Exception)
        {
            return new List<Routine>();
        }
        finally
        {
            _dbConnection?.Close();
        }
    }

    public async Task<Routine?> GetById(string id)
    {
        try
        {
            _dbConnection.Open();

            string query = $@"SELECT 
                                id
                                , owner_id
                                , name
                                , description
                                , follow_up_routine_id
                                FROM routines
                                WHERE id = '{id}'
                            ;";
            
            var routines = await _dbConnection.QueryAsync<Routine>(query, id);
            return routines.FirstOrDefault();
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

    public async Task<bool> Update(Routine routine)
    {
        try
        {
            _dbConnection.Open();

            string selectQuery = $@"SELECT 
                                        * 
                                        FROM routines
                                        WHERE id = '{routine.Id}'
                                    ;";

            var entity = await _dbConnection.QueryAsync<Routine>(selectQuery, routine.Id);

            if (!entity.Any())
                return false;

            string updateQuery = @"UPDATE routines SET 
                                        owner_id = @OwnerId
                                        , name = @Name
                                        , description = @Description
                                        , follow_up_routine_id = @FollowupRoutineId
                                        WHERE id = @Id
                                    ;";

            await _dbConnection.ExecuteAsync(updateQuery, routine);
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