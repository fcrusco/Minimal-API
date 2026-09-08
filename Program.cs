using GestaoAcademica.Api.Data; // Importa o namespace do contexto de dados (AppDbContext)
using GestaoAcademica.Api.Features.Alunos; // Importa o namespace da feature para ter acesso ao método de extensão de rotas
using Microsoft.EntityFrameworkCore; // Importa as extensões do EF Core (como UseInMemoryDatabase)
using Scalar.AspNetCore; // Importa o namespace da biblioteca Scalar para documentação interativa

var builder = WebApplication.CreateBuilder(args); // Cria o construtor da aplicação web, inicializando configurações e serviços

// Configura o Entity Framework Core para utilizar o banco de dados em memória, nomeando a instância como "GestaoAcademicaDb"
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("GestaoAcademicaDb"));

builder.Services.AddOpenApi(); // Adiciona os serviços nativos do .NET para geração da especificação OpenAPI

// [Item 6] Registra os serviços necessários para a geração automática de respostas ProblemDetails em erros não tratados
builder.Services.AddProblemDetails();

var app = builder.Build(); // Compila a aplicação e constrói o pipeline de processamento HTTP

// [Item 6] Middleware global de tratamento de exceções: qualquer exceção não tratada no pipeline
// é convertida automaticamente em uma resposta padronizada application/problem+json (status 500),
// em vez de vazar stack trace ou derrubar a conexão sem uma resposta estruturada.
app.UseExceptionHandler();

// Executa blocos exclusivos para o ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Expõe o endpoint JSON contendo o contrato da especificação OpenAPI
    app.MapScalarApiReference(); // Configura e mapeia a interface gráfica moderna do Scalar no endpoint padrão
}

app.MapAlunosEndpoints(); // Executa o método de extensão que registra todas as rotas e handlers do módulo de alunos

app.Run(); // Inicializa o servidor web Kestrel e coloca a API em execução ouvindo as requisições