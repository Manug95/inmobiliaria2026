using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using MySql.Data.MySqlClient;

namespace inmobiliaria2026.Repositories;

public class ImagenRepository(IConfiguration config) : BaseRepository(config), IImagenRepository
{
    public async Task<bool> ActualizarAsync(Imagen imagen)
    {
        bool estaModificada = false;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                UPDATE imagenes 
                SET 
                {nameof(Imagen.Ruta)} = @{nameof(Imagen.Ruta)}
                WHERE {nameof(Imagen.Id)} = @{nameof(Imagen.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Imagen.Ruta)}", imagen.Ruta);
                command.Parameters.AddWithValue($"{nameof(Imagen.Id)}", imagen.Id);

                connection.Open();
                estaModificada = command.ExecuteNonQuery() > 0;
                connection.Close();
            }
        }

        return estaModificada;
    }

    public async Task<long> CrearAsync(Imagen imagen)
    {
        long id = 0;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                INSERT INTO imagenes 
                ({nameof(Imagen.InmuebleId)}, {nameof(Imagen.Ruta)}) 
                VALUES 
                (@{nameof(Imagen.InmuebleId)}, @{nameof(Imagen.Ruta)}); 
                SELECT LAST_INSERT_ID();"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Imagen.Ruta)}", imagen.Ruta);
                command.Parameters.AddWithValue($"{nameof(Imagen.InmuebleId)}", imagen.InmuebleId);

                try
                {
                    connection.Open();
                    id = Convert.ToInt32(command.ExecuteScalar());
                    imagen.Id = id;
                }
                catch (MySqlException mye)
                {
                    Console.WriteLine(mye.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                connection.Close();
            }
        }

        return id;
    }

    public async Task<bool> EliminarAsync(long id)
    {
        bool estaBorrada = false;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = $"DELETE FROM imagenes WHERE {nameof(Imagen.Id)} = @{nameof(Imagen.Id)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Imagen.Id)}", id);

                connection.Open();
                estaBorrada = command.ExecuteNonQuery() > 0;
                connection.Close();
            }
        }

        return estaBorrada;
    }

    public Task<List<Imagen>> ListarAsync(int limit, int offset)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Imagen>> ListarPorInmuebleAsync(int inmuebleId, int limit, int offset)
    {
        var imagenes = new List<Imagen>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                SELECT 
                    {nameof(Imagen.Id)}, 
                    {nameof(Imagen.Ruta)}, 
                    {nameof(Imagen.InmuebleId)} 
                FROM imagenes 
                WHERE {nameof(Imagen.InmuebleId)} = @inmuebleId
                LIMIT @limit OFFSET @offset;"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"inmuebleId", inmuebleId);
                command.Parameters.AddWithValue($"limit", limit);
                command.Parameters.AddWithValue($"offset", (offset - 1) * limit);
                
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        imagenes.Add(new Imagen
                        {
                            Id = reader.GetInt64(nameof(Imagen.Id)),
                            Ruta = reader.GetString(nameof(Imagen.Ruta)),
                            InmuebleId = reader.GetInt32(nameof(Imagen.InmuebleId))
                        });
                    }
                }
            }
        }

        return imagenes;
    }

    public Task<Imagen?> ObtenerPorIdAsync(long id)
    {
        throw new NotImplementedException();
    }
}