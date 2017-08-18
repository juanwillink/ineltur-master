using System;
using System.Linq;
using System.Web.Mvc;
using Ineltur.CuentasCorrientes.Modelos;
using Ineltur.CuentasCorrientes.Modelos.Servicios;
using Ineltur.Datos.Entidades;

namespace Ineltur.CuentasCorrientes.Controllers
{
    using TipoMoneda = Ineltur.Datos.Moneda;

    public class AdministracionController : BaseController
    {
        private IQueryable<MovimientoCuentaCorriente> ListaDetalles(Guid id, DateTime? desde, DateTime? hasta)
        //private IQueryable<Transaccion> ListaDetalles(Guid id, DateTime? desde, DateTime? hasta)
        {
            var detalles = DataContext.MovimientosCuentaCorriente.Where(d => d.IdUsuario == id);
            //var detalles = DataContext.Transacciones.Where(d => d.IdUsuario == id);

            if (desde.HasValue)
            {
                desde = desde.Value.Date;

                detalles = detalles.Where(d => d.FechaMovimiento >= desde.Value);
                //detalles = detalles.Where(d => d.FechaAlta >= desde.Value);
            }
            if (hasta.HasValue)
            {
                hasta = (hasta.Value + TimeSpan.FromDays(1)).Date;

                detalles = detalles.Where(d => d.FechaMovimiento < hasta);
                //detalles = detalles.Where(d => d.FechaAlta < hasta);
            }

            return detalles;
        }

        // **************************************
        // URL: /Administracion/Menu
        // **************************************

        [Authorize]
        public ActionResult Menu()
        {
            return View();
        }

        // **************************************
        // URL: /Administracion/Cuentas
        // **************************************

        [Authorize]
        public ActionResult Cuentas()
        {
            var modelo = new ModeloListaCuentas()
            {
                Cuentas = DataContext.Usuarios.Where(u => u.IdTipoUsuario == PerfilesUsuario.PerfilConcentrador || u.IdTipoUsuario == PerfilesUsuario.PerfilCliente).Select(
                u => new ModeloCuenta()
                {
                    IdUsuario = u.IdUsuario,
                    Nombre = String.Format("{0} {1}", u.Nombre, u.Apellido).Trim(),
                    NombreUsuario = u.NombreUsuario,
                    Activo = u.Activo,

                    Saldo = (decimal)u.MovimientosCuentaCorriente.Sum(
                    //Saldo = (decimal)u.Transacciones.Sum(

                            m => (double)((m.MontoHaber - m.MontoDebe) * m.Cotizacion)),
                    FormaPago = u.UsuarioWebService.FormaPago,
                    Margen = u.UsuarioWebService.Margen
                }).ToArray()
            };

            Array.ForEach(modelo.Cuentas, c =>
            {
                c.Saldo = c.Saldo.HasValue ? Decimal.Round(c.Saldo.Value, 2) : (decimal?)null;
            });

            return View(modelo);
        }

        // **************************************
        // URL: /Administracion/DatosCuenta
        // **************************************

