# Gestão Acadêmica API (.NET 10)

Projeto de exemplo para ensino de **Minimal APIs** no ASP.NET Core, com **Entity Framework Core InMemory**, **documentação via Scalar** e organização em **Vertical Slice Architecture**.

---

## 🇧🇷 Português (Brasil)

### Sobre o projeto

Este projeto é um exemplo didático de uma API de Gestão Acadêmica, focada no cadastro de Alunos (CRUD completo). Ele foi construído para servir de material de estudo em aula, demonstrando na prática os principais conceitos de Minimal APIs no .NET 10.

### Objetivos de aprendizagem

Ao estudar este projeto, você vai:

- Entender o que são Minimal APIs e quando faz sentido usá-las em vez de Controllers;
- Comparar, na prática, Controllers vs. Minimal APIs;
- Aplicar injeção de dependência por método (Method Injection);
- Organizar o código seguindo o padrão Vertical Slice Architecture (organização por Features);
- Implementar um CRUD completo com Entity Framework Core (banco em memória);
- Gerar e explorar documentação OpenAPI de forma interativa com o Scalar;
- Validar dados de entrada e padronizar respostas de erro com `ProblemDetails`.

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download) instalado;
- Visual Studio 2026, VS Code ou JetBrains Rider;
- Conhecimento básico de C# e HTTP (verbos GET/POST/PUT/DELETE e códigos de status).

### O que é uma Minimal API?

Minimal APIs são uma abordagem leve e direta para construir endpoints HTTP no ASP.NET Core. Elas eliminam a necessidade de herdar classes base (como `ControllerBase`) ou usar atributos complexos, mapeando rotas diretamente por meio de funções e métodos estáticos para maximizar a performance e reduzir o consumo de memória.

### Principais Diferenças e Vantagens: Controllers vs. Minimal APIs

| Aspecto | Controllers | Minimal APIs |
|---|---|---|
| **Arquitetura de Execução** | Dependem do pipeline pesado do MVC, baseado em reflexão (Reflection). | Interagem diretamente com as primitivas do Kestrel. |
| **Injeção de Dependência** | Exigem injeção via construtor para a classe inteira. | Utilizam injeção direta por método (Method Injection), instanciando apenas o serviço necessário para a rota. |
| **Performance e Native AOT** | Suporte parcial e mais custoso à compilação Ahead-of-Time. | Suporte nativo total à compilação AOT, com tempo de inicialização (cold start) reduzido. |
| **Organização de Código** | Geralmente organizados por camadas técnicas. | Permitem o padrão Vertical Slices (agrupamento por Features), mantendo rotas, modelos e DTOs coesos no mesmo contexto. |

### Swagger vs. Scalar

- **Swagger (Swashbuckle):** Padrão tradicional de documentação no ASP.NET Core. Teve seu suporte e atualizações descontinuados pelo mantenedor principal, o que levou a Microsoft a removê-lo como padrão nos templates recentes do .NET.
- **Scalar:** Interface moderna e leve baseada em OpenAPI. Oferece carregamento instantâneo, suporte nativo à versão 3.1 do OpenAPI e geração automática de exemplos de código de requisição (cURL, JavaScript, Python) direto na interface.

### Estrutura do projeto

```
GestaoAcademica.Api/
├── Data/
│   └── AppDbContext.cs        # Contexto do EF Core (banco em memória)
├── Features/
│   └── Alunos/
│       ├── Aluno.cs           # Entidade + DTOs (SalvarAlunoRequest, AlunoResponse)
│       └── AlunosEndpoint.cs  # Rotas e handlers do CRUD de Alunos
├── Program.cs                 # Composição da aplicação (pipeline, DI, middlewares)
├── appsettings.json
└── GestaoAcademica.Api.csproj
```

Repare que tudo relacionado à feature **Alunos** (modelo, DTOs e rotas) fica dentro da mesma pasta — essa é a ideia do Vertical Slice Architecture: organizar o código por funcionalidade de negócio, e não por camada técnica.

