using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using MySql.Data.MySqlClient;

namespace inmobiliaria2026.Repositories;

public class ReservaRepository : BaseRepository, IReservaRepository
{
    public ReservaRepository(IConfiguration config) : base(config) { }

    public async Task<bool> ActualizarAsync(Reserva reserva)
    {
        bool modificada = false;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                UPDATE reservas 
                SET 
                {nameof(Reserva.IdInmueble)} = @{nameof(Reserva.IdInmueble)}, 
                {nameof(Reserva.IdInquilino)} = @{nameof(Reserva.IdInquilino)}, 
                {nameof(Reserva.Monto)} = @{nameof(Reserva.Monto)}, 
                {nameof(Reserva.FechaInicio)} = @{nameof(Reserva.FechaInicio)}, 
                {nameof(Reserva.FechaFin)} = @{nameof(Reserva.FechaFin)}, 
                {nameof(Reserva.FechaTerminado)} = @{nameof(Reserva.FechaTerminado)} 
                WHERE {nameof(Reserva.Id)} = @{nameof(Reserva.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Reserva.IdInmueble)}", reserva.IdInmueble);
                command.Parameters.AddWithValue($"{nameof(Reserva.IdInquilino)}", reserva.IdInquilino);
                command.Parameters.AddWithValue($"{nameof(Reserva.Monto)}", reserva.Monto);
                command.Parameters.AddWithValue($"{nameof(Reserva.FechaInicio)}", reserva.FechaInicio);
                command.Parameters.AddWithValue($"{nameof(Reserva.FechaFin)}", reserva.FechaFin);
                command.Parameters.AddWithValue($"{nameof(Reserva.FechaTerminado)}", reserva.FechaTerminado);
                command.Parameters.AddWithValue($"{nameof(Reserva.Id)}", reserva.Id);

                connection.Open();
                modificada = command.ExecuteNonQuery() > 0;
                connection.Close();
            }
        }

        return modificada;
    }

    public async Task<int> ContarReservas(int? idInm, string? desde, string? hasta)
    {
        int cantidadReservas = 0;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                SELECT COUNT({nameof(Reserva.Id)}) AS cantidad 
                FROM reservas 
                WHERE {nameof(Reserva.Borrado)} = 0"
            ;

            if (idInm.HasValue)
                sql += $" AND {nameof(Reserva.IdInmueble)} = @idInm";
            
            if (!string.IsNullOrWhiteSpace(desde) && !string.IsNullOrWhiteSpace(hasta))
                sql += @$" AND ({nameof(Reserva.FechaInicio)} BETWEEN @desde AND @hasta 
                            OR {nameof(Reserva.FechaFin)} BETWEEN @desde AND @hasta
                            OR (@desde >= {nameof(Reserva.FechaInicio)} AND @desde <= {nameof(Reserva.FechaFin)})
                            OR (@hasta >= {nameof(Reserva.FechaInicio)} AND @hasta <= {nameof(Reserva.FechaFin)})) 
                            AND {nameof(Reserva.FechaTerminado)} IS NULL";

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                if (idInm.HasValue) 
                    command.Parameters.AddWithValue($"idInm", idInm.Value);
                if (!string.IsNullOrWhiteSpace(desde) && !string.IsNullOrWhiteSpace(hasta))
                {
                    command.Parameters.AddWithValue("desde", desde);
                    command.Parameters.AddWithValue("hasta", hasta);
                }

                connection.Open();

                cantidadReservas = Convert.ToInt32(command.ExecuteScalar());

                connection.Close();
            }
        }

        return cantidadReservas;
    }

    public async Task<long> CrearAsync(Reserva reserva)
    {
        int id = 0;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                INSERT INTO reservas 
                (
                    {nameof(Reserva.IdInmueble)}, 
                    {nameof(Reserva.IdInquilino)}, 
                    {nameof(Reserva.Monto)}, 
                    {nameof(Reserva.FechaInicio)}, 
                    {nameof(Reserva.FechaFin)}, 
                    {nameof(Reserva.IdUsuarioReservador)} 
                )
                VALUES 
                (
                    @{nameof(Reserva.IdInmueble)}, 
                    @{nameof(Reserva.IdInquilino)}, 
                    @{nameof(Reserva.Monto)}, 
                    @{nameof(Reserva.FechaInicio)}, 
                    @{nameof(Reserva.FechaFin)}, 
                    @{nameof(Reserva.IdUsuarioReservador)} 
                ); 
                
                SELECT LAST_INSERT_ID();"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Reserva.IdInmueble)}", reserva.IdInmueble);
                command.Parameters.AddWithValue($"{nameof(Reserva.IdInquilino)}", reserva.IdInquilino);
                command.Parameters.AddWithValue($"{nameof(Reserva.Monto)}", reserva.Monto);
                command.Parameters.AddWithValue($"{nameof(Reserva.FechaInicio)}", reserva.FechaInicio);
                command.Parameters.AddWithValue($"{nameof(Reserva.FechaFin)}", reserva.FechaFin);
                command.Parameters.AddWithValue($"{nameof(Reserva.IdUsuarioReservador)}", reserva.IdUsuarioReservador);

                try
                {
                    connection.Open();

                    id = Convert.ToInt32(command.ExecuteScalar());
                    reserva.Id = id;
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
        bool borrada = false;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                UPDATE reservas 
                SET {nameof(Reserva.Borrado)} = 1 
                WHERE {nameof(Reserva.Id)} = @{nameof(Reserva.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Reserva.Id)}", id);

                connection.Open();
                borrada = command.ExecuteNonQuery() > 0;
                connection.Close();
            }
        }

        return borrada;
    }

    public async Task<bool> EstaOcupado(string desde, string hasta, int inmuebleId, long reservaId)
    {
        int reservasContadas = 0;

        string sql = @$"
            SELECT COUNT({nameof(Reserva.Id)}) 
            FROM reservas 
            WHERE {nameof(Reserva.IdInmueble)} = @idInmueble 
                AND ({nameof(Reserva.FechaInicio)} BETWEEN @desde AND @hasta 
                    OR {nameof(Reserva.FechaFin)} BETWEEN @desde AND @hasta
                    OR (@desde >= {nameof(Reserva.FechaInicio)} AND @desde <= {nameof(Reserva.FechaFin)})
                    OR (@hasta >= {nameof(Reserva.FechaInicio)} AND @hasta <= {nameof(Reserva.FechaFin)}));"
        ;

        using (var connection = new MySqlConnection(_connectionString))
        {
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("idInmueble", inmuebleId);
                command.Parameters.AddWithValue("desde", desde);
                command.Parameters.AddWithValue("hasta", hasta);

                connection.Open();
                reservasContadas = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }

        return reservasContadas > 0;
    }

    public Task<List<Reserva>> ListarAsync(int limit, int offset)
    {
        throw new NotImplementedException();
    }

    public async Task<IList<Reserva>> ListarReservas(int? offset, int? limit, int? idInm, string? desde, string? hasta)
    {
        var reservas = new List<Reserva>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                SELECT 
                    r.{nameof(Reserva.Id)}, 
                    r.{nameof(Reserva.IdInmueble)}, 
                    r.{nameof(Reserva.IdInquilino)}, 
                    r.{nameof(Reserva.Monto)}, 
                    r.{nameof(Reserva.FechaInicio)}, 
                    r.{nameof(Reserva.FechaFin)}, 
                    r.{nameof(Reserva.FechaTerminado)}, 
                    r.{nameof(Reserva.IdUsuarioReservador)}, 
                    r.{nameof(Reserva.IdUsuarioTerminador)}, 
                    inm.{nameof(Inmueble.IdPropietario)}, 
                    inm.{nameof(Inmueble.Calle)}, 
                    inm.{nameof(Inmueble.NroCalle)}, 
                    inm.{nameof(Inmueble.IdTipoInmueble)}, 
                    ti.{nameof(Inmueble.Tipo)}, 
                    p.{nameof(Propietario.Nombre)}, 
                    p.{nameof(Propietario.Apellido)}, 
                    p.{nameof(Propietario.Dni)}, 
                    inq.{nameof(Inquilino.Nombre)}, 
                    inq.{nameof(Inquilino.Apellido)}, 
                    inq.{nameof(Inquilino.Dni)} 
                FROM reservas AS r 
                INNER JOIN inmuebles AS inm 
                    ON r.{nameof(Reserva.IdInmueble)} = inm.id 
                INNER JOIN tipos_inmueble AS ti 
                    ON inm.{nameof(Inmueble.IdTipoInmueble)} = ti.id 
                INNER JOIN propietarios AS p 
                    ON inm.{nameof(Inmueble.IdPropietario)} = p.id 
                INNER JOIN inquilinos AS inq 
                    ON r.{nameof(Reserva.IdInquilino)} = inq.id 
                WHERE r.{nameof(Reserva.Borrado)} = 0"
            ;

            if (idInm.HasValue)
                sql += $" AND c.{nameof(Reserva.IdInmueble)} = @idInm";

            if (!string.IsNullOrWhiteSpace(desde) && !string.IsNullOrWhiteSpace(hasta))
                sql += @$" AND ((c.{nameof(Reserva.FechaInicio)} BETWEEN @desde AND @hasta) 
                            OR (c.{nameof(Reserva.FechaFin)} BETWEEN @desde AND @hasta)) 
                            OR @desde BETWEEN c.{nameof(Reserva.FechaInicio)} AND c.{nameof(Reserva.FechaFin)} 
                            OR @hasta BETWEEN c.{nameof(Reserva.FechaInicio)} AND c.{nameof(Reserva.FechaFin)} 
                            AND c.{nameof(Reserva.FechaTerminado)} IS NULL";

            if (offset.HasValue && limit.HasValue)
                    sql += $" LIMIT @limit OFFSET @offset";

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                if (idInm.HasValue) command.Parameters.AddWithValue($"idInm", idInm.Value);

                if (!string.IsNullOrWhiteSpace(desde) && !string.IsNullOrWhiteSpace(hasta))
                {
                    command.Parameters.AddWithValue("desde", desde);
                    command.Parameters.AddWithValue("hasta", hasta);
                }

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
                        reservas.Add(new Reserva
                        {
                            Id = reader.GetInt64(nameof(Reserva.Id)),
                            IdInmueble = reader.GetInt32(nameof(Reserva.IdInmueble)),
                            IdInquilino = reader.GetInt32(nameof(Reserva.IdInquilino)),
                            Monto = reader.GetDecimal(nameof(Reserva.Monto)),
                            FechaInicio = reader.GetDateTime(nameof(Reserva.FechaInicio)),
                            FechaFin = reader.GetDateTime(nameof(Reserva.FechaFin)),
                            FechaTerminado = reader[nameof(Reserva.FechaTerminado)] == DBNull.Value ? null : reader.GetDateTime(nameof(Reserva.FechaTerminado)),
                            IdUsuarioReservador = reader[nameof(Reserva.IdUsuarioReservador)] == DBNull.Value ? 0 : reader.GetInt32(nameof(Reserva.IdUsuarioReservador)),
                            IdUsuarioTerminador = reader[nameof(Reserva.IdUsuarioTerminador)] == DBNull.Value ? 0 : reader.GetInt32(nameof(Reserva.IdUsuarioTerminador)),
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(nameof(Reserva.IdInmueble)),
                                IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                                IdTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
                                Calle = reader.GetString(nameof(Inmueble.Calle)),
                                NroCalle = reader.GetUInt32(nameof(Inmueble.NroCalle)),
                                Tipo = new TipoInmueble
                                {
                                    Id = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
                                    Tipo = reader.GetString(nameof(TipoInmueble.Tipo))
                                },
                                Duenio = new Propietario
                                {
                                    Id = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                                    Nombre = reader.GetString(nameof(Propietario.Nombre)),
                                    Apellido = reader.GetString(nameof(Propietario.Apellido)),
                                    Dni = reader.GetString(nameof(Propietario.Dni))
                                }
                            },
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(nameof(Reserva.IdInquilino)),
                                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                                Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                                Dni = reader.GetString(nameof(Inquilino.Dni))
                            }
                        });
                    }
                }
            }
        }

        return reservas;
    }

    public async Task<Reserva?> ObtenerPorIdAsync(long id)
    {
        Reserva? reserva = null;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                SELECT 
                    r.{nameof(Reserva.Id)}, 
                    r.{nameof(Reserva.IdInmueble)}, 
                    r.{nameof(Reserva.IdInquilino)}, 
                    r.{nameof(Reserva.Monto)}, 
                    r.{nameof(Reserva.FechaInicio)}, 
                    r.{nameof(Reserva.FechaFin)}, 
                    r.{nameof(Reserva.FechaTerminado)}, 
                    r.{nameof(Reserva.IdUsuarioReservador)}, 
                    r.{nameof(Reserva.IdUsuarioTerminador)}, 
                    inm.{nameof(Inmueble.IdPropietario)}, 
                    inm.{nameof(Inmueble.Calle)}, 
                    inm.{nameof(Inmueble.NroCalle)}, 
                    inm.{nameof(Inmueble.IdTipoInmueble)}, 
                    inm.{nameof(Inmueble.Precio)}, 
                    inm.{nameof(Inmueble.Senia)}, 
                    ti.{nameof(Inmueble.Tipo)}, 
                    p.{nameof(Propietario.Nombre)} AS nombreProp, 
                    p.{nameof(Propietario.Apellido)} AS apellidoProp, 
                    p.{nameof(Propietario.Dni)} AS dniProp, 
                    inq.{nameof(Inquilino.Nombre)} AS nombreInq, 
                    inq.{nameof(Inquilino.Apellido)} AS apellidoInq, 
                    inq.{nameof(Inquilino.Dni)} AS dniInq, 
                    uc.{nameof(Usuario.Nombre)} AS nombreContratador, 
                    uc.{nameof(Usuario.Apellido)} AS apellidoContratador, 
                    uc.{nameof(Usuario.Rol)} AS RolContratador, 
                    ut.{nameof(Usuario.Nombre)} AS nombreTerminador, 
                    ut.{nameof(Usuario.Apellido)} AS apellidoTerminador, 
                    ut.{nameof(Usuario.Rol)} AS RolTerminador  
                FROM reservas AS r 
                INNER JOIN usuarios AS uc 
                    ON r.{nameof(Reserva.IdUsuarioReservador)} = uc.id 
                LEFT JOIN usuarios AS ut 
                    ON r.{nameof(Reserva.IdUsuarioTerminador)} = ut.id 
                INNER JOIN inmuebles AS inm 
                    ON r.{nameof(Reserva.IdInmueble)} = inm.id 
                INNER JOIN tipos_inmueble AS ti 
                    ON inm.{nameof(Inmueble.IdTipoInmueble)} = ti.id 
                INNER JOIN propietarios AS p 
                    ON inm.{nameof(Inmueble.IdPropietario)} = p.id 
                INNER JOIN inquilinos AS inq 
                    ON r.{nameof(Reserva.IdInquilino)} = inq.id 
                WHERE r.{nameof(Reserva.Borrado)} = 0 AND r.{nameof(Reserva.Id)} = @{nameof(Reserva.Id)}"
            ;

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                command.Parameters.AddWithValue($"{nameof(Reserva.Id)}", id);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        reserva = new Reserva
                        {
                            Id = reader.GetInt64(nameof(Reserva.Id)),
                            IdInmueble = reader.GetInt32(nameof(Reserva.IdInmueble)),
                            IdInquilino = reader.GetInt32(nameof(Reserva.IdInquilino)),
                            Monto = reader.GetDecimal(nameof(Reserva.Monto)),
                            FechaInicio = reader.GetDateTime(nameof(Reserva.FechaInicio)),
                            FechaFin = reader.GetDateTime(nameof(Reserva.FechaFin)),
                            FechaTerminado = reader[nameof(Reserva.FechaTerminado)] == DBNull.Value ? null : reader.GetDateTime(nameof(Reserva.FechaTerminado)),
                            IdUsuarioReservador = reader[nameof(Reserva.IdUsuarioReservador)] == DBNull.Value ? 0 : reader.GetInt32(nameof(Reserva.IdUsuarioReservador)),
                            IdUsuarioTerminador = reader[nameof(Reserva.IdUsuarioTerminador)] == DBNull.Value ? 0 : reader.GetInt32(nameof(Reserva.IdUsuarioTerminador)),
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(nameof(Reserva.IdInmueble)),
                                IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                                IdTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
                                Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                                Senia = reader.GetInt32(nameof(Inmueble.Senia)),
                                Calle = reader.GetString(nameof(Inmueble.Calle)),
                                NroCalle = reader.GetUInt32(nameof(Inmueble.NroCalle)),
                                Tipo = new TipoInmueble
                                {
                                    Id = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
                                    Tipo = reader.GetString(nameof(TipoInmueble.Tipo))
                                },
                                Duenio = new Propietario
                                {
                                    Id = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                                    Nombre = reader.GetString("nombreProp"),
                                    Apellido = reader.GetString("apellidoProp"),
                                    Dni = reader.GetString("dniProp")
                                }
                            },
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(nameof(Reserva.IdInquilino)),
                                Nombre = reader.GetString("nombreInq"),
                                Apellido = reader.GetString("apellidoInq"),
                                Dni = reader.GetString("dniInq")
                            },
                            UsuarioReservador = new Usuario
                            {
                                Id = reader.GetInt32(nameof(Reserva.IdUsuarioReservador)),
                                Nombre = reader["nombreContratador"] == DBNull.Value ? null : reader.GetString("nombreContratador"),
                                Apellido = reader["apellidoContratador"] == DBNull.Value ? null : reader.GetString("apellidoContratador"),
                                Rol = reader.GetString("RolContratador")
                            }
                        };
                        if (reader[nameof(Reserva.IdUsuarioTerminador)] != DBNull.Value)
                        {
                            reserva.UsuarioTerminador = new Usuario
                            {
                                Id = reader.GetInt32(nameof(Reserva.IdUsuarioTerminador)),
                                Nombre = reader["nombreTerminador"] == DBNull.Value ? null : reader.GetString("nombreTerminador"),
                                Apellido = reader["apellidoTerminador"] == DBNull.Value ? null : reader.GetString("apellidoTerminador"),
                                Rol = reader.GetString("RolTerminador")
                            };
                        }
                    }
                }
            }
        }

        return reserva;
    }
}