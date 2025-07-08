using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmprestimoOferta.Domain
{
    public class Pagamento
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public Guid EmprestimoId { get; private set; }
        public DateTime DataPagamento { get; private set; }
        public decimal ValorPago { get; private set; }
        public decimal JurosPorAtraso { get; private set; }
        public decimal TotalComJuros => ValorPago + JurosPorAtraso;

        // Construtor protegido para o EF Core
        protected Pagamento() { }

        public Pagamento(Guid emprestimoId, DateTime dataPagamento, decimal valorPago, decimal jurosPorAtraso)
        {
            if (emprestimoId == Guid.Empty)
                throw new ArgumentException("Empréstimo inválido.");

            if (valorPago <= 0)
                throw new ArgumentException("Valor do pagamento deve ser maior que zero.");

            EmprestimoId = emprestimoId;
            DataPagamento = dataPagamento;
            ValorPago = valorPago;
            JurosPorAtraso = jurosPorAtraso;
        }
    }