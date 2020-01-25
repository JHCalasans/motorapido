using System;

namespace MotoRapido.Models
{
    public class MensagemMotoristaFuncionario
    {

        public Int64 codigo { get; set; }

        public String descricao { get; set; }

        public DateTime dataCriacao { get; set; }

        public Motorista motorista { get; set; }

        public String enviadaPorMotorista { get; set; }

    }
}
