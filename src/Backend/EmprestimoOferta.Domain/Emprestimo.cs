using EmprestimoOferta.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmprestimoOferta.Domain
{
    public class Emprestimo
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid ClienteId { get; private set; }
        public decimal Valor { get; private set; } // Valor do empréstimo
        public int QuantidadeParcelas { get; private set; } // Número de parcelas
        public decimal TaxaJuros { get; private set; } // Taxa de juros aplicada
        public DateTime DataInicio { get; private set; } // Data de início do empréstimo
        public TipoJuros TipoJuros { get; private set; } = TipoJuros.Composto; // Tipo de juros (Simples ou Composto)


        public enum StatusEmprestimo
        {
            Ativo,
            Pago,
            Atrasado
        }

        public StatusEmprestimo Status { get; private set; } = StatusEmprestimo.Ativo; // Status do empréstimo

        // Propriedades adicionaispara controle de pagamento

        public decimal ValorTotalComJuros { get; private set; } // Valor total a ser pago com juros
        public DateTime VencimentoParcela { get; private set; } // Data de vencimento da parcela
        public bool Atrasado { get; private set; } // Indica se o empréstimo está atrasado

        public List<Pagamento> Pagamentos { get; private set; } = new(); // Lista de pagamentos associados ao empréstimo


        protected Emprestimo() { } // Construtor protegido para o EF Core

        public Emprestimo(Guid clienteId, decimal valor, int quantidadeParcelas, decimal taxaJuros, DateTime dataInicio, TipoJuros tipoJuros)
        {
            if (clienteId == Guid.Empty)
                throw new ArgumentException("ClienteId é obrigatório.");
            if (valor <= 0)
                throw new ArgumentException("Valor do empréstimo deve ser maior que zero.");
            if (quantidadeParcelas <= 0)
                throw new ArgumentException("Quantidade de parcelas deve ser maior que zero.");
            if (taxaJuros < 0)
                throw new ArgumentException("Taxa de juros não pode ser negativa.");

            ClienteId = clienteId;
            Valor = valor;
            QuantidadeParcelas = quantidadeParcelas;
            TaxaJuros = taxaJuros;
            DataInicio = dataInicio;
            TipoJuros = tipoJuros;

            ValorTotalComJuros = CalcularValorTotalComJuros();
            VencimentoParcela = CalcularVencimentoParcela();
        }


        // Método para calcular o valor total com juros
        private decimal CalcularValorTotalComJuros()
        {
            if (TipoJuros == TipoJuros.Simples)
            {
                return Math.Round(Valor + (Valor * TaxaJuros / 100 * QuantidadeParcelas), 2);
            }
            else // Composto
            {
                var fator = (decimal)Math.Pow((double)(1 + TaxaJuros / 100), QuantidadeParcelas);
                return Math.Round(Valor * fator, 2);
            }
        }



        // Método para calcular a data de vencimento da primeira parcela
        private DateTime CalcularVencimentoParcela()
        {
            // Considerando que as parcelas vencem mensalmente
            return DataInicio.AddMonths(1);
        }

        // Método para atualizar o status do empréstimo
        public void AtualizarStatus(StatusEmprestimo novoStatus)
        {
            Status = novoStatus;
            if (novoStatus == StatusEmprestimo.Pago)
            {
                Atrasado = false; // Se o empréstimo foi pago, não pode estar atrasado
            }
        }

        // Método para verificar se o empréstimo está atrasado
        public void VerificarAtraso(DateTime dataAtual)
        {
            if (dataAtual > VencimentoParcela && Status == StatusEmprestimo.Ativo)
            {
                Atrasado = true;
            }
            else
            {
                Atrasado = false;
            }
        }

        // Método para calcular o valor da parcela
        public decimal CalcularValorParcela()
        {
            if (QuantidadeParcelas <= 0)
                throw new InvalidOperationException("Quantidade de parcelas deve ser maior que zero.");
            return ValorTotalComJuros / QuantidadeParcelas;
        }

        // Metdo para Registrar o pagamento de uma parcela
        public void RegistrarPagamento(DateTime dataPagamento, decimal valorPago)
        {
            if (Status == StatusEmprestimo.Pago)
                throw new InvalidOperationException("Este empréstimo já foi quitado.");

            bool estaAtrasado = dataPagamento > VencimentoParcela;
            Atrasado = estaAtrasado;

            decimal juros = 0;
            if (estaAtrasado)
            {
                // Exemplo: 2% de juros por mês de atraso
                var mesesAtraso = ((dataPagamento.Year - VencimentoParcela.Year) * 12) + dataPagamento.Month - VencimentoParcela.Month;
                mesesAtraso = Math.Max(mesesAtraso, 1); // Garante pelo menos 1 mês de juros
                juros = Valor * TaxaJuros / 100 * mesesAtraso;
            }

            var pagamento = new Pagamento(Id, dataPagamento, valorPago, juros);
            Pagamentos.Add(pagamento);

            if (QuantidadeParcelas == 1)
            {
                AtualizarStatus(StatusEmprestimo.Pago);
            }
            else
            {
                QuantidadeParcelas--;
                VencimentoParcela = VencimentoParcela.AddMonths(1);
            }
        }


    }
}