        [Authorize]
        public ActionResult DatosCuenta(Guid id)
        {
            var modelo = DataContext.Usuarios.Where(u => u.IdUsuario == id && u.IdTipoUsuario == PerfilesUsuario.PerfilConcentrador || u.IdUsuario == id && u.IdTipoUsuario == PerfilesUsuario.PerfilCliente).Select(
            u => new ModeloCuenta()
            {
                IdUsuario = u.IdUsuario,
                Nombre = String.Format("{0} {1}", u.Nombre, u.Apellido).Trim(),
                NombreUsuario = u.NombreUsuario,
                Contrasenya = u.Clave,
                Activo = u.Activo,
                FormaPago = u.UsuarioWebService.FormaPago,
                Margen = u.UsuarioWebService.Margen
            }).SingleOrDefault();

            return View(modelo);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DatosCuenta(ModeloCuenta cuenta)
        {
            var usuario = DataContext.Usuarios.Where(u => u.IdUsuario == cuenta.IdUsuario && u.IdTipoUsuario == PerfilesUsuario.PerfilConcentrador || u.IdUsuario == cuenta.IdUsuario && u.IdTipoUsuario == PerfilesUsuario.PerfilCliente).Single();

            usuario.Clave = cuenta.Contrasenya;
            usuario.Activo = cuenta.Activo;
            if (usuario.UsuarioWebService == null)
            {
                usuario.UsuarioWebService = new UsuarioWebService();
            }
            usuario.UsuarioWebService.FormaPago = cuenta.FormaPago.GetValueOrDefault();

            var modelo = new ModeloCuenta()
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = String.Format("{0} {1}", usuario.Nombre, usuario.Apellido).Trim(),
                NombreUsuario = usuario.NombreUsuario,
                Contrasenya = usuario.Clave,
                Activo = usuario.Activo,
                FormaPago = usuario.UsuarioWebService.FormaPago,
                Margen = usuario.UsuarioWebService.Margen
            };

            return View(modelo);
        }

        // **************************************
        // URL: /Administracion/DetallesCuenta
        // **************************************

        [Authorize]
        public ActionResult DetallesCuenta(Guid id, int? pagina, DateTime? desde, DateTime? hasta)
        {
            const int DetallesPorPagina = 25;

            if (!pagina.HasValue || pagina <= 0) pagina = 1;

            var detalles = ListaDetalles(id, desde, hasta);
            int cant = detalles.Count();
            string nombre = DataContext.Usuarios.Single(u => u.IdUsuario == id).Nombre;

            detalles = detalles.Skip((pagina.Value - 1) * DetallesPorPagina);

            var modelo = new ModeloListaDetallesCuentas()
            {
                Desde = desde,
                Hasta = hasta,

                Pagina = pagina.Value,
                UltimaPagina = (cant + DetallesPorPagina - 1) / DetallesPorPagina,

                IdUsuario = id,
                Nombre = nombre,
                Detalles = detalles.Take(DetallesPorPagina).Select(m => new ModeloDetalleCuenta()
                {
                    Fecha = m.FechaMovimiento,
                    //Fecha = m.FechaAlta,

                    Tipo = m.Transaccion == null ? TipoMovimiento.Pago :
                    //Tipo = m.TipoTransaccion == null ? TipoMovimiento.Pago :

                            (m.MontoDebe ?? 0.0) > (m.MontoHaber ?? 0.0) ? TipoMovimiento.ReservaAnulada :
                            TipoMovimiento.Reserva,
                    Moneda = (TipoMoneda)Enum.Parse(typeof(TipoMoneda), m.Moneda.Descripcion, true),
                    Monto = (decimal)(m.MontoHaber - m.MontoDebe),
                    MontoPesos = (decimal)((m.MontoHaber - m.MontoDebe) * m.Cotizacion),

                    Cliente = m.Transaccion != null ? m.Transaccion.Usuario.Nombre + " " + m.Transaccion.Usuario.Apellido : null
                    //Cliente = m.TipoTransaccion != null ? m.Usuario.Nombre + " " + m.Usuario.Apellido : null

                }).ToArray()
            };

            Array.ForEach(modelo.Detalles, d =>
            {
                d.Monto = Decimal.Round(d.Monto, 2);
                d.MontoPesos = Decimal.Round(d.MontoPesos, 2);
            });

            return View(modelo);
        }

        // **************************************
        // URL: /Administracion/ExportarDetallesCuenta
        // **************************************

