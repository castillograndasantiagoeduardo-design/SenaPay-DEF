using Microsoft.EntityFrameworkCore;
using SenaPay.Domain.Entities;
using SenaPay.Domain.Interfaces;
using SenaPay.Infrastructure.Data;

namespace SenaPay.Infrastructure.Repositories.Funcionarios;

public class FuncionarioRepository : IFuncionarioRepository
{
    private readonly SenaPayContext _context;

    public FuncionarioRepository(SenaPayContext context)
    {
        _context = context;
    }

    public async Task<Funcionario?> ObtenerPorDocumentoAsync(int documento)
    {
        return await _context.Funcionarios
            .Include(f => f.IdUsuarioNavigation)
                .ThenInclude(u => u.IdSedeNavigation)
            .FirstOrDefaultAsync(f => f.IdUsuarioNavigation.Documento == documento);
    }
}
