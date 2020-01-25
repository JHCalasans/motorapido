using System;

namespace MotoRapido.Models
{
    public class RetornoVeiculosMotorista
    {
        public String tipoVeiculo { get; set; }

        public String placa{ get; set; }

        public String modelo{ get; set; }

        public int codVeiculo { get; set; }

        public String veiculoFormatado
        { 
            get { return tipoVeiculo + "( " + modelo + " : " + placa + " )"; }
        }
    }
}