### Como executar o projeto

```bash
git clone https://github.com/fcrusco/Minimal-API.git
cd Minimal-API
dotnet restore
dotnet run
```

Com a aplicação rodando em ambiente de desenvolvimento, acesse a documentação interativa do Scalar em:

```
https://localhost:<porta>/scalar/v1
```

(a porta exata aparece no console ao rodar `dotnet run`, ou em `Properties/launchSettings.json`).

### Endpoints disponíveis

| Método | Rota | Descrição | Corpo da requisição | Respostas possíveis |
|---|---|---|---|---|
| `POST` | `/api/alunos` | Cria um novo aluno | `{ "nome": "...", "turma": "...", "periodo": "..." }` | `201 Created` / `400 ValidationProblem` |
| `GET` | `/api/alunos` | Lista todos os alunos | — | `200 OK` |
| `GET` | `/api/alunos/{id}` | Busca um aluno por ID | — | `200 OK` / `404 Not Found` |
| `PUT` | `/api/alunos/{id}` | Atualiza um aluno existente | `{ "nome": "...", "turma": "...", "periodo": "..." }` | `204 No Content` / `404 Not Found` / `400 ValidationProblem` |
| `DELETE` | `/api/alunos/{id}` | Remove um aluno | — | `204 No Content` / `404 Not Found` |

### Exemplos de uso (cURL)

**Criar um aluno**
```bash
curl -X POST https://localhost:<porta>/api/alunos \
  -H "Content-Type: application/json" \
  -d '{"nome":"Maria Silva","turma":"3A","periodo":"Matutino"}'
```

**Listar todos os alunos**
```bash
curl https://localhost:<porta>/api/alunos
```

**Buscar um aluno por ID**
```bash
curl https://localhost:<porta>/api/alunos/1
```

**Atualizar um aluno**
```bash
curl -X PUT https://localhost:<porta>/api/alunos/1 \
  -H "Content-Type: application/json" \
  -d '{"nome":"Maria Silva Santos","turma":"3A","periodo":"Matutino"}'
```

**Remover um aluno**
```bash
curl -X DELETE https://localhost:<porta>/api/alunos/1
```

### Validação e tratamento de erros

- Os campos `Nome`, `Turma` e `Periodo` são validados antes de qualquer acesso ao banco. Quando algum está vazio, a API responde `400 Bad Request` no formato `application/problem+json`, com um dicionário indicando qual campo falhou.
- Qualquer exceção não tratada no pipeline é capturada por um middleware global (`UseExceptionHandler`) e convertida em uma resposta `ProblemDetails` padronizada, em vez de vazar detalhes internos.


### Versões Utilizadas

- **IDE:** Visual Studio 2026
- **Runtime:** .NET 10.0
- **Microsoft.AspNetCore.OpenApi:** 10.0.11
- **Microsoft.EntityFrameworkCore.InMemory:** 10.0.11
- **Scalar.AspNetCore:** 2.17.2

<img width="435" height="645" alt="image" src="https://github.com/user-attachments/assets/4d4498ad-0acd-4fe3-be02-09c1b1f7b745" />

---

## 🇺🇸 English

### About the project

This project is a hands-on example of an Academic Management API, focused on Student (Aluno) registration with a full CRUD. It was built as teaching material, demonstrating in practice the main concepts of Minimal APIs in .NET 10.

### Learning objectives

By studying this project, you will:

- Understand what Minimal APIs are and when to use them instead of Controllers;
- Compare Controllers vs. Minimal APIs in practice;
- Apply dependency injection at the method level (Method Injection);
- Organize code following the Vertical Slice Architecture pattern (feature-based organization);
- Implement a full CRUD with Entity Framework Core (in-memory database);
- Generate and explore interactive OpenAPI documentation with Scalar;
- Validate input data and standardize error responses with `ProblemDetails`.

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) installed;
- Visual Studio 2026, VS Code, or JetBrains Rider;
- Basic knowledge of C# and HTTP (GET/POST/PUT/DELETE verbs and status codes).

