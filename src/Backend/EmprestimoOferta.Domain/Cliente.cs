using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmprestimoOferta.Domain
{
    public class Cliente
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string NomeCompleto { get; private set; } = string.Empty;
        public string Documento { get; private set; } = string.Empty; // Pode ser CPF, CNPJ ou outro identificador único
        public string Email { get; private set; } = string.Empty;
        public string? Telefone { get; private set; }
        public string? EnderecoResidencial { get; private set; }
        public string? EnderecoTrabalho { get; private set; }
        public string? ChavePix { get; private set; }
        public string? InformacoesExtras { get; private set; }

        // Construtor protegido para o EF Core
        protected Cliente() { }

        public Cliente(string nomeCompleto, string documento, string email)
        {
            if (string.IsNullOrWhiteSpace(nomeCompleto))
                throw new ArgumentException("Nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(documento))
                throw new ArgumentException("Documento é obrigatório.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email é obrigatório.");

            NomeCompleto = nomeCompleto;
            Documento = documento;
            Email = email;
        }

        // Métodos para atualizar dados
        public void AtualizarContato(string? telefone, string? email)
        {
            Telefone = telefone;
            if (!string.IsNullOrWhiteSpace(email))
                Email = email;
        }

        public void AtualizarEnderecos(string? residencial, string? trabalho)
        {
            EnderecoResidencial = residencial;
            EnderecoTrabalho = trabalho;
        }

        public void DefinirChavePix(string? chavePix)
        {
            ChavePix = chavePix;
        }

        public void AdicionarInformacoesExtras(string? info)
        {
            InformacoesExtras = info;
        }

    }

}
