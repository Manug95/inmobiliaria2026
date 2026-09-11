using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using MySql.Data.MySqlClient;

namespace inmobiliaria2026.Repositories;

public class PagoRepository(IConfiguration config) : BaseRepository(config), IPagoRepository
{
    public async Task<bool> ActualizarAsync(Pago pago)
    {
        bool modificado = false;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                UPDATE pagos 
                SET {nameof(Pago.Concepto)} = @{nameof(Pago.Concepto)} 
                WHERE {nameof(Pago.Id)} = @{nameof(Pago.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Pago.Concepto)}", pago.Concepto);
                command.Parameters.AddWithValue($"{nameof(Pago.Id)}", pago.Id);

                connection.Open();
                modificado = command.ExecuteNonQuery() > 0;
                connection.Close();
            }
        }

        return modificado;
    }

    public async Task<long> ContarPagos(long reservaId = 0)
    {
        long cantidadPagos = 0;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = $"SELECT COUNT({nameof(Pago.Id)}) AS cantidad FROM pagos";

            if (reservaId > 0)
                sql += $" WHERE {nameof(Pago.ReservaId)} = @{nameof(Pago.ReservaId)}";

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                if (reservaId > 0) 
                    command.Parameters.AddWithValue($"{nameof(Pago.ReservaId)}", reservaId);

                connection.Open();
                cantidadPagos = Convert.ToInt64(command.ExecuteScalar());
                connection.Close();
            }
        }

        return cantidadPagos;
    }

    public async Task<long> ContarPagosDeAlquileres(long reservaId)
    {
        long cantidadPagos = 0;
        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                SELECT COUNT({nameof(Pago.Id)}) AS cantidad 
                FROM pagos 
                WHERE {nameof(Pago.ReservaId)} = @{nameof(Pago.ReservaId)} 
                    AND {nameof(Pago.Anulado)} = 0;"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Pago.ReservaId)}", reservaId);
                connection.Open();
                cantidadPagos = Convert.ToInt64(command.ExecuteScalar());
                connection.Close();
            }
        }
        return cantidadPagos;
    }

    public async Task<long> CrearAsync(Pago pago)
    {
        long id = 0;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                INSERT INTO pagos 
                (
                    {nameof(Pago.ReservaId)}, 
                    {nameof(Pago.Fecha)}, 
                    {nameof(Pago.Importe)}, 
                    {nameof(Pago.Concepto)}, 
                    {nameof(Pago.IdUsuarioCobrador)} 
                )
                VALUES 
                (
                    @{nameof(Pago.ReservaId)}, 
                    @{nameof(Pago.Fecha)}, 
                    @{nameof(Pago.Importe)}, 
                    @{nameof(Pago.Concepto)}, 
                    {nameof(Pago.IdUsuarioCobrador)}  
                ); 
                
                SELECT LAST_INSERT_ID();"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Pago.ReservaId)}", pago.ReservaId);
                command.Parameters.AddWithValue($"{nameof(Pago.Fecha)}", pago.Fecha);
                command.Parameters.AddWithValue($"{nameof(Pago.Importe)}", pago.Importe);
                command.Parameters.AddWithValue($"{nameof(Pago.Concepto)}", pago.Concepto);
                command.Parameters.AddWithValue($"{nameof(Pago.IdUsuarioCobrador)}", pago.IdUsuarioCobrador);

                try
                {
                    connection.Open();

                    id = Convert.ToInt64(command.ExecuteScalar());
                    pago.Id = id;
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
        bool borrado = false;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                UPDATE pagos 
                SET {nameof(Pago.Anulado)} = 1 
                WHERE {nameof(Pago.Id)} = @{nameof(Pago.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Pago.Id)}", id);

                connection.Open();
                borrado = command.ExecuteNonQuery() > 0;
                connection.Close();
            }
        }

        return borrado;
    }

    public async Task<bool> EliminarAsync(long id, int usuarioId)
    {
        bool borrado = false;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                UPDATE pagos 
                SET {nameof(Pago.Anulado)} = 1, 
                    {nameof(Pago.IdUsuarioAnulador)} = @{nameof(Pago.IdUsuarioAnulador)}  
                WHERE {nameof(Pago.Id)} = @{nameof(Pago.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Pago.IdUsuarioAnulador)}", usuarioId);
                command.Parameters.AddWithValue($"{nameof(Pago.Id)}", id);

                connection.Open();
                borrado = command.ExecuteNonQuery() > 0;
                connection.Close();
            }
        }

        return borrado;
    }

    public Task<List<Pago>> ListarAsync(int limit, int offset)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Pago>> ListarPagos(int? offset = null, int? limit = null, long reservaId = 0)
    {
        var pagos = new List<Pago>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                SELECT 
                    p.{nameof(Pago.Id)},  
                    p.{nameof(Pago.ReservaId)},  
                    p.{nameof(Pago.Fecha)}, 
                    p.{nameof(Pago.Importe)}, 
                    p.{nameof(Pago.Anulado)}, 
                    IFNULL({nameof(Pago.Concepto)}, 'Sin Concepto') AS con, 
                    r.{nameof(Reserva.FechaInicio)}, 
                    r.{nameof(Reserva.FechaFin)}, 
                    r.{nameof(Reserva.FechaTerminado)}, 
                    r.{nameof(Reserva.IdInquilino)}, 
                    r.{nameof(Reserva.IdInmueble)}, 
                    r.{nameof(Reserva.Monto)}, 
                    IFNULL(r.{nameof(Reserva.IdUsuarioReservador)}, 0) AS idUContratador, 
                    IFNULL(r.{nameof(Reserva.IdUsuarioTerminador)}, 0) AS idUTerminador 
                FROM pagos AS p 
                INNER JOIN reservas AS r 
                    ON p.{nameof(Pago.ReservaId)} = r.{nameof(Reserva.Id)}"
            ;

            if (reservaId > 0)
                sql += $" WHERE p.{nameof(Pago.ReservaId)} = @{nameof(Pago.ReservaId)}";

            if (offset.HasValue && limit.HasValue)
                    sql += $" LIMIT @limit OFFSET @offset";

            using (var command = new MySqlCommand(sql + ";", connection))
            {
                if (reservaId > 0) 
                    command.Parameters.AddWithValue($"{nameof(Pago.ReservaId)}", reservaId);

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
                        pagos.Add(new Pago
                        {
                            Id = reader.GetInt64(nameof(Pago.Id)),
                            Fecha = reader.GetDateTime(nameof(Pago.Fecha)),
                            Importe = reader.GetDecimal(nameof(Pago.Importe)),
                            Concepto = reader.GetString("con"),
                            Anulado = reader.GetBoolean(nameof(Pago.Anulado)),
                            Reserva = new Reserva
                            {
                                Id = reader.GetInt64(nameof(Pago.ReservaId)),
                                FechaInicio = reader.GetDateTime(nameof(Reserva.FechaInicio)),
                                FechaFin = reader.GetDateTime(nameof(Reserva.FechaFin)),
                                FechaTerminado = reader[nameof(Reserva.FechaTerminado)] == DBNull.Value ? null : reader.GetDateTime(nameof(Reserva.FechaTerminado)),
                                IdInquilino = reader.GetInt32(nameof(Reserva.IdInquilino)),
                                IdInmueble = reader.GetInt32(nameof(Reserva.IdInmueble)),
                                IdUsuarioReservador = reader.GetInt32("idUContratador"),
                                IdUsuarioTerminador = reader.GetInt32("idUTerminador")
                            }
                        });
                    }
                }
            }
        }

        return pagos;
    }

    public async Task<Pago?> ObtenerPorIdAsync(long id)
    {
        Pago? pago = null;

        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                SELECT 
                    p.{nameof(Pago.Id)},  
                    p.{nameof(Pago.ReservaId)},   
                    p.{nameof(Pago.Fecha)}, 
                    p.{nameof(Pago.Importe)}, 
                    p.{nameof(Pago.Anulado)}, 
                    p.{nameof(Pago.IdUsuarioCobrador)}, 
                    p.{nameof(Pago.IdUsuarioAnulador)}, 
                    IFNULL({nameof(Pago.Concepto)}, 'Sin Concepto') AS con, 
                    r.{nameof(Reserva.FechaInicio)}, 
                    r.{nameof(Reserva.FechaFin)}, 
                    r.{nameof(Reserva.FechaTerminado)}, 
                    r.{nameof(Reserva.IdInquilino)}, 
                    r.{nameof(Reserva.IdInmueble)}, 
                    r.{nameof(Reserva.Monto)}, 
                    IFNULL(r.{nameof(Reserva.IdUsuarioReservador)}, 0) AS idUContratador, 
                    IFNULL(r.{nameof(Reserva.IdUsuarioTerminador)}, 0) AS idUTerminador, 
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
                    ti.{nameof(TipoInmueble.Tipo)} AS tipoInmueble, 
                    pr.{nameof(Propietario.Nombre)} AS nombreProp, 
                    pr.{nameof(Propietario.Apellido)} AS apellidoProp, 
                    pr.{nameof(Propietario.Dni)} AS dniProp, 
                    inq.{nameof(Inquilino.Nombre)} AS nombreInq, 
                    inq.{nameof(Inquilino.Apellido)} AS apellidoInq, 
                    inq.{nameof(Inquilino.Dni)} AS dniInq, 
                    uc.{nameof(Usuario.Nombre)} AS nombreCobrador, 
                    uc.{nameof(Usuario.Apellido)} AS apellidoCobrador, 
                    uc.{nameof(Usuario.Rol)} AS RolCobrador, 
                    ua.{nameof(Usuario.Nombre)} AS nombreAnulador, 
                    ua.{nameof(Usuario.Apellido)} AS apellidoAnulador, 
                    ua.{nameof(Usuario.Rol)} AS RolAnulador 
                FROM pagos AS p 
                INNER JOIN usuarios AS uc 
                    ON p.{nameof(Pago.IdUsuarioCobrador)} = uc.id 
                LEFT JOIN usuarios AS ua 
                    ON p.{nameof(Pago.IdUsuarioAnulador)} = ua.id 
                INNER JOIN reservas AS r 
                    ON p.{nameof(Pago.ReservaId)} = r.{nameof(Reserva.Id)} 
                INNER JOIN inmuebles AS i 
                    ON r.{nameof(Reserva.IdInmueble)} = i.id 
                INNER JOIN tipos_inmueble AS ti 
                    ON i.{nameof(Inmueble.IdTipoInmueble)} = ti.id 
                INNER JOIN propietarios AS pr 
                    ON i.{nameof(Inmueble.IdPropietario)} = pr.id 
                INNER JOIN inquilinos AS inq 
                    ON r.{nameof(Reserva.IdInquilino)} = inq.id 
                WHERE p.{nameof(Pago.Id)} = @{nameof(Pago.Id)};"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Pago.Id)}", id);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        pago = new Pago
                        {
                            Id = reader.GetInt32(nameof(Pago.Id)),
                            ReservaId = reader.GetInt32(nameof(Pago.ReservaId)),
                            Fecha = reader.GetDateTime(nameof(Pago.Fecha)),
                            Importe = reader.GetDecimal(nameof(Pago.Importe)),
                            Concepto = reader.GetString("con"),
                            Anulado = reader.GetBoolean(nameof(Pago.Anulado)),
                            IdUsuarioCobrador = reader.GetInt32(nameof(Pago.IdUsuarioCobrador)),
                            IdUsuarioAnulador = reader[nameof(Pago.IdUsuarioAnulador)] == DBNull.Value ? 0 : reader.GetInt32(nameof(Pago.IdUsuarioAnulador)),
                            Reserva = new Reserva
                            {
                                Id = reader.GetInt32(nameof(Pago.ReservaId)),
                                FechaInicio = reader.GetDateTime(nameof(Reserva.FechaInicio)),
                                FechaFin = reader.GetDateTime(nameof(Reserva.FechaFin)),
                                FechaTerminado = reader[nameof(Reserva.FechaTerminado)] == DBNull.Value ? null : reader.GetDateTime(nameof(Reserva.FechaTerminado)),
                                Monto = reader.GetDecimal(nameof(Reserva.Monto)),
                                IdInquilino = reader.GetInt32(nameof(Reserva.IdInquilino)),
                                IdInmueble = reader.GetInt32(nameof(Reserva.IdInmueble)),
                                IdUsuarioReservador = reader.GetInt32("idUContratador"),
                                IdUsuarioTerminador = reader.GetInt32("idUTerminador"),
                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32(nameof(Reserva.IdInmueble)),
                                    IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                                    IdTipoInmueble = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
                                    Calle = reader.GetString(nameof(Inmueble.Calle)),
                                    NroCalle = reader.GetUInt32(nameof(Inmueble.NroCalle)),
                                    Latitud = reader.GetDecimal("latitud"),
                                    Longitud = reader.GetDecimal("longitud"),
                                    Disponible = reader.GetBoolean(nameof(Inmueble.Disponible)),
                                    Cupo = reader.GetInt32(nameof(Inmueble.Cupo)),
                                    Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                                    Senia = reader.GetInt32(nameof(Inmueble.Senia)),
                                    Tipo = new TipoInmueble
                                    {
                                        Id = reader.GetInt32(nameof(Inmueble.IdTipoInmueble)),
                                        Tipo = reader.GetString("tipoInmueble")
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
                                }
                            },
                            UsuarioCobrador = new Usuario
                            {
                                Id = reader.GetInt32(nameof(Pago.IdUsuarioCobrador)),
                                Nombre = reader["nombreCobrador"] == DBNull.Value ? null : reader.GetString("nombreCobrador"),
                                Apellido = reader["apellidoCobrador"] == DBNull.Value ? null : reader.GetString("apellidoCobrador"),
                                Rol = reader.GetString("rolCobrador")
                            }
                        };
                        if (reader[nameof(Pago.IdUsuarioAnulador)] != DBNull.Value)
                        {
                            pago.UsuarioAnulador = new Usuario
                            {
                                Id = reader.GetInt32(nameof(Pago.IdUsuarioAnulador)),
                                Nombre = reader["nombreAnulador"] == DBNull.Value ? null : reader.GetString("nombreAnulador"),
                                Apellido = reader["apellidoAnulador"] == DBNull.Value ? null : reader.GetString("apellidoAnulador"),
                                Rol = reader.GetString("rolAnulador")
                            };
                        }
                    }
                }
            }
        }

        return pago;
    }

    public async Task<decimal> SumarImportes(long reservaId)
    {
        decimal sumaMulta = 0;
        using (var connection = new MySqlConnection(_connectionString))
        {
            string sql = @$"
                SELECT IFNULL(SUM({nameof(Pago.Importe)}), 0) AS suma 
                FROM pagos 
                WHERE {nameof(Pago.ReservaId)} = @{nameof(Pago.ReservaId)} 
                    AND {nameof(Pago.Anulado)} = 0;"
            ;

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"{nameof(Pago.ReservaId)}", reservaId);
                connection.Open();
                sumaMulta = Convert.ToDecimal(command.ExecuteScalar());
                connection.Close();
            }
        }
        return sumaMulta;
    }
}