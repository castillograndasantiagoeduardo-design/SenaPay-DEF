// Services/Interfaces/ITransaccionService.cs
using SenaPay.Domain.Interfaces.Tienda.DTOs;
using SenaPay.Domain.Entities;

namespace SenaPay.Domain.Interfaces.Tienda;


    public interface ITransaccionService
    {
        Task<ResultadoCompra> ProcesarCompraAsync(int idAprendiz, CarritoDto carrito);
    }
 