### What is a Minimal API?

Minimal APIs are a lightweight, streamlined approach for building fast HTTP endpoints in ASP.NET Core. They eliminate the boilerplate of inheriting base classes (such as `ControllerBase`) or using heavy attributes, mapping routes directly using delegates and static methods to maximize performance and reduce memory footprint.

### Main Differences and Advantages: Controllers vs. Minimal APIs

| Aspect | Controllers | Minimal APIs |
|---|---|---|
| **Execution Pipeline** | Rely on the MVC reflection-heavy pipeline. | Interact closely with Kestrel primitives. |
| **Dependency Injection** | Require constructor injection for the entire class. | Support Method Injection, resolving only the specific dependencies required by the handler. |
| **Performance & Native AOT** | Partial, more costly Ahead-of-Time support. | Full native AOT support, with faster cold starts. |
| **Code Organization** | Usually organized by technical layer. | Naturally fit Vertical Slice Architecture (feature-based folders), keeping routes, models, and DTOs co-located. |

### Swagger vs. Scalar

- **Swagger (Swashbuckle):** The traditional documentation standard in ASP.NET Core. Its active maintenance slowed down/stopped, prompting Microsoft to drop it as the default in recent .NET templates.
- **Scalar:** A modern, lightweight OpenAPI-based interface. It features instant loading, native OpenAPI 3.1 support, and built-in code snippet generation for requests (cURL, JavaScript, Python) directly within the UI.

### Project structure

```
GestaoAcademica.Api/
├── Data/
│   └── AppDbContext.cs        # EF Core context (in-memory database)
├── Features/
│   └── Alunos/
│       ├── Aluno.cs           # Entity + DTOs (SalvarAlunoRequest, AlunoResponse)
│       └── AlunosEndpoint.cs  # Routes and handlers for the Alunos CRUD
├── Program.cs                 # App composition (pipeline, DI, middlewares)
├── appsettings.json
└── GestaoAcademica.Api.csproj
```

Notice that everything related to the **Alunos** feature (model, DTOs, and routes) lives in the same folder — that's the idea behind Vertical Slice Architecture: organizing code by business feature rather than by technical layer.

### Running the project

```bash
git clone https://github.com/fcrusco/Minimal-API.git
cd Minimal-API
dotnet restore
dotnet run
```

With the app running in the Development environment, open the interactive Scalar docs at:

```
https://localhost:<port>/scalar/v1
```

(the exact port is shown in the console when you run `dotnet run`, or in `Properties/launchSettings.json`).

### Available endpoints

| Method | Route | Description | Request body | Possible responses |
|---|---|---|---|---|
| `POST` | `/api/alunos` | Creates a new student | `{ "nome": "...", "turma": "...", "periodo": "..." }` | `201 Created` / `400 ValidationProblem` |
| `GET` | `/api/alunos` | Lists all students | — | `200 OK` |
| `GET` | `/api/alunos/{id}` | Retrieves a student by ID | — | `200 OK` / `404 Not Found` |
| `PUT` | `/api/alunos/{id}` | Updates an existing student | `{ "nome": "...", "turma": "...", "periodo": "..." }` | `204 No Content` / `404 Not Found` / `400 ValidationProblem` |
| `DELETE` | `/api/alunos/{id}` | Removes a student | — | `204 No Content` / `404 Not Found` |

### Usage examples (cURL)

**Create a student**
```bash
curl -X POST https://localhost:<port>/api/alunos \
  -H "Content-Type: application/json" \
  -d '{"nome":"Maria Silva","turma":"3A","periodo":"Matutino"}'
```

**List all students**
```bash
curl https://localhost:<port>/api/alunos
```

**Get a student by ID**
```bash
curl https://localhost:<port>/api/alunos/1
```

