using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using MySql.Data.MySqlClient;

namespace inmobiliaria2026.Repositories;

public class InquilinoRepository : BaseRepository, IInquilinoRepository
{
    private readonly string[] campos = ["Id", "Nombre", "Apellido", "Dni", "Telefono", "Email"];

    public InquilinoRepository(IConfiguration config) : base(config) { }
    
    public async Task<bool> ActualizarAsync(Inquilino inquilino)
    {
        bool estaModificado = false;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                UPDATE inquilinos 
                SET 
                {nameof(Inquilino.Nombre)} = @{nameof(Inquilino.Nombre)},
                {nameof(Inquilino.Apellido)} = @{nameof(Inquilino.Apellido)},
                {nameof(Inquilino.Dni)} = @{nameof(Inquilino.Dni)},
                {nameof(Inquilino.Telefono)} = @{nameof(Inquilino.Telefono)},
                {nameof(Inquilino.Email)} = @{nameof(Inquilino.Email)}
                WHERE {nameof(Inquilino.Id)} = @{nameof(Inquilino.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Inquilino.Nombre)}", inquilino.Nombre);
                command.Parameters.AddWithValue($"{nameof(Inquilino.Apellido)}", inquilino.Apellido);
                command.Parameters.AddWithValue($"{nameof(Inquilino.Dni)}", inquilino.Dni);
                command.Parameters.AddWithValue($"{nameof(Inquilino.Telefono)}", inquilino.Telefono);
                command.Parameters.AddWithValue($"{nameof(Inquilino.Email)}", inquilino.Email);
                command.Parameters.AddWithValue($"{nameof(Inquilino.Id)}", inquilino.Id);

                connection.Open();

                estaModificado = command.ExecuteNonQuery() > 0;

                connection.Close();
            }
        }

        return estaModificado;
    }

    public async Task<int> ContarInquilinos()
    {
        int cantidadInquilinos = 0;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                SELECT COUNT({nameof(Inquilino.Id)}) AS cantidad 
                FROM inquilinos 
                WHERE {nameof(Inquilino.Activo)} = 1;"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {

                connection.Open();

                cantidadInquilinos = Convert.ToInt32(command.ExecuteScalar());

                connection.Close();
            }
        }

        return cantidadInquilinos;
    }

    public async Task<int> CrearAsync(Inquilino inquilino)
    {
        int id = 0;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                INSERT INTO inquilinos 
                (
                    {nameof(Inquilino.Nombre)}, 
                    {nameof(Inquilino.Apellido)}, 
                    {nameof(Inquilino.Dni)}, 
                    {nameof(Inquilino.Telefono)}, 
                    {nameof(Inquilino.Email)}
                )
                VALUES 
                (
                    @{nameof(Inquilino.Nombre)}, 
                    @{nameof(Inquilino.Apellido)}, 
                    @{nameof(Inquilino.Dni)}, 
                    @{nameof(Inquilino.Telefono)}, 
                    @{nameof(Inquilino.Email)}
                );
                
                SELECT LAST_INSERT_ID();"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Inquilino.Nombre)}", inquilino.Nombre);
                command.Parameters.AddWithValue($"{nameof(Inquilino.Apellido)}", inquilino.Apellido);
                command.Parameters.AddWithValue($"{nameof(Inquilino.Dni)}", inquilino.Dni);
                command.Parameters.AddWithValue($"{nameof(Inquilino.Telefono)}", inquilino.Telefono);
                command.Parameters.AddWithValue($"{nameof(Inquilino.Email)}", inquilino.Email);

                try
                {
                    connection.Open();

                    id = Convert.ToInt32(command.ExecuteScalar());
                    inquilino.Id = id;
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
                UPDATE inquilinos 
                SET {nameof(Inquilino.Activo)} = 0 
                WHERE {nameof(Inquilino.Id)} = @{nameof(Inquilino.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Inquilino.Id)}", id);

                connection.Open();

                estaBorrado = command.ExecuteNonQuery() > 0;

                connection.Close();
            }
        }

        return estaBorrado;
    }

    public async Task<List<Inquilino>> ListarAsync(int limit, int offset)
    {
        if (offset < 0 || limit < 0) return [];

        var inquilinos = new List<Inquilino>();

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                SELECT 
                    {nameof(Inquilino.Id)}, 
                    {nameof(Inquilino.Nombre)}, 
                    {nameof(Inquilino.Apellido)}, 
                    {nameof(Inquilino.Dni)}, 
                    {nameof(Inquilino.Telefono)}, 
                    {nameof(Inquilino.Email)},  
                    {nameof(Inquilino.Activo)}  
                FROM inquilinos 
                WHERE {nameof(Inquilino.Activo)} = 1 
                LIMIT @limit OFFSET @offset;"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"limit", limit);
                command.Parameters.AddWithValue($"offset", offset);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inquilinos.Add(new Inquilino
                        {
                            Id = reader.GetInt32(nameof(Inquilino.Id)),
                            Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                            Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                            Dni = reader.GetString(nameof(Inquilino.Dni)),
                            Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                            Email = reader.GetString(nameof(Inquilino.Email)),
                            Activo = reader.GetBoolean(nameof(Inquilino.Activo))
                        });
                    }
                }
            }
        }

        return inquilinos;
    }

    public async Task<IList<Inquilino>> ListarInquilinos(string? nomApe = null, string? orderBy = null, string? order = "ASC", int? limit = null, int? offset = null)
    {
        var inquilinos = new List<Inquilino>();

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                SELECT 
                    {nameof(Inquilino.Id)}, 
                    {nameof(Inquilino.Nombre)}, 
                    {nameof(Inquilino.Apellido)}, 
                    {nameof(Inquilino.Dni)}, 
                    {nameof(Inquilino.Telefono)}, 
                    {nameof(Inquilino.Email)},  
                    {nameof(Inquilino.Activo)}  
                FROM inquilinos 
                WHERE {nameof(Inquilino.Activo)} = 1"
            ;

            if (!string.IsNullOrWhiteSpace(nomApe))
                sql += $" AND ({nameof(Inquilino.Nombre)} LIKE @nomApe OR {nameof(Inquilino.Apellido)} LIKE @nomApe)";
            if (!string.IsNullOrWhiteSpace(orderBy) && campos.Contains(orderBy, StringComparer.OrdinalIgnoreCase))
                sql += $" ORDER BY @orderBy {order}";
            if (offset.HasValue && limit.HasValue)
                sql += $" LIMIT @limit OFFSET @offset";

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                if (!string.IsNullOrWhiteSpace(nomApe)) 
                    command.Parameters.AddWithValue($"nomApe", $"%{nomApe}%");
                if (!string.IsNullOrWhiteSpace(orderBy) && campos.Contains(orderBy, StringComparer.OrdinalIgnoreCase)) 
                    command.Parameters.AddWithValue($"orderBy", orderBy);
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
                        inquilinos.Add(new Inquilino
                        {
                            Id = reader.GetInt32(nameof(Inquilino.Id)),
                            Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                            Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                            Dni = reader.GetString(nameof(Inquilino.Dni)),
                            Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                            Email = reader.GetString(nameof(Inquilino.Email)),
                            Activo = reader.GetBoolean(nameof(Inquilino.Activo))
                        });
                    }
                }
            }
        }

        return inquilinos;
    }

    public async Task<Inquilino?> ObtenerPorIdAsync(int id)
    {
        if (id < 0) return null;

        Inquilino? inquilino = null;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                SELECT 
                    {nameof(Inquilino.Id)}, 
                    {nameof(Inquilino.Nombre)}, 
                    {nameof(Inquilino.Apellido)}, 
                    {nameof(Inquilino.Dni)}, 
                    {nameof(Inquilino.Telefono)}, 
                    {nameof(Inquilino.Email)}, 
                    {nameof(Inquilino.Activo)} 
                FROM inquilinos 
                WHERE {nameof(Inquilino.Activo)} = 1
                    AND {nameof(Inquilino.Id)} = @{nameof(Inquilino.Id)}"
            ;

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                command.Parameters.AddWithValue($"{nameof(Inquilino.Id)}", id);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        inquilino = new Inquilino
                        {
                            Id = reader.GetInt32(nameof(Inquilino.Id)),
                            Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                            Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                            Dni = reader.GetString(nameof(Inquilino.Dni)),
                            Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                            Email = reader.GetString(nameof(Inquilino.Email)),
                            Activo = reader.GetBoolean(nameof(Inquilino.Activo))
                        };
                    }
                }
            }
        }

        return inquilino;
    }
}