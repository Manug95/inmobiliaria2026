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

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                UPDATE inmuebles 
                SET 
                {nameof(Inmueble.IdPropietario)} = @{nameof(Inmueble.IdPropietario)},
                {nameof(Inmueble.IdTipoInmueble)} = @{nameof(Inmueble.IdTipoInmueble)},
                {nameof(Inmueble.Cupo)} = @{nameof(Inmueble.Cupo)},
                {nameof(Inmueble.Calle)} = @{nameof(Inmueble.Calle)}, 
                {nameof(Inmueble.NroCalle)} = @{nameof(Inmueble.NroCalle)}, 
                {nameof(Inmueble.Precio)} = @{nameof(Inmueble.Precio)}, 
                {nameof(Inmueble.Senia)} = @{nameof(Inmueble.Senia)}, 
                {nameof(Inmueble.Disponible)} = @{nameof(Inmueble.Disponible)},
                {nameof(Inmueble.Foto)} = @{nameof(Inmueble.Foto)} 
                WHERE {nameof(Inmueble.Id)} = @{nameof(Inmueble.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Inmueble.IdPropietario)}", inmueble.IdPropietario);
                command.Parameters.AddWithValue($"{nameof(Inmueble.IdTipoInmueble)}", inmueble.IdTipoInmueble);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Cupo)}", inmueble.Cupo);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Calle)}", inmueble.Calle);
                command.Parameters.AddWithValue($"{nameof(Inmueble.NroCalle)}", inmueble.NroCalle);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Precio)}", inmueble.Precio);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Senia)}", inmueble.Senia);
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

    public async Task<int> ContarInmuebles(int? disponible, int? idProp)
    {
        int cantidadInmuebles = 0;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                SELECT COUNT({nameof(Inmueble.Id)}) AS cantidad 
                FROM inmuebles 
                WHERE {nameof(Inmueble.Borrado)} = 0"
            ;

            if (disponible.HasValue && ((int)Disponiblilidad.HABILITADOS == disponible.Value || (int)Disponiblilidad.NO_HABILITADOS == disponible.Value))
                sql += $" AND {nameof(Inmueble.Disponible)} = @disponible";
            if (idProp.HasValue && idProp.Value > 0)
                sql += $" AND {nameof(Inmueble.IdPropietario)} = @idPropietario";

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                if (disponible.HasValue && ((int)Disponiblilidad.HABILITADOS == disponible.Value || (int)Disponiblilidad.NO_HABILITADOS == disponible.Value))
                    command.Parameters.AddWithValue("disponible", disponible.Value);
                if (idProp.HasValue && idProp.Value > 0)
                    command.Parameters.AddWithValue("idPropietario", idProp.Value);

                connection.Open();
                cantidadInmuebles = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }

        return cantidadInmuebles;
    }

    public async Task<long> ContarInmueblesParaAlquilar(string desde, string hasta, int? tipo, int? cupo, decimal? precio)
    {
        long cantidadInmuebles = 0;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                SELECT COUNT(i.{nameof(Inmueble.Id)}) AS cantidad 
                FROM inmuebles AS i
                INNER JOIN tipos_inmueble AS ti 
                    ON i.{nameof(Inmueble.IdTipoInmueble)} = ti.id 
                WHERE i.{nameof(Inmueble.Borrado)} = 0 
                    AND i.{nameof(Inmueble.Disponible)} = 1 
                    AND i.{nameof(Inmueble.Id)} NOT IN (
                        SELECT DISTINCT r.{nameof(Reserva.IdInmueble)} 
                        FROM reservas AS r
                        WHERE {nameof(Reserva.FechaInicio)} BETWEEN @desde AND @hasta 
                            OR {nameof(Reserva.FechaFin)} BETWEEN @desde AND @hasta 
                            OR (@desde >= {nameof(Reserva.FechaInicio)} AND @desde <= {nameof(Reserva.FechaFin)}) 
                            OR (@hasta >= {nameof(Reserva.FechaInicio)} AND @hasta <= {nameof(Reserva.FechaFin)}) 
                    )"
            ;

            if (tipo.HasValue)
                sql += $" AND i.{nameof(Inmueble.IdTipoInmueble)} = @tipo";

            if (cupo.HasValue)
                sql += $" AND i.{nameof(Inmueble.Cupo)} = @cupo";

            if (precio.HasValue)
                sql += $" AND i.{nameof(Inmueble.Precio)} <= @precio";

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                command.Parameters.AddWithValue("desde", desde);
                command.Parameters.AddWithValue("hasta", hasta);

                if (tipo.HasValue) command.Parameters.AddWithValue("tipo", tipo.Value);
                if (cupo.HasValue) command.Parameters.AddWithValue("cupo", cupo.Value);
                if (precio.HasValue) command.Parameters.AddWithValue("precio", precio.Value);

                connection.Open();
                cantidadInmuebles = Convert.ToInt64(command.ExecuteScalar());
                connection.Close();
            }
        }

        return cantidadInmuebles;
    }

    public async Task<int> CrearAsync(Inmueble inmueble)
    {
        int id = 0;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                INSERT INTO inmuebles 
                (
                    {nameof(Inmueble.IdPropietario)}, 
                    {nameof(Inmueble.IdTipoInmueble)}, 
                    {nameof(Inmueble.Cupo)}, 
                    {nameof(Inmueble.Calle)}, 
                    {nameof(Inmueble.NroCalle)}, 
                    {nameof(Inmueble.Latitud)}, 
                    {nameof(Inmueble.Longitud)}, 
                    {nameof(Inmueble.Precio)},
                    {nameof(Inmueble.Senia)},
                    {nameof(Inmueble.Foto)} 
                )
                VALUES 
                (
                    @{nameof(Inmueble.IdPropietario)}, 
                    @{nameof(Inmueble.IdTipoInmueble)}, 
                    @{nameof(Inmueble.Cupo)}, 
                    @{nameof(Inmueble.Calle)}, 
                    @{nameof(Inmueble.NroCalle)}, 
                    @{nameof(Inmueble.Latitud)}, 
                    @{nameof(Inmueble.Longitud)}, 
                    @{nameof(Inmueble.Precio)},
                    @{nameof(Inmueble.Senia)},
                    @{nameof(Inmueble.Foto)} 
                );
                
                SELECT LAST_INSERT_ID();"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Inmueble.IdPropietario)}", inmueble.IdPropietario);
                command.Parameters.AddWithValue($"{nameof(Inmueble.IdTipoInmueble)}", inmueble.IdTipoInmueble);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Cupo)}", inmueble.Cupo);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Calle)}", inmueble.Calle);
                command.Parameters.AddWithValue($"{nameof(Inmueble.NroCalle)}", inmueble.NroCalle);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Latitud)}", inmueble.Latitud);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Longitud)}", inmueble.Longitud);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Precio)}", inmueble.Precio);
                command.Parameters.AddWithValue($"{nameof(Inmueble.Senia)}", inmueble.Senia);
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

        using (var connection = new MySqlConnection(_connectionString))
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

    public async Task<bool> EliminarImagen(string ruta)
    {
        bool estaBorrada = false;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = "DELETE FROM imagenes WHERE ruta = @ruta;";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("ruta", ruta);

                connection.Open();
                estaBorrada = command.ExecuteNonQuery() > 0;
                connection.Close();
            }
        }

        return estaBorrada;
    }

    public async Task<bool> GuardarImagen(int inmuebleId, string ruta)
    {
        bool imagenGuardada = false;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"INSERT INTO imagenes (inmuebleId, ruta) VALUES (@inmuebleId, @ruta);";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("inmuebleId", inmuebleId);
                command.Parameters.AddWithValue("ruta", ruta);

                try
                {
                    connection.Open();
                    imagenGuardada = command.ExecuteNonQuery() > 0;
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

        return imagenGuardada;
    }

    public async Task<List<Inmueble>> ListarAsync(int limit, int offset)
    {
        var diccionarioInmuebles = new Dictionary<int, Inmueble>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                SELECT 
                    i.{nameof(Inmueble.Id)}, 
                    i.{nameof(Inmueble.IdPropietario)}, 
                    i.{nameof(Inmueble.IdTipoInmueble)}, 
                    i.{nameof(Inmueble.Cupo)}, 
                    i.{nameof(Inmueble.Calle)}, 
                    i.{nameof(Inmueble.NroCalle)}, 
                    IFNULL(i.{nameof(Inmueble.Latitud)}, 0) AS latitud, 
                    IFNULL(i.{nameof(Inmueble.Longitud)}, 0) AS longitud, 
                    i.{nameof(Inmueble.Precio)}, 
                    i.{nameof(Inmueble.Senia)}, 
                    i.{nameof(Inmueble.Disponible)}, 
                    i.{nameof(Inmueble.Foto)}, 
                    img.ruta AS imagen, 
                    ti.{nameof(TipoInmueble.Tipo)}, 
                    p.{nameof(Propietario.Nombre)}, 
                    p.{nameof(Propietario.Apellido)}, 
                    p.{nameof(Propietario.Dni)} 
                FROM inmuebles AS i 
                INNER JOIN tipos_inmueble AS ti 
                    ON i.{nameof(Inmueble.IdTipoInmueble)} = ti.id 
                LEFT JOIN imagenes AS img
                    ON img.inmuebleId = i.{nameof(Inmueble.Id)} 
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
                    command.Parameters.AddWithValue($"offset", (offset - 1) * limit);
                }

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int inmuebleId = reader.GetInt32(nameof(Inmueble.Id));
                        if (!diccionarioInmuebles.ContainsKey(inmuebleId))
                        {
                            Inmueble inmueble = new Inmueble
                            {
                                Id = inmuebleId,
                                IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                                IdTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
                                Cupo = reader.GetInt32(nameof(Inmueble.Cupo)),
                                Calle = reader.GetString(nameof(Inmueble.Calle)),
                                NroCalle = reader.GetUInt32(nameof(Inmueble.NroCalle)),
                                Latitud = reader.GetDecimal("latitud"),
                                Longitud = reader.GetDecimal("longitud"),
                                Disponible = reader.GetBoolean(nameof(Inmueble.Disponible)),
                                Foto = reader[nameof(Inmueble.Foto)] == DBNull.Value ? null : reader.GetString(nameof(Inmueble.Foto)),
                                Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                                Senia = reader.GetInt32(nameof(Inmueble.Senia)),
                                Fotos = [],
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
                            };
                            
                            if (reader["imagen"] != DBNull.Value)
                                inmueble.Fotos.Add(reader.GetString("imagen"));
                                
                            diccionarioInmuebles.Add(inmuebleId, inmueble);
                        }
                    }
                }
            }
        }

        return diccionarioInmuebles.Values.ToList();
    }

    public async Task<IList<Inmueble>> ListarInmuebles(int disponible, int? offset, int? limit, string? nomApeProp)
    {
        var inmuebles = new List<Inmueble>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                SELECT 
                    i.{nameof(Inmueble.Id)}, 
                    i.{nameof(Inmueble.IdPropietario)}, 
                    i.{nameof(Inmueble.IdTipoInmueble)}, 
                    i.{nameof(Inmueble.Cupo)}, 
                    i.{nameof(Inmueble.Calle)}, 
                    i.{nameof(Inmueble.NroCalle)}, 
                    IFNULL(i.{nameof(Inmueble.Latitud)}, 0) AS latitud, 
                    IFNULL(i.{nameof(Inmueble.Longitud)}, 0) AS longitud, 
                    i.{nameof(Inmueble.Precio)}, 
                    i.{nameof(Inmueble.Senia)}, 
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

            if ((int)Disponiblilidad.HABILITADOS == disponible || (int)Disponiblilidad.NO_HABILITADOS == disponible)
                sql += $" AND {nameof(Inmueble.Disponible)} = @disponible";

            if (!string.IsNullOrWhiteSpace(nomApeProp))
                sql += $" AND (p.{nameof(Propietario.Nombre)} LIKE @nomApe OR p.{nameof(Propietario.Apellido)} LIKE @nomApe)";

            if (offset.HasValue && limit.HasValue)
                sql += $" LIMIT @limit OFFSET @offset";

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                if ((int)Disponiblilidad.HABILITADOS == disponible || (int)Disponiblilidad.NO_HABILITADOS == disponible)
                    command.Parameters.AddWithValue("disponible", disponible);
                if (!string.IsNullOrWhiteSpace(nomApeProp)) 
                    command.Parameters.AddWithValue($"nomApe", $"{nomApeProp}%");
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
                            Cupo = reader.GetInt32(nameof(Inmueble.Cupo)),
                            Calle = reader.GetString(nameof(Inmueble.Calle)),
                            NroCalle = reader.GetUInt32(nameof(Inmueble.NroCalle)),
                            Latitud = reader.GetDecimal("latitud"),
                            Longitud = reader.GetDecimal("longitud"),
                            Disponible = reader.GetBoolean(nameof(Inmueble.Disponible)),
                            Foto = reader[nameof(Inmueble.Foto)] == DBNull.Value ? null : reader.GetString(nameof(Inmueble.Foto)),
                            Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                            Senia = reader.GetInt32(nameof(Inmueble.Senia)),
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

    public async Task<List<Inmueble>> ListarInmueblesParaAlquilar(string desde, string hasta, int? tipo, int? cupo, decimal? precio, int offset, int limit)
    {
        var inmuebles = new List<Inmueble>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                SELECT 
                    i.*, 
                    ti.{nameof(TipoInmueble.Tipo)} AS tipoInmueble, 
                    p.{nameof(Propietario.Nombre)} AS nombreProp, 
                    p.{nameof(Propietario.Apellido)} AS apellidoProp, 
                    p.{nameof(Propietario.Dni)} AS dniProp 
                FROM inmuebles AS i 
                INNER JOIN tipos_inmueble AS ti 
                    ON i.{nameof(Inmueble.IdTipoInmueble)} = ti.id 
                INNER JOIN propietarios AS p 
                    ON i.{nameof(Inmueble.IdPropietario)} = p.id 
                WHERE i.{nameof(Inmueble.Borrado)} = 0 
                    AND i.{nameof(Inmueble.Disponible)} = 1 
                    AND i.{nameof(Inmueble.Id)} NOT IN (
                        SELECT DISTINCT {nameof(Reserva.IdInmueble)} 
                        FROM reservas 
                        WHERE {nameof(Reserva.FechaInicio)} BETWEEN @desde AND @hasta 
                            OR {nameof(Reserva.FechaFin)} BETWEEN @desde AND @hasta 
                            OR (@desde >= {nameof(Reserva.FechaInicio)} AND @desde <= {nameof(Reserva.FechaFin)}) 
                            OR (@hasta >= {nameof(Reserva.FechaInicio)} AND @hasta <= {nameof(Reserva.FechaFin)}) 
                    )" 
            ;

            if (tipo.HasValue)
                sql += $" AND i.{nameof(Inmueble.IdTipoInmueble)} = @tipo";

            if (cupo.HasValue)
                sql += $" AND i.{nameof(Inmueble.Cupo)} = @cupo";

            if (precio.HasValue)
                sql += $" AND i.{nameof(Inmueble.Precio)} <= @precio";

            if (offset > 0 && limit > 0)
                sql += " LIMIT @limit OFFSET @offset";

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                command.Parameters.AddWithValue("desde", desde);
                command.Parameters.AddWithValue("hasta", hasta);

                if (tipo.HasValue) command.Parameters.AddWithValue("tipo", tipo.Value);
                if (cupo.HasValue) command.Parameters.AddWithValue("cupo", cupo.Value);
                if (precio.HasValue) command.Parameters.AddWithValue("precio", precio.Value);

                if (offset > 0 && limit > 0)
                {
                    command.Parameters.AddWithValue("limit", limit);
                    command.Parameters.AddWithValue("offset", (offset - 1) * limit);
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
                            Cupo = reader.GetInt32(nameof(Inmueble.Cupo)),
                            Calle = reader.GetString(nameof(Inmueble.Calle)),
                            NroCalle = reader.GetUInt32(nameof(Inmueble.NroCalle)),
                            Latitud = reader[nameof(Inmueble.Latitud)] == DBNull.Value ? 0 : reader.GetDecimal(nameof(Inmueble.Latitud)),
                            Longitud = reader[nameof(Inmueble.Longitud)] == DBNull.Value ? 0 : reader.GetDecimal(nameof(Inmueble.Longitud)),
                            Disponible = reader.GetBoolean(nameof(Inmueble.Disponible)),
                            Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                            Senia = reader.GetInt32(nameof(Inmueble.Senia)),
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

    public async Task<IList<Inmueble>> ListarInmueblesPorPropietario(int idProp, int? offset, int? limit)
    {
        var inmuebles = new List<Inmueble>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                SELECT 
                    i.{nameof(Inmueble.Id)}, 
                    i.{nameof(Inmueble.IdPropietario)}, 
                    i.{nameof(Inmueble.IdTipoInmueble)}, 
                    i.{nameof(Inmueble.Cupo)}, 
                    i.{nameof(Inmueble.Calle)}, 
                    i.{nameof(Inmueble.NroCalle)}, 
                    IFNULL(i.{nameof(Inmueble.Latitud)}, 0) AS latitud, 
                    IFNULL(i.{nameof(Inmueble.Longitud)}, 0) AS longitud, 
                    i.{nameof(Inmueble.Precio)}, 
                    i.{nameof(Inmueble.Senia)}, 
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
                            Cupo = reader.GetInt32(nameof(Inmueble.Cupo)),
                            Calle = reader.GetString(nameof(Inmueble.Calle)),
                            NroCalle = reader.GetUInt32(nameof(Inmueble.NroCalle)),
                            Latitud = reader.GetDecimal("latitud"),
                            Longitud = reader.GetDecimal("longitud"),
                            Disponible = reader.GetBoolean(nameof(Inmueble.Disponible)),
                            Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                            Senia = reader.GetInt32(nameof(Inmueble.Senia)),
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

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                SELECT 
                    i.{nameof(Inmueble.Id)}, 
                    i.{nameof(Inmueble.IdPropietario)}, 
                    i.{nameof(Inmueble.IdTipoInmueble)}, 
                    i.{nameof(Inmueble.Cupo)}, 
                    i.{nameof(Inmueble.Calle)}, 
                    i.{nameof(Inmueble.NroCalle)}, 
                    IFNULL(i.{nameof(Inmueble.Latitud)}, 0) AS latitud, 
                    IFNULL(i.{nameof(Inmueble.Longitud)}, 0) AS longitud, 
                    i.{nameof(Inmueble.Precio)}, 
                    i.{nameof(Inmueble.Senia)}, 
                    i.{nameof(Inmueble.Disponible)}, 
                    i.{nameof(Inmueble.Foto)}, 
                    img.ruta AS imagen, 
                    ti.{nameof(TipoInmueble.Tipo)} AS tipoInmueble, 
                    p.{nameof(Propietario.Nombre)} AS nombreDuenio, 
                    p.{nameof(Propietario.Apellido)} AS apellidoDuenio, 
                    p.{nameof(Propietario.Dni)} AS dniDuenio 
                FROM inmuebles AS i 
                INNER JOIN tipos_inmueble AS ti 
                    ON i.{nameof(Inmueble.IdTipoInmueble)} = ti.id 
                LEFT JOIN imagenes AS img
                    ON img.inmuebleId = i.{nameof(Inmueble.Id)} 
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
                    while (reader.Read())
                    {
                        if (inmueble == null)
                        {
                            inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(nameof(Inmueble.Id)),
                                IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                                IdTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
                                Cupo = reader.GetInt32(nameof(Inmueble.Cupo)),
                                Calle = reader.GetString(nameof(Inmueble.Calle)),
                                NroCalle = reader.GetUInt32(nameof(Inmueble.NroCalle)),
                                Latitud = reader.GetDecimal("latitud"),
                                Longitud = reader.GetDecimal("longitud"),
                                Disponible = reader.GetBoolean(nameof(Inmueble.Disponible)),
                                Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                                Senia = reader.GetInt32(nameof(Inmueble.Senia)),
                                Foto = reader[nameof(Inmueble.Foto)] == DBNull.Value ? null : reader.GetString(nameof(Inmueble.Foto)),
                                Fotos = [],
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

                        if (reader["imagen"] != DBNull.Value)
                            inmueble?.Fotos?.Add(reader.GetString("imagen"));
                    }
                }
            }
        }

        return inmueble;
    }
}