**Update a student**
```bash
curl -X PUT https://localhost:<port>/api/alunos/1 \
  -H "Content-Type: application/json" \
  -d '{"nome":"Maria Silva Santos","turma":"3A","periodo":"Matutino"}'
```

**Delete a student**
```bash
curl -X DELETE https://localhost:<port>/api/alunos/1
```

### Validation and error handling

- The `Nome`, `Turma`, and `Periodo` fields are validated before any database access. If any is empty, the API responds with `400 Bad Request` in `application/problem+json` format, with a dictionary indicating which field failed.
- Any unhandled exception in the pipeline is caught by a global middleware (`UseExceptionHandler`) and converted into a standardized `ProblemDetails` response, instead of leaking internal details.


### Versions Used

- **IDE:** Visual Studio 2026
- **Runtime:** .NET 10.0
- **Microsoft.AspNetCore.OpenApi:** 10.0.11
- **Microsoft.EntityFrameworkCore.InMemory:** 10.0.11
- **Scalar.AspNetCore:** 2.17.2

---

## 🇪🇸 Español

### Acerca del proyecto

Este proyecto es un ejemplo didáctico de una API de Gestión Académica, centrada en el registro de Alumnos con un CRUD completo. Fue creado como material de estudio para su uso en clase, demostrando en la práctica los principales conceptos de Minimal APIs en .NET 10.

### Objetivos de aprendizaje

Al estudiar este proyecto, aprenderás a:

- Entender qué son las Minimal APIs y cuándo tiene sentido usarlas en lugar de Controllers;
- Comparar, en la práctica, Controllers vs. Minimal APIs;
- Aplicar inyección de dependencias a nivel de método (Method Injection);
- Organizar el código siguiendo el patrón Vertical Slice Architecture (organización por Features);
- Implementar un CRUD completo con Entity Framework Core (base de datos en memoria);
- Generar y explorar documentación OpenAPI de forma interactiva con Scalar;
- Validar datos de entrada y estandarizar las respuestas de error con `ProblemDetails`.

### Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download) instalado;
- Visual Studio 2026, VS Code o JetBrains Rider;
- Conocimientos básicos de C# y HTTP (verbos GET/POST/PUT/DELETE y códigos de estado).

### ¿Qué es una Minimal API?

Las Minimal APIs son un enfoque ligero y directo para construir endpoints HTTP en ASP.NET Core. Eliminan la necesidad de heredar clases base (como `ControllerBase`) o utilizar atributos pesados, mapeando rutas directamente mediante funciones y métodos estáticos para maximizar el rendimiento y minimizar el consumo de memoria.

### Principales Diferencias y Ventajas: Controllers vs. Minimal APIs

| Aspecto | Controllers | Minimal APIs |
|---|---|---|
| **Pipeline de Ejecución** | Dependen del pipeline pesado de MVC, basado en reflexión. | Interactúan directamente con las primitivas de Kestrel. |
| **Inyección de Dependencias** | Requieren inyección por constructor para toda la clase. | Utilizan inyección directa por método (Method Injection), resolviendo únicamente el servicio necesario para la ruta. |
| **Rendimiento y Native AOT** | Soporte parcial y más costoso para compilación Ahead-of-Time. | Soporte nativo total para compilación AOT, con arranques (cold start) más rápidos. |
| **Organización del Código** | Generalmente organizados por capa técnica. | Facilitan la arquitectura de cortes verticales (Vertical Slices), agrupando rutas, modelos y DTOs en un mismo contexto de negocio. |

### Swagger vs. Scalar

- **Swagger (Swashbuckle):** El estándar de documentación tradicional en ASP.NET Core. Su mantenimiento activo disminuyó, lo que llevó a Microsoft a dejar de incluirlo por defecto en las plantillas recientes de .NET.
- **Scalar:** Una interfaz moderna y ligera basada en OpenAPI. Ofrece carga instantánea, compatibilidad nativa con OpenAPI 3.1 y generación automática de fragmentos de código para solicitudes (cURL, JavaScript, Python) directamente en la interfaz de usuario.

