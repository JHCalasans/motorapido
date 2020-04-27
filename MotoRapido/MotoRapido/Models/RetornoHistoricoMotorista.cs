using System;

namespace MotoRapido.Models
{
    public class RetornoHistoricoMotorista
    {
        public DateTime dataChamada { get; set; }

        public String tipoVeiculo { get; set; }

        public String placa { get; set; }

        public String situacao { get; set; }

        public String destino { get; set; }

        public String valor { get; set; }

        public bool showDestino
        {
            get { return situacao.Equals("FINALIZADA"); }
        }
    }
}
