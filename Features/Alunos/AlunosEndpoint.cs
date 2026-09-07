using GestaoAcademica.Api.Data;                    // Importa o namespace do contexto de banco de dados (AppDbContext)
using Microsoft.AspNetCore.Http.HttpResults;      // Importa os resultados tipados HTTP (Created, Ok, NotFound, etc.)
using Microsoft.EntityFrameworkCore;              // Importa as extensões e métodos assíncronos do EF Core (ToListAsync, etc.)
using static GestaoAcademica.Api.Features.Alunos.Aluno; // Importa estaticamente os records aninhados (SalvarAlunoRequest e AlunoResponse)

namespace GestaoAcademica.Api.Features.Alunos;      // Define o namespace lógico da feature de alunos


public static class AlunosEndpoint                // Declara uma classe estática para conter os mapeamentos de rotas e handlers
{


    // Método de extensão responsável por registrar e agrupar todas as rotas do módulo de alunos
    public static void MapAlunosEndpoints(this IEndpointRouteBuilder app)
    {
        // Cria um grupo de rotas com o prefixo comum '/api/alunos' e define a tag para a documentação OpenAPI/Scalar
        var group = app.MapGroup("/api/alunos").WithTags("Alunos");

        group.MapPost("/", CriarAluno);             // Mapeia a rota POST para inserção de um novo aluno
        group.MapGet("/", ObterTodos);              // Mapeia a rota GET para listagem de todos os alunos

        // Mapeia a rota GET por ID, aplicando uma restrição explícita para aceitar apenas valores inteiros (:int)
        group.MapGet("/{id:int}", ObterPorId);

        // Mapeia a rota PUT por ID com restrição inteira para atualização de registros existentes
        group.MapPut("/{id:int}", AtualizarAluno);

        // Mapeia a rota DELETE por ID com restrição inteira para remoção de registros
        group.MapDelete("/{id:int}", DeletarAluno);
    }



    // Handler assíncrono para cadastrar um novo aluno, recebendo o DTO de entrada e o contexto do banco via injeção de método
    private static async Task<Created<Aluno>> CriarAluno(SalvarAlunoRequest request, AppDbContext db)
    {
        // Calcula o próximo ID de forma incremental: se houver registros, pega o maior ID e soma 1, caso contrário inicia em 1
        int proximoId = await db.Alunos.AnyAsync()
            ? await db.Alunos.MaxAsync(a => a.Id) + 1
            : 1;

        // Instancia a entidade de domínio preenchendo o ID calculado e os dados enviados na requisição
        var aluno = new Aluno
        {
            Id = proximoId,
            Nome = request.Nome,
            Turma = request.Turma,
            Periodo = request.Periodo
        };

        db.Alunos.Add(aluno);          // Adiciona a nova entidade ao rastreamento do Entity Framework
        await db.SaveChangesAsync();   // Persiste as alterações de forma assíncrona no banco de dados

        // Retorna o status HTTP 201 Created contendo a URI de acesso ao novo recurso e o objeto criado
        return TypedResults.Created($"/api/alunos/{aluno.Id}", aluno);
    }



    // Handler assíncrono que consulta todos os alunos e os mapeia para uma lista de DTOs de resposta
    private static async Task<Ok<List<AlunoResponse>>> ObterTodos(AppDbContext db)
    {
        var alunos = await db.Alunos.ToListAsync(); // Consulta assíncrona de todos os registros da tabela de alunos

        // Transforma a lista de entidades de banco em uma lista do record imutável de saída (DTO) usando LINQ
        var response = alunos.Select(a =>
            new AlunoResponse(a.Id, a.Nome, a.Turma, a.Periodo)
        ).ToList();

        return TypedResults.Ok(response);          // Retorna o status HTTP 200 OK contendo a lista formatada
    }



    // Handler assíncrono para buscar um aluno específico por ID, podendo retornar Ok ou NotFound
    private static async Task<Results<Ok<Aluno>, NotFound>> ObterPorId(int id, AppDbContext db)
    {
        var aluno = await db.Alunos.FindAsync(id); // Localiza o registro na base de dados utilizando a chave primária

        // Operador condicional: se o aluno existir retorna 200 OK com o objeto, senão retorna 404 NotFound
        return aluno is not null ? TypedResults.Ok(aluno) : TypedResults.NotFound();
    }



    // Handler assíncrono para atualizar dados de um aluno existente, retornando NoContent ou NotFound
    private static async Task<Results<NoContent, NotFound>> AtualizarAluno(int id, Aluno alunoAtualizado, AppDbContext db)
    {
        var alunoExistente = await db.Alunos.FindAsync(id); // Busca o aluno pelo ID informado na rota
        if (alunoExistente is null) return TypedResults.NotFound(); // Retorna 404 se o registro não for encontrado

        // Atualiza as propriedades da entidade rastreada com os novos dados recebidos
        alunoExistente.Nome = alunoAtualizado.Nome;
        alunoExistente.Turma = alunoAtualizado.Turma;
        alunoExistente.Periodo = alunoAtualizado.Periodo;

        await db.SaveChangesAsync();         // Salva as alterações efetivadas no banco
        return TypedResults.NoContent();     // Retorna o status HTTP 204 No Content indicando sucesso sem corpo de resposta
    }



    // Handler assíncrono para remover um aluno da base de dados
    private static async Task<Results<NoContent, NotFound>> DeletarAluno(int id, AppDbContext db)
    {
        var aluno = await db.Alunos.FindAsync(id); // Localiza o registro pelo ID
        if (aluno is null) return TypedResults.NotFound(); // Retorna 404 caso o aluno não exista

        db.Alunos.Remove(aluno);             // Marca a entidade para exclusão no contexto do EF Core
        await db.SaveChangesAsync();         // Efetiva a remoção na base de dados

        return TypedResults.NoContent();     // Retorna o status HTTP 204 No Content indicando sucesso na exclusão
    }
}