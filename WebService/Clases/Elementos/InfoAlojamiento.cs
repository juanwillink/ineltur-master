using System;
using Ineltur.Datos;
using System.Collections.Generic;

namespace Ineltur.WebService
{
    public class InfoAlojamiento
    {
        public Guid IdAlojamiento { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Descripcion2 { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public InfoDestino Destino { get; set; }
        public TipoAlojamiento Tipo { get; set; }
        public CategoriaAlojamiento Categoria { get; set; }
        public string PoliticasCancelacion { get; set; }
        public string FotoAlojamientoUrl { get; set; }
        public string FotoAlojamientoDescripcion { get; set; }
        public List<string> FotoUrlLista { get; set; }
        public List<string> FotoDescripcionLista { get; set; }
        public List<string> Amenidades { get; set; }
        public RegimenAlojamiento Regimen { get; set; }
        public bool BajoPeticion { get; set; }
        //public int? DiasCancelacionCargo { get; set; }

        //Info Cupos
        public Moneda Moneda { get; set; }
        public InfoUnidad[] Unidades { get; set; }
        public float? Latitud { get; set; }
        public float? Longitud { get; set; }
        public bool TienePromocion { get; set; }

    }

    public class InfoCuposAlojamiento
    {
        public Guid IdAlojamiento { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public InfoUnidadConCupos[] Unidades { get; set; }

    }

}