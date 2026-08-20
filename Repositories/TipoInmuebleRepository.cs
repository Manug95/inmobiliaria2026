using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using MySql.Data.MySqlClient;

namespace inmobiliaria2026.Repositories;

public class TipoInmuebleRepository : BaseRepository, ITipoInmuebleRepository
{
    public TipoInmuebleRepository(IConfiguration config) : base(config) { }

    public async Task<bool> ActualizarAsync(TipoInmueble tipoInmueble)
    {
        bool estaModificado = false;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                UPDATE tipos_inmueble 
                SET 
                {nameof(TipoInmueble.Tipo)} = @{nameof(TipoInmueble.Tipo)}, 
                {nameof(TipoInmueble.Descripcion)} = @{nameof(TipoInmueble.Descripcion)}
                WHERE {nameof(TipoInmueble.Id)} = @{nameof(TipoInmueble.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(TipoInmueble.Tipo)}", tipoInmueble.Tipo);
                command.Parameters.AddWithValue($"{nameof(TipoInmueble.Descripcion)}", tipoInmueble.Descripcion);
                command.Parameters.AddWithValue($"{nameof(TipoInmueble.Id)}", tipoInmueble.Id);

                connection.Open();

                estaModificado = command.ExecuteNonQuery() > 0;

                connection.Close();
            }
        }

        return estaModificado;
    }

    public async Task<int> ContarTiposInmueble()
    {
        int cantidadTiposInmueble = 0;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                SELECT COUNT({nameof(TipoInmueble.Id)}) AS cantidad 
                FROM tipos_inmueble 
                WHERE activo = 1;"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                cantidadTiposInmueble = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }

        return cantidadTiposInmueble;
    }

    public async Task<int> CrearAsync(TipoInmueble tipoInmueble)
    {
        int id = 0;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                INSERT INTO tipos_inmueble 
                ({nameof(TipoInmueble.Tipo)}, {nameof(TipoInmueble.Descripcion)}) 
                VALUES 
                (@{nameof(TipoInmueble.Tipo)}, @{nameof(TipoInmueble.Descripcion)}); 
                SELECT LAST_INSERT_ID();"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(TipoInmueble.Tipo)}", tipoInmueble.Tipo?.ToUpper());
                command.Parameters.AddWithValue($"{nameof(TipoInmueble.Descripcion)}", tipoInmueble.Descripcion);

                try
                {
                    connection.Open();
                    id = Convert.ToInt32(command.ExecuteScalar());
                    tipoInmueble.Id = id;
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

    public async Task<bool> EliminarAsync(int id)
    {
        bool estaBorrado = false;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                UPDATE tipos_inmueble
                SET activo = 0 
                WHERE {nameof(TipoInmueble.Id)} = @{nameof(TipoInmueble.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(TipoInmueble.Id)}", id);

                connection.Open();
                estaBorrado = command.ExecuteNonQuery() > 0;
                connection.Close();
            }
        }

        return estaBorrado;
    }

    public async Task<List<TipoInmueble>> ListarAsync(int limit, int offset)
    {
        var tiposInmuebles = new List<TipoInmueble>();

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                SELECT 
                    {nameof(TipoInmueble.Id)}, 
                    {nameof(TipoInmueble.Tipo)}, 
                    IFNULL({nameof(TipoInmueble.Descripcion)}, 'Sin Descripción') AS {nameof(TipoInmueble.Descripcion)} 
                FROM tipos_inmueble 
                WHERE activo = 1
                LIMIT @limit OFFSET @offset;"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"limit", limit);
                command.Parameters.AddWithValue($"offset", (offset - 1) * limit);
                
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tiposInmuebles.Add(new TipoInmueble
                        {
                            Id = reader.GetInt32(nameof(TipoInmueble.Id)),
                            Tipo = reader.GetString(nameof(TipoInmueble.Tipo)),
                            Descripcion = reader.GetString(nameof(TipoInmueble.Descripcion))
                        });
                    }
                }
            }
        }

        return tiposInmuebles;
    }

    public async Task<TipoInmueble?> ObtenerPorIdAsync(int id)
    {
        if (id <= 0) return null;
        
        TipoInmueble? tipoInmueble = null;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                SELECT 
                    {nameof(TipoInmueble.Id)}, 
                    {nameof(TipoInmueble.Tipo)}, 
                    IFNULL({nameof(TipoInmueble.Descripcion)}, 'Sin Descripción') AS {nameof(TipoInmueble.Descripcion)}
                FROM tipos_inmueble 
                WHERE {nameof(TipoInmueble.Id)} = {id}"
            ;

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tipoInmueble = new TipoInmueble
                        {
                            Id = reader.GetInt32(nameof(TipoInmueble.Id)),
                            Tipo = reader.GetString(nameof(TipoInmueble.Tipo)),
                            Descripcion = reader.GetString(nameof(TipoInmueble.Descripcion))
                        };
                    }
                }
            }
        }

        return tipoInmueble;
    }
}