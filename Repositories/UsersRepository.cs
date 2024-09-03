using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using RoutinesDbService.Models;

namespace RoutinesDbService.Repositories;
public class UsersRepository
{
    private readonly IDbConnection _dbConnection;

    public UsersRepository(IOptions<ConnectionString> connectionString)
    {
        _dbConnection = new MySqlConnection(connectionString.Value.RoutinesConnection);
    }

    public async Task<bool> Create(User user)
    {
        try
        {
            _dbConnection?.Open();

            string query = @"INSERT INTO user(id, name, nickname, email) VALUES 
                                 (@Id, @Name, @Nickname, @Email)
                            ;";

            await _dbConnection.ExecuteAsync(query, user);
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
                                        FROM user 
                                        WHERE id = '{id}'
                                        ;";

            var entity = await _dbConnection.QueryAsync<User>(selectQuery, id);

            if (!entity.Any())
                return false;

            string query = $@"DELETE FROM user WHERE id = '{id}';";

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

    public async Task<List<User>> GetAll()
    {
        try
        {
            _dbConnection?.Open();

            string query = @"SELECT
                                id
                                , name
                                , nickname
                                , email 
                                FROM user
                            ;";
            
            var users = await _dbConnection.QueryAsync<User>(query);
            return users.ToList();
        }
        catch (Exception)
        {
            return new List<User>();
        }
        finally
        {
            _dbConnection?.Close();
        }
    }

    public async Task<User?> GetById(string id)
    {
        try
        {
            _dbConnection?.Open();

            string query = $@"SELECT 
                                id
                                , name
                                , nickname
                                , email 
                                FROM user 
                                WHERE id = '{id}'
                            ;";
            
            var users = await _dbConnection.QueryAsync<User>(query, id);
            return users.FirstOrDefault();
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

    public async Task<bool> Update(User user)
    {
        try
        {
            _dbConnection?.Open();

            string selectQuery = $@"SELECT 
                                        * 
                                        FROM user 
                                        WHERE id = '{user.Id}'
                                    ;";

            var entity = await _dbConnection.QueryAsync<User>(selectQuery, user.Id);

            if (!entity.Any())
                return false;

            string updateQuery = @"UPDATE user SET 
                                        name = @Name
                                        , nickname = @Nickname
                                        , email = @Email
                                        WHERE id = @Id
                                    ;";

            await _dbConnection.ExecuteAsync(updateQuery, user);
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