### Estructura del proyecto

```
GestaoAcademica.Api/
├── Data/
│   └── AppDbContext.cs        # Contexto de EF Core (base de datos en memoria)
├── Features/
│   └── Alunos/
│       ├── Aluno.cs           # Entidad + DTOs (SalvarAlunoRequest, AlunoResponse)
│       └── AlunosEndpoint.cs  # Rutas y handlers del CRUD de Alumnos
├── Program.cs                 # Composición de la aplicación (pipeline, DI, middlewares)
├── appsettings.json
└── GestaoAcademica.Api.csproj
```

Observa que todo lo relacionado con la feature **Alunos** (modelo, DTOs y rutas) está dentro de la misma carpeta — esa es la idea del Vertical Slice Architecture: organizar el código por funcionalidad de negocio, y no por capa técnica.

### Cómo ejecutar el proyecto

```bash
git clone https://github.com/fcrusco/Minimal-API.git
cd Minimal-API
dotnet restore
dotnet run
```

Con la aplicación ejecutándose en el entorno de desarrollo, accede a la documentación interactiva de Scalar en:

```
https://localhost:<puerto>/scalar/v1
```

(el puerto exacto aparece en la consola al ejecutar `dotnet run`, o en `Properties/launchSettings.json`).

### Endpoints disponibles

| Método | Ruta | Descripción | Cuerpo de la solicitud | Respuestas posibles |
|---|---|---|---|---|
| `POST` | `/api/alunos` | Crea un nuevo alumno | `{ "nome": "...", "turma": "...", "periodo": "..." }` | `201 Created` / `400 ValidationProblem` |
| `GET` | `/api/alunos` | Lista todos los alumnos | — | `200 OK` |
| `GET` | `/api/alunos/{id}` | Busca un alumno por ID | — | `200 OK` / `404 Not Found` |
| `PUT` | `/api/alunos/{id}` | Actualiza un alumno existente | `{ "nome": "...", "turma": "...", "periodo": "..." }` | `204 No Content` / `404 Not Found` / `400 ValidationProblem` |
| `DELETE` | `/api/alunos/{id}` | Elimina un alumno | — | `204 No Content` / `404 Not Found` |

### Ejemplos de uso (cURL)

**Crear un alumno**
```bash
curl -X POST https://localhost:<puerto>/api/alunos \
  -H "Content-Type: application/json" \
  -d '{"nome":"Maria Silva","turma":"3A","periodo":"Matutino"}'
```

**Listar todos los alumnos**
```bash
curl https://localhost:<puerto>/api/alunos
```

**Buscar un alumno por ID**
```bash
curl https://localhost:<puerto>/api/alunos/1
```

**Actualizar un alumno**
```bash
curl -X PUT https://localhost:<puerto>/api/alunos/1 \
  -H "Content-Type: application/json" \
  -d '{"nome":"Maria Silva Santos","turma":"3A","periodo":"Matutino"}'
```

**Eliminar un alumno**
```bash
curl -X DELETE https://localhost:<puerto>/api/alunos/1
```

### Validación y manejo de errores

- Los campos `Nome`, `Turma` y `Periodo` se validan antes de cualquier acceso a la base de datos. Si alguno está vacío, la API responde `400 Bad Request` en formato `application/problem+json`, con un diccionario que indica qué campo falló.
- Cualquier excepción no controlada en el pipeline es capturada por un middleware global (`UseExceptionHandler`) y convertida en una respuesta `ProblemDetails` estandarizada, en lugar de exponer detalles internos.


### Versiones Utilizadas

- **IDE:** Visual Studio 2026
- **Runtime:** .NET 10.0
- **Microsoft.AspNetCore.OpenApi:** 10.0.11
- **Microsoft.EntityFrameworkCore.InMemory:** 10.0.11
- **Scalar.AspNetCore:** 2.17.2
