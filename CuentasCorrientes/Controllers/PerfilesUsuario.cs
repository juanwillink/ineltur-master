using System;

namespace Ineltur.CuentasCorrientes.Controllers
{
    public static class PerfilesUsuario
    {
        public static readonly Guid PerfilAdministrador = new Guid("B0086C7B-9086-429E-96E4-E71BB97FB784");
        public static readonly Guid PerfilAuditor = new Guid("432A5341-62D2-43F5-913C-57DD235516E1");
        public static readonly Guid PerfilConcentrador = new Guid("A6019388-75B1-432A-8045-3FE17073C67F");
        public static readonly Guid PerfilCliente = new Guid("5D9A7C90-94EC-45FE-BDC3-32709E54F704");

    }
}