using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using MySql.Data.MySqlClient;

namespace inmobiliaria2026.Repositories;

public class InmuebleRepository : BaseRepository, IInmuebleRepository
{
    public InmuebleRepository(IConfiguration config) : base(config) { }

    public async Task<bool> ActualizarAsync(Inmueble inmueble)
    {
        bool estaModificado = false;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                UPDATE inmuebles 
                SET 
                {nameof(Inmueble.IdPropietario)} = @{nameof(Inmueble.IdPropietario)},
                {nameof(Inmueble.IdTipoInmueble)} = @{nameof(Inmueble.IdTipoInmueble)},
                {nameof(Inmueble.CantidadAmbientes)} = @{nameof(Inmueble.CantidadAmbientes)},
                {nameof(Inmueble.Calle)} = @{nameof(Inmueble.Calle)}, 
                {nameof(Inmueble.NroCalle)} = @{nameof(Inmueble.NroCalle)}, 
                {nameof(Inmueble.Precio)} = @{nameof(Inmueble.Precio)}, 
                {nameof(Inmueble.Disponible)} = @{nameof(Inmueble.Disponible)},
                {nameof(Inmueble.Foto)} = @{nameof(Inmueble.Foto)} 
                WHERE {nameof(Inmueble.Id)} = @{nameof(Inmueble.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Inmueble.IdPropietario)}", inmueble.IdPropietario);
                command.Parameters.AddWithValue($"{nameof(Inmueble.IdTipoInmueble)}", inmueble.IdTipoInmueble);
                command.Parameters.AddWithValue($"{nameof(Inmueble.CantidadAmbientes)}", inmueble.CantidadAmbientes);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Calle)}", inmueble.Calle);
                command.Parameters.AddWithValue($"{nameof(Inmueble.NroCalle)}", inmueble.NroCalle);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Precio)}", inmueble.Precio);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Disponible)}", inmueble.Disponible);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Foto)}", inmueble.Foto);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Id)}", inmueble.Id);

                connection.Open();
                estaModificado = command.ExecuteNonQuery() > 0;
                connection.Close();
            }
        }

        return estaModificado;
    }

    public async Task<int> ContarInmuebles(int? disponible)
    {
        int cantidadInmuebles = 0;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                SELECT COUNT({nameof(Inmueble.Id)}) AS cantidad 
                FROM inmuebles 
                WHERE {nameof(Inmueble.Borrado)} = 0"
            ;

            if (disponible.HasValue && disponible.Value >= 0 && disponible.Value < 2)
                sql += $" AND {nameof(Inmueble.Disponible)} = {disponible}";

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                connection.Open();
                cantidadInmuebles = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }

        return cantidadInmuebles;
    }

    public async Task<int> CrearAsync(Inmueble inmueble)
    {
        int id = 0;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                INSERT INTO inmuebles 
                (
                    {nameof(Inmueble.IdPropietario)}, 
                    {nameof(Inmueble.IdTipoInmueble)}, 
                    {nameof(Inmueble.CantidadAmbientes)}, 
                    {nameof(Inmueble.Calle)}, 
                    {nameof(Inmueble.NroCalle)}, 
                    {nameof(Inmueble.Latitud)}, 
                    {nameof(Inmueble.Longitud)}, 
                    {nameof(Inmueble.Precio)},
                    {nameof(Inmueble.Foto)} 
                )
                VALUES 
                (
                    @{nameof(Inmueble.IdPropietario)}, 
                    @{nameof(Inmueble.IdTipoInmueble)}, 
                    @{nameof(Inmueble.CantidadAmbientes)}, 
                    @{nameof(Inmueble.Calle)}, 
                    @{nameof(Inmueble.NroCalle)}, 
                    @{nameof(Inmueble.Latitud)}, 
                    @{nameof(Inmueble.Longitud)}, 
                    @{nameof(Inmueble.Precio)},
                    @{nameof(Inmueble.Foto)} 
                );
                
                SELECT LAST_INSERT_ID();"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Inmueble.IdPropietario)}", inmueble.IdPropietario);
                command.Parameters.AddWithValue($"{nameof(Inmueble.IdTipoInmueble)}", inmueble.IdTipoInmueble);
                command.Parameters.AddWithValue($"{nameof(Inmueble.CantidadAmbientes)}", inmueble.CantidadAmbientes);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Calle)}", inmueble.Calle);
                command.Parameters.AddWithValue($"{nameof(Inmueble.NroCalle)}", inmueble.NroCalle);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Latitud)}", inmueble.Latitud);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Longitud)}", inmueble.Longitud);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Precio)}", inmueble.Precio);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Foto)}", inmueble.Foto);

                try
                {
                    connection.Open();

                    id = Convert.ToInt32(command.ExecuteScalar());
                    inmueble.Id = id;
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
        bool estaEliminado = false;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                UPDATE inmuebles 
                SET {nameof(Inmueble.Borrado)} = 1, {nameof(Inmueble.Disponible)} = 0 
                WHERE {nameof(Inmueble.Id)} = @{nameof(Inmueble.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Inmueble.Id)}", id);

                connection.Open();
                estaEliminado = command.ExecuteNonQuery() > 0;
                connection.Close();
            }
        }

        return estaEliminado;
    }

    public async Task<List<Inmueble>> ListarAsync(int limit, int offset)
    {
        var inmuebles = new List<Inmueble>();

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                SELECT 
                    i.{nameof(Inmueble.Id)}, 
                    i.{nameof(Inmueble.IdPropietario)}, 
                    i.{nameof(Inmueble.IdTipoInmueble)}, 
                    i.{nameof(Inmueble.CantidadAmbientes)}, 
                    i.{nameof(Inmueble.Calle)}, 
                    i.{nameof(Inmueble.NroCalle)}, 
                    IFNULL(i.{nameof(Inmueble.Latitud)}, 0) AS latitud, 
                    IFNULL(i.{nameof(Inmueble.Longitud)}, 0) AS longitud, 
                    i.{nameof(Inmueble.Precio)}, 
                    i.{nameof(Inmueble.Disponible)}, 
                    i.{nameof(Inmueble.Foto)}, 
                    ti.{nameof(TipoInmueble.Tipo)}, 
                    p.{nameof(Propietario.Nombre)}, 
                    p.{nameof(Propietario.Apellido)}, 
                    p.{nameof(Propietario.Dni)} 
                FROM inmuebles AS i 
                INNER JOIN tipos_inmueble AS ti 
                    ON i.{nameof(Inmueble.IdTipoInmueble)} = ti.id 
                INNER JOIN propietarios AS p 
                    ON i.{nameof(Inmueble.IdPropietario)} = p.id 
                WHERE {nameof(Inmueble.Borrado)} = 0"
            ;

            if (offset > 0 && limit > 0)
                sql += $" LIMIT @limit OFFSET @offset";

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                if (offset > 0 && limit > 0)
                {
                    command.Parameters.AddWithValue($"limit", limit);
                    command.Parameters.AddWithValue($"offset", offset);
                }

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inmuebles.Add(new Inmueble
                        {
                            Id = reader.GetInt32(nameof(Inmueble.Id)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            IdTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
                            CantidadAmbientes = reader.GetInt32(nameof(Inmueble.CantidadAmbientes)),
                            Calle = reader.GetString(nameof(Inmueble.Calle)),
                            NroCalle = reader.GetUInt32(nameof(Inmueble.NroCalle)),
                            Latitud = reader.GetDecimal("latitud"),
                            Longitud = reader.GetDecimal("longitud"),
                            Disponible = reader.GetBoolean(nameof(Inmueble.Disponible)),
                            Foto = reader[nameof(Inmueble.Foto)] == DBNull.Value ? null : reader.GetString(nameof(Inmueble.Foto)),
                            Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                            Duenio = new Propietario
                            {
                                Id = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                                Nombre = reader.GetString(nameof(Propietario.Nombre)),
                                Apellido = reader.GetString(nameof(Propietario.Apellido)),
                                Dni = reader.GetString(nameof(Propietario.Dni))
                            },
                            Tipo = new TipoInmueble
                            {
                                Id = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
                                Tipo = reader.GetString(nameof(TipoInmueble.Tipo))
                            }
                        });
                    }
                }
            }
        }

        return inmuebles;
    }

    public async Task<IList<Inmueble>> ListarInmuebles(int disponible, int? offset = null, int? limit = null, string? nomApeProp = null)
    {
        var inmuebles = new List<Inmueble>();

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                SELECT 
                    i.{nameof(Inmueble.Id)}, 
                    i.{nameof(Inmueble.IdPropietario)}, 
                    i.{nameof(Inmueble.IdTipoInmueble)}, 
                    i.{nameof(Inmueble.CantidadAmbientes)}, 
                    i.{nameof(Inmueble.Calle)}, 
                    i.{nameof(Inmueble.NroCalle)}, 
                    IFNULL(i.{nameof(Inmueble.Latitud)}, 0) AS latitud, 
                    IFNULL(i.{nameof(Inmueble.Longitud)}, 0) AS longitud, 
                    i.{nameof(Inmueble.Precio)}, 
                    i.{nameof(Inmueble.Disponible)}, 
                    i.{nameof(Inmueble.Foto)}, 
                    ti.{nameof(TipoInmueble.Tipo)}, 
                    p.{nameof(Propietario.Nombre)}, 
                    p.{nameof(Propietario.Apellido)}, 
                    p.{nameof(Propietario.Dni)} 
                FROM inmuebles AS i 
                INNER JOIN tipos_inmueble AS ti 
                    ON i.{nameof(Inmueble.IdTipoInmueble)} = ti.id 
                INNER JOIN propietarios AS p 
                    ON i.{nameof(Inmueble.IdPropietario)} = p.id 
                WHERE {nameof(Inmueble.Borrado)} = 0"
            ;

            if (disponible >= 0 && disponible < 2)
                sql += $" AND {nameof(Inmueble.Disponible)} = {disponible}";

            if (!string.IsNullOrWhiteSpace(nomApeProp))
                sql += $" AND (p.{nameof(Propietario.Nombre)} LIKE @nomApe OR p.{nameof(Propietario.Apellido)} LIKE @nomApe)";

            if (offset.HasValue && limit.HasValue)
                sql += $" LIMIT @limit OFFSET @offset";

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                if (!string.IsNullOrWhiteSpace(nomApeProp)) command.Parameters.AddWithValue($"nomApe", $"%{nomApeProp}%");
                if (offset.HasValue && limit.HasValue)
                {
                    command.Parameters.AddWithValue($"limit", limit.Value);
                    command.Parameters.AddWithValue($"offset", (offset.Value - 1) * limit.Value);
                }

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inmuebles.Add(new Inmueble
                        {
                            Id = reader.GetInt32(nameof(Inmueble.Id)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            IdTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
                            CantidadAmbientes = reader.GetInt32(nameof(Inmueble.CantidadAmbientes)),
                            Calle = reader.GetString(nameof(Inmueble.Calle)),
                            NroCalle = reader.GetUInt32(nameof(Inmueble.NroCalle)),
                            Latitud = reader.GetDecimal("latitud"),
                            Longitud = reader.GetDecimal("longitud"),
                            Disponible = reader.GetBoolean(nameof(Inmueble.Disponible)),
                            Foto = reader[nameof(Inmueble.Foto)] == DBNull.Value ? null : reader.GetString(nameof(Inmueble.Foto)),
                            Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                            Duenio = new Propietario
                            {
                                Id = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                                Nombre = reader.GetString(nameof(Propietario.Nombre)),
                                Apellido = reader.GetString(nameof(Propietario.Apellido)),
                                Dni = reader.GetString(nameof(Propietario.Dni))
                            },
                            Tipo = new TipoInmueble
                            {
                                Id = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
                                Tipo = reader.GetString(nameof(TipoInmueble.Tipo))
                            }
                        });
                    }
                }
            }
        }

        return inmuebles;
    }

    public Task<IList<Inmueble>> ListarInmueblesParaAlquilar(string desde, string hasta, string? uso, int? tipo, int? cantAmb, decimal? precio, int offset, int limit)
    {
        throw new NotImplementedException();
    }

    public async Task<IList<Inmueble>> ListarInmueblesPorPropietario(int idProp, int? offset, int? limit)
    {
        var inmuebles = new List<Inmueble>();

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                SELECT 
                    i.{nameof(Inmueble.Id)}, 
                    i.{nameof(Inmueble.IdPropietario)}, 
                    i.{nameof(Inmueble.IdTipoInmueble)}, 
                    i.{nameof(Inmueble.CantidadAmbientes)}, 
                    i.{nameof(Inmueble.Calle)}, 
                    i.{nameof(Inmueble.NroCalle)}, 
                    IFNULL(i.{nameof(Inmueble.Latitud)}, 0) AS latitud, 
                    IFNULL(i.{nameof(Inmueble.Longitud)}, 0) AS longitud, 
                    i.{nameof(Inmueble.Precio)}, 
                    i.{nameof(Inmueble.Disponible)}, 
                    i.{nameof(Inmueble.Foto)}, 
                    ti.{nameof(TipoInmueble.Tipo)} AS tipoInmueble, 
                    p.{nameof(Propietario.Nombre)} AS nombreProp, 
                    p.{nameof(Propietario.Apellido)} AS apellidoProp, 
                    p.{nameof(Propietario.Dni)} AS dniProp 
                FROM inmuebles AS i 
                INNER JOIN tipos_inmueble AS ti 
                    ON i.{nameof(Inmueble.IdTipoInmueble)} = ti.id 
                INNER JOIN propietarios AS p 
                    ON i.{nameof(Inmueble.IdPropietario)} = p.id 
                WHERE i.{nameof(Inmueble.Borrado)} = 0 AND i.{nameof(Inmueble.IdPropietario)} = @{nameof(Inmueble.IdPropietario)}"
            ;

            if (offset.HasValue && limit.HasValue)
                sql += $" LIMIT @limit OFFSET @offset";

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                command.Parameters.AddWithValue($"{nameof(Inmueble.IdPropietario)}", idProp);

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
                        inmuebles.Add(new Inmueble
                        {
                            Id = reader.GetInt32(nameof(Inmueble.Id)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            IdTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
                            CantidadAmbientes = reader.GetInt32(nameof(Inmueble.CantidadAmbientes)),
                            Calle = reader.GetString(nameof(Inmueble.Calle)),
                            NroCalle = reader.GetUInt32(nameof(Inmueble.NroCalle)),
                            Latitud = reader.GetDecimal("latitud"),
                            Longitud = reader.GetDecimal("longitud"),
                            Disponible = reader.GetBoolean(nameof(Inmueble.Disponible)),
                            Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                            Foto = reader[nameof(Inmueble.Foto)] == DBNull.Value ? null : reader.GetString(nameof(Inmueble.Foto)),
                            Duenio = new Propietario
                            {
                                Id = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                                Nombre = reader.GetString("nombreProp"),
                                Apellido = reader.GetString("apellidoProp"),
                                Dni = reader.GetString("dniProp")
                            },
                            Tipo = new TipoInmueble
                            {
                                Id = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
                                Tipo = reader.GetString("tipoInmueble")
                            }
                        });
                    }
                }
            }
        }

        return inmuebles;
    }

    public async Task<Inmueble?> ObtenerPorIdAsync(int id)
    {
        Inmueble? inmueble = null;

        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                SELECT 
                    i.{nameof(Inmueble.Id)}, 
                    i.{nameof(Inmueble.IdPropietario)}, 
                    i.{nameof(Inmueble.IdTipoInmueble)}, 
                    i.{nameof(Inmueble.CantidadAmbientes)}, 
                    i.{nameof(Inmueble.Calle)}, 
                    i.{nameof(Inmueble.NroCalle)}, 
                    IFNULL(i.{nameof(Inmueble.Latitud)}, 0) AS latitud, 
                    IFNULL(i.{nameof(Inmueble.Longitud)}, 0) AS longitud, 
                    i.{nameof(Inmueble.Precio)}, 
                    i.{nameof(Inmueble.Disponible)}, 
                    i.{nameof(Inmueble.Foto)}, 
                    ti.{nameof(TipoInmueble.Tipo)} AS tipoInmueble, 
                    p.{nameof(Propietario.Nombre)} AS nombreDuenio, 
                    p.{nameof(Propietario.Apellido)} AS apellidoDuenio, 
                    p.{nameof(Propietario.Dni)} AS dniDuenio 
                FROM inmuebles AS i 
                INNER JOIN tipos_inmueble AS ti 
                    ON i.{nameof(Inmueble.IdTipoInmueble)} = ti.id 
                INNER JOIN propietarios AS p 
                    ON i.{nameof(Inmueble.IdPropietario)} = p.id 
                WHERE i.{nameof(Inmueble.Id)} = @{nameof(Inmueble.Id)} AND {nameof(Inmueble.Borrado)} = 0;"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Inmueble.Id)}", id);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        inmueble = new Inmueble
                        {
                            Id = reader.GetInt32(nameof(Inmueble.Id)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            IdTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
                            CantidadAmbientes = reader.GetInt32(nameof(Inmueble.CantidadAmbientes)),
                            Calle = reader.GetString(nameof(Inmueble.Calle)),
                            NroCalle = reader.GetUInt32(nameof(Inmueble.NroCalle)),
                            Latitud = reader.GetDecimal("latitud"),
                            Longitud = reader.GetDecimal("longitud"),
                            Disponible = reader.GetBoolean(nameof(Inmueble.Disponible)),
                            Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                            Foto = reader[nameof(Inmueble.Foto)] == DBNull.Value ? null : reader.GetString(nameof(Inmueble.Foto)),
                            Duenio = new Propietario
                            {
                                Id = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                                Nombre = reader.GetString("nombreDuenio"),
                                Apellido = reader.GetString("apellidoDuenio"),
                                Dni = reader.GetString("dniDuenio")
                            },
                            Tipo = new TipoInmueble
                            {
                                Id = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
                                Tipo = reader.GetString("tipoInmueble")
                            }
                        };
                    }
                }
            }
        }

        return inmueble;
    }
}