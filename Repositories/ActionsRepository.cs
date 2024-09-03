using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using RoutinesDbService.Models;
using Action = RoutinesDbService.Models.Action;

namespace RoutinesDbService.Repositories;

public class ActionsRepository
{
    private readonly IDbConnection _dbConnection;

    public ActionsRepository(IOptions<ConnectionString> connectionString)
    {
        _dbConnection = new MySqlConnection(connectionString.Value.RoutinesConnection);
    }

    public async Task<bool> Create(Action action)
    {
        try
        {
            _dbConnection?.Open();

            string query = @"INSERT INTO actions (id, routine_id, name, description, follow_up_action_id) VALUES 
                                 (@Id, @RoutineId, @Name, @Description, @FollowUpActionId)
                            ;";

            await _dbConnection.ExecuteAsync(query, action);
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
                                        FROM actions 
                                        WHERE id = '{id}'
                                        ;";

            var entity = await _dbConnection.QueryAsync<Action>(selectQuery, id);

            if (!entity.Any())
                return false;

            string query = $@"DELETE FROM actions = WHERE id = '{id}';";

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

    public async Task<List<Action>> GetAll()
    {
        try
        {
            _dbConnection?.Open();

            string query = @"SELECT
                                id
                                , routine_id
                                , name
                                , description
                                , follow_up_action_id
                                FROM actions
                            ;";
            
            var actions = await _dbConnection.QueryAsync<Action>(query);
            return actions.ToList();
        }
        catch (Exception)
        {
            return new List<Action>();
        }
        finally
        {
            _dbConnection?.Close();
        }
    }

    public async Task<Action?> GetById(string id)
    {
        try
        {
            _dbConnection?.Open();

            string query = $@"SELECT 
                                id
                                , routine_id
                                , name
                                , description
                                , follow_up_action_id
                                FROM actions 
                                WHERE id = '{id}'
                            ;";
            
            var actions = await _dbConnection.QueryAsync<Action>(query, id);
            return actions.FirstOrDefault();
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

    public async Task<bool> Update(Action action)
    {
        try
        {
            _dbConnection?.Open();

            string selectQuery = $@"SELECT 
                                        * 
                                        FROM actions 
                                        WHERE id = '{action.Id}'
                                    ;";

            var entity = await _dbConnection.QueryAsync<User>(selectQuery, action.Id);

            if (!entity.Any())
                return false;

            string updateQuery = @"UPDATE actions 
                                    SET 
                                        routine_id = @RoutineId
                                        , name = @Name
                                        , description = @Description
                                        , follow_up_action_id = @FollowUpActionId
                                    WHERE id = @Id
                                    ;";

            await _dbConnection.ExecuteAsync(updateQuery, action);
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