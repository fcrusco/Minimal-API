using GestaoAcademica.Api.Features.Alunos; // Importa o namespace onde a classe de domínio 'Aluno' está localizada
using Microsoft.EntityFrameworkCore;      // Importa as classes e métodos principais do Entity Framework Core

namespace GestaoAcademica.Api.Data;        // Define o namespace lógico onde esta classe de infraestrutura está agrupada

public class AppDbContext : DbContext     // Declara a classe do contexto do banco de dados herdando as funcionalidades do EF Core
{
    // Construtor que recebe as opções de configuração (como a engine in-memory) e as repassa para o construtor da classe base (DbContext)
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Mapeia a tabela 'Alunos' no banco de dados virtual, permitindo consultas e persistência baseada na entidade Aluno
    public DbSet<Aluno> Alunos => Set<Aluno>();
}