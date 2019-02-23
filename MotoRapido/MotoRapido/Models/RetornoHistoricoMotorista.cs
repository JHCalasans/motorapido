using System;
using System.Collections.Generic;
using System.Text;

namespace MotoRapido.Models
{
    public class RetornoHistoricoMotorista
    {
        public DateTime dataChamada { get; set; }

        public String tipoVeiculo { get; set; }

        public String placa { get; set; }

        public String situacao { get; set; }
    }
}
