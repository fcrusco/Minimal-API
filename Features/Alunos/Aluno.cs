namespace GestaoAcademica.Api.Features.Alunos; // Define o namespace que agrupa os componentes da feature de Alunos

public class Aluno // Declara a classe de entidade de domínio que representa o aluno na base de dados
{
    public int Id { get; set; } // Propriedade que atua como chave primária com ID incremental gerado pelo banco
    public string Nome { get; set; } = string.Empty; // Propriedade para armazenar o nome do aluno, inicializada vazia para evitar nulos
    public string Turma { get; set; } = string.Empty; // Propriedade para armazenar a turma correspondente do aluno
    public string Periodo { get; set; } = string.Empty; // Propriedade para armazenar o período escolar (ex: Matutino, Noturno)

    // DTO de entrada: record imutável estruturado apenas com os dados que a API precisa receber no POST ou PUT
    public record SalvarAlunoRequest(string Nome, string Turma, string Periodo);

    // DTO de saída: record imutável usado para formatar o contrato de retorno, blindando a entidade interna
    public record AlunoResponse(int Id, string Nome, string Turma, string Periodo);
}