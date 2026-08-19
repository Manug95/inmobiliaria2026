using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using MySql.Data.MySqlClient;

namespace inmobiliaria2026.Repositories;

public class PropietarioRepository : BaseRepository, IPropietarioRepository
{
    private readonly string[] campos = ["Id", "Nombre", "Apellido", "Dni", "Telefono", "Email"];

    public PropietarioRepository(IConfiguration config) : base(config) { }

    public async Task<bool> ActualizarAsync(Propietario propietario)
    {
        bool modificado = false;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                UPDATE propietarios 
                SET 
                {nameof(Propietario.Nombre)} = @{nameof(Propietario.Nombre)},
                {nameof(Propietario.Apellido)} = @{nameof(Propietario.Apellido)},
                {nameof(Propietario.Dni)} = @{nameof(Propietario.Dni)},
                {nameof(Propietario.Telefono)} = @{nameof(Propietario.Telefono)},
                {nameof(Propietario.Email)} = @{nameof(Propietario.Email)}
                WHERE {nameof(Propietario.Id)} = @{nameof(Propietario.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Propietario.Nombre)}", propietario.Nombre);
                command.Parameters.AddWithValue($"{nameof(Propietario.Apellido)}", propietario.Apellido);
                command.Parameters.AddWithValue($"{nameof(Propietario.Dni)}", propietario.Dni);
                command.Parameters.AddWithValue($"{nameof(Propietario.Telefono)}", propietario.Telefono);
                command.Parameters.AddWithValue($"{nameof(Propietario.Email)}", propietario.Email);
                command.Parameters.AddWithValue($"{nameof(Propietario.Id)}", propietario.Id);

                connection.Open();

                modificado = command.ExecuteNonQuery() > 0;

                connection.Close();
            }
        }

        return modificado;
    }

    public async Task<int> ContarPropietarios()
    {
        int cantidadPropietarios = 0;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                SELECT COUNT({nameof(Propietario.Id)}) AS cantidad 
                FROM propietarios 
                WHERE {nameof(Propietario.Activo)} = 1;"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                cantidadPropietarios = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }

        return cantidadPropietarios;
    }

    public async Task<int> CrearAsync(Propietario propietario)
    {
        int id = 0;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                INSERT INTO propietarios 
                (
                    {nameof(Propietario.Nombre)}, 
                    {nameof(Propietario.Apellido)}, 
                    {nameof(Propietario.Dni)}, 
                    {nameof(Propietario.Telefono)}, 
                    {nameof(Propietario.Email)}
                )
                VALUES 
                (
                    @{nameof(Propietario.Nombre)}, 
                    @{nameof(Propietario.Apellido)}, 
                    @{nameof(Propietario.Dni)}, 
                    @{nameof(Propietario.Telefono)}, 
                    @{nameof(Propietario.Email)}
                );
                
                SELECT LAST_INSERT_ID();"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Propietario.Nombre)}", propietario.Nombre);
                command.Parameters.AddWithValue($"{nameof(Propietario.Apellido)}", propietario.Apellido);
                command.Parameters.AddWithValue($"{nameof(Propietario.Dni)}", propietario.Dni);
                command.Parameters.AddWithValue($"{nameof(Propietario.Telefono)}", propietario.Telefono);
                command.Parameters.AddWithValue($"{nameof(Propietario.Email)}", propietario.Email);

                try
                {
                    connection.Open();
                    id = Convert.ToInt32(command.ExecuteScalar());
                    propietario.Id = id;
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
        bool fueBorrado = false;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                UPDATE propietarios 
                SET {nameof(Propietario.Activo)} = 0 
                WHERE {nameof(Propietario.Id)} = @{nameof(Propietario.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Propietario.Id)}", id);
                connection.Open();
                fueBorrado = command.ExecuteNonQuery() > 0;
                connection.Close();
            }
        }

        return fueBorrado;
    }

    public async Task<List<Propietario>> ListarAsync(int limit, int offset)
    {
        if (limit < 0 || offset < 0) return [];
        
        var propietarios = new List<Propietario>();

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                SELECT 
                    {nameof(Propietario.Id)}, 
                    {nameof(Propietario.Nombre)}, 
                    {nameof(Propietario.Apellido)}, 
                    {nameof(Propietario.Dni)}, 
                    {nameof(Propietario.Telefono)}, 
                    {nameof(Propietario.Email)}, 
                    {nameof(Propietario.Activo)}
                FROM propietarios 
                WHERE {nameof(Propietario.Activo)} = 1
                LIMIT @limit OFFSET @offset"
            ;

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                command.Parameters.AddWithValue($"limit", limit);
                command.Parameters.AddWithValue($"offset", offset);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        propietarios.Add(new Propietario
                        {
                            Id = reader.GetInt32(nameof(Propietario.Id)),
                            Nombre = reader.GetString(nameof(Propietario.Nombre)),
                            Apellido = reader.GetString(nameof(Propietario.Apellido)),
                            Dni = reader.GetString(nameof(Propietario.Dni)),
                            Telefono = reader.GetString(nameof(Propietario.Telefono)),
                            Email = reader.GetString(nameof(Propietario.Email)),
                            Activo = reader.GetBoolean(nameof(Propietario.Activo))
                        });
                    }
                }
            }
        }

        return propietarios;
    }

    public async Task<IList<Propietario>> ListarPropietarios(string? nomApe = null, string? orderBy = null, string? order = "ASC", int? offset = null, int? limit = null)
    {
        var propietarios = new List<Propietario>();

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                SELECT 
                    {nameof(Propietario.Id)}, 
                    {nameof(Propietario.Nombre)}, 
                    {nameof(Propietario.Apellido)}, 
                    {nameof(Propietario.Dni)}, 
                    {nameof(Propietario.Telefono)}, 
                    {nameof(Propietario.Email)}, 
                    {nameof(Propietario.Activo)}
                FROM propietarios 
                WHERE {nameof(Propietario.Activo)} = 1"
            ;

            if (!string.IsNullOrWhiteSpace(nomApe))
                sql += $" AND ({nameof(Propietario.Nombre)} LIKE @nomApe OR {nameof(Propietario.Apellido)} LIKE @nomApe)";
            if (!string.IsNullOrWhiteSpace(orderBy) && campos.Contains(orderBy, StringComparer.OrdinalIgnoreCase))
                sql += $" ORDER BY @orderBy {order}";
            if (offset.HasValue && limit.HasValue)
                sql += $" LIMIT @limit OFFSET @offset";

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                if (!string.IsNullOrWhiteSpace(nomApe)) command.Parameters.AddWithValue("nomApe", $"{nomApe}%");
                if (!string.IsNullOrWhiteSpace(orderBy) && campos.Contains(orderBy, StringComparer.OrdinalIgnoreCase)) command.Parameters.AddWithValue("orderBy", orderBy);
                if (offset.HasValue && limit.HasValue)
                {
                    command.Parameters.AddWithValue($"limit", limit.Value);
                    command.Parameters.AddWithValue($"offset", offset.Value);
                }

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        propietarios.Add(new Propietario
                        {
                            Id = reader.GetInt32(nameof(Propietario.Id)),
                            Nombre = reader.GetString(nameof(Propietario.Nombre)),
                            Apellido = reader.GetString(nameof(Propietario.Apellido)),
                            Dni = reader.GetString(nameof(Propietario.Dni)),
                            Telefono = reader.GetString(nameof(Propietario.Telefono)),
                            Email = reader.GetString(nameof(Propietario.Email)),
                            Activo = reader.GetBoolean(nameof(Propietario.Activo))
                        });
                    }
                }
            }
        }

        return propietarios;
    }

    public async Task<Propietario?> ObtenerPorIdAsync(int id)
    {
        if (id < 0) return null;

        Propietario? propietario = null;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                SELECT 
                    {nameof(Propietario.Id)}, 
                    {nameof(Propietario.Nombre)}, 
                    {nameof(Propietario.Apellido)}, 
                    {nameof(Propietario.Dni)}, 
                    {nameof(Propietario.Telefono)}, 
                    {nameof(Propietario.Email)}, 
                    {nameof(Propietario.Activo)}
                FROM propietarios 
                WHERE {nameof(Propietario.Activo)} = 1
                    AND {nameof(Propietario.Id)} = @{nameof(Propietario.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Propietario.Id)}", id);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        propietario = new Propietario
                        {
                            Id = reader.GetInt32(nameof(Propietario.Id)),
                            Nombre = reader.GetString(nameof(Propietario.Nombre)),
                            Apellido = reader.GetString(nameof(Propietario.Apellido)),
                            Dni = reader.GetString(nameof(Propietario.Dni)),
                            Telefono = reader.GetString(nameof(Propietario.Telefono)),
                            Email = reader.GetString(nameof(Propietario.Email)),
                            Activo = reader.GetBoolean(nameof(Propietario.Activo))
                        };
                    }
                }
            }
        }

        return propietario;
    }
}