        [Authorize]
        public ActionResult ExportarDetallesCuenta(Guid id, DateTime? desde, DateTime? hasta)
        {
            var conv = System.ComponentModel.TypeDescriptor.GetConverter(new TipoMovimiento());
            string nombre = DataContext.Usuarios.Single(u => u.IdUsuario == id).Nombre;
            var detalles = ListaDetalles(id, desde, hasta).ToArray().Select(m => new
            {
                Fecha = m.FechaMovimiento,
                //Fecha = m.FechaAlta,

                Tipo = conv.ConvertToString((m.Transaccion == null ? TipoMovimiento.Pago :
                //Tipo = conv.ConvertToString((m.TipoTransaccion == null ? TipoMovimiento.Pago :

                        (m.MontoDebe ?? 0.0) > (m.MontoHaber ?? 0.0) ? TipoMovimiento.ReservaAnulada :
                        TipoMovimiento.Reserva)),
                Moneda = m.Moneda.Descripcion,
                Monto = Decimal.Round((decimal)(m.MontoHaber - m.MontoDebe), 2),
                MontoPesos = Decimal.Round((decimal)((m.MontoHaber - m.MontoDebe) * m.Cotizacion), 2),

                Cliente = m.Transaccion != null ? m.Transaccion.Usuario.Nombre + " " + m.Transaccion.Usuario.Apellido : null
                //Cliente = m.TipoTransaccion != null ? m.Usuario.Nombre + " " + m.Usuario.Apellido : null

            }).ToArray();

            var tabla = detalles.DataTableDeEnumerable();
            var data = MsExcel.ExportToExcel(tabla);

            return new FileContentResult(data, "application/vnd.ms-excel")
            {
                FileDownloadName = String.Format("DetallesCuenta-{0}.xls", nombre)
            };
        }

        // **************************************
        // URL: /Administracion/Pago
        // **************************************

        [Authorize]
        public ActionResult Pago(Guid id)
        {
            var usuario = DataContext.Usuarios.Single(u => u.IdUsuario == id);
            var modelo = new ModeloPago()
            {
                IdUsuario = id,
                Nombre = usuario.Nombre,

                Saldo = Decimal.Round((decimal)usuario.MovimientosCuentaCorriente.Sum(
                //Saldo = Decimal.Round((decimal)usuario.Transacciones.Sum(

                    m => (double)((m.MontoHaber - m.MontoDebe) * m.Cotizacion)), 2),
                FechaPago = DateTime.Now.Date
            };

            return View(modelo);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Pago(ModeloPago pago)
        {
            if (ModelState.IsValid)
            {
                var mov = new MovimientoCuentaCorriente()
                //var mov = new Transaccion()

                {
                    IdMovimientoCuentaCorriente = Guid.NewGuid(),
                    //IdTransaccion = Guid.NewGuid(),

                    IdUsuario = pago.IdUsuario,

                    FechaMovimiento = pago.FechaPago,
                    //FechaAlta = pago.FechaPago,
                    
                    Moneda = DataContext.Monedas.SingleOrDefault(m => m.Descripcion == pago.Moneda.ToString()),
                    MontoDebe = 0f,
                    MontoHaber = (float)pago.MontoPagado,
                    Cotizacion = (float)pago.TipoCambio
                };

                DataContext.MovimientosCuentaCorriente.InsertOnSubmit(mov);
                //DataContext.Transacciones.InsertOnSubmit(mov);
                return RedirectToAction("DetallesCuenta", new { id = pago.IdUsuario });
            }
            else
            {
                ViewBag.Message = "Detalles del pago no válidos";

                pago.Saldo = Decimal.Round((decimal)DataContext.MovimientosCuentaCorriente.Where(
                //pago.Saldo = Decimal.Round((decimal)DataContext.Transacciones.Where(
                
                    m => m.IdUsuario == pago.IdUsuario).Sum(m => (double)((m.MontoHaber - m.MontoDebe) * m.Cotizacion)), 2);
                pago.FechaPago = DateTime.Now.Date;
                return View(pago);
            }
        }

        // **************************************
        // URL: /Administracion/ObtenerCambio
        // **************************************

        [Authorize]
        public ActionResult ObtenerCambio(string id)
        {
            return Json(new
            {
                Moneda = id,
                Cambio = DataContext.Monedas.Where(m => m.Descripcion == id).Sum(m => m.Cotizacion)
            }, JsonRequestBehavior.AllowGet);
        }
    }
}