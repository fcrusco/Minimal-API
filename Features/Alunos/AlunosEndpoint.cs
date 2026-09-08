using GestaoAcademica.Api.Data;                    // Importa o namespace do contexto de banco de dados (AppDbContext)
using Microsoft.AspNetCore.Http.HttpResults;      // Importa os resultados tipados HTTP (Created, Ok, NotFound, ValidationProblem, etc.)
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


    // Valida os campos obrigatórios do DTO de entrada. Retorna null quando os dados são válidos,
    // ou um ValidationProblem (400) com o dicionário de erros por campo quando algo está inválido.
    private static ValidationProblem? ValidarAluno(SalvarAlunoRequest request)
    {
        var erros = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Nome))
            erros[nameof(request.Nome)] = ["O nome do aluno é obrigatório."];

        if (string.IsNullOrWhiteSpace(request.Turma))
            erros[nameof(request.Turma)] = ["A turma é obrigatória."];

        if (string.IsNullOrWhiteSpace(request.Periodo))
            erros[nameof(request.Periodo)] = ["O período é obrigatório."];

        return erros.Count > 0 ? TypedResults.ValidationProblem(erros) : null;
    }


    // Handler assíncrono para cadastrar um novo aluno, recebendo o DTO de entrada e o contexto do banco via injeção de método
    // Retorna AlunoResponse (DTO de saída) em vez da entidade Aluno, mantendo o contrato consistente com os demais endpoints
    // Valida o request antes de qualquer acesso ao banco
    private static async Task<Results<Created<AlunoResponse>, ValidationProblem>> CriarAluno(SalvarAlunoRequest request, AppDbContext db)
    {
        var erroValidacao = ValidarAluno(request);
        if (erroValidacao is not null) return erroValidacao;

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

        var response = new AlunoResponse(aluno.Id, aluno.Nome, aluno.Turma, aluno.Periodo);

        // Retorna o status HTTP 201 Created contendo a URI de acesso ao novo recurso e o DTO de saída
        return TypedResults.Created($"/api/alunos/{aluno.Id}", response);
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
    // [Item 1] Retorna AlunoResponse em vez da entidade Aluno crua
    private static async Task<Results<Ok<AlunoResponse>, NotFound>> ObterPorId(int id, AppDbContext db)
    {
        var aluno = await db.Alunos.FindAsync(id); // Localiza o registro na base de dados utilizando a chave primária
        if (aluno is null) return TypedResults.NotFound(); // Retorna 404 se o registro não for encontrado

        var response = new AlunoResponse(aluno.Id, aluno.Nome, aluno.Turma, aluno.Periodo);
        return TypedResults.Ok(response); // Retorna 200 OK com o DTO de saída
    }


    // Handler assíncrono para atualizar dados de um aluno existente, retornando NoContent ou NotFound
    // Agora recebe o DTO de entrada (SalvarAlunoRequest) em vez da entidade Aluno inteira,
    // evitando que o cliente envie/manipule campos como o Id diretamente no corpo da requisição.
    // Valida o request antes de consultar o banco
    private static async Task<Results<NoContent, NotFound, ValidationProblem>> AtualizarAluno(int id, SalvarAlunoRequest request, AppDbContext db)
    {
        var erroValidacao = ValidarAluno(request);
        if (erroValidacao is not null) return erroValidacao;

        var alunoExistente = await db.Alunos.FindAsync(id); // Busca o aluno pelo ID informado na rota
        if (alunoExistente is null) return TypedResults.NotFound(); // Retorna 404 se o registro não for encontrado

        // Atualiza as propriedades da entidade rastreada com os novos dados recebidos no DTO
        alunoExistente.Nome = request.Nome;
        alunoExistente.Turma = request.Turma;
        alunoExistente.Periodo = request.Periodo;

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