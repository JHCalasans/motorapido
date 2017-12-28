using System;
using System.Collections.Generic;
using System.Text;

namespace MotoRapido.Models
{
    public class Motorista
    {
        public int codigo{ get; set; }

        public String nome{ get; set; }

        public String identidade{ get; set; }

        public String cpf{ get; set; }

        public String senha{ get; set; }

        public String login{ get; set; }

        public String ativo{ get; set; }

        public DateTime dataCriacao{ get; set; }

        public DateTime dataDesativacao{ get; set; }

        public String celular{ get; set; }

        public String logradouro{ get; set; }

        public String bairro{ get; set; }

        public String cep{ get; set; }

        public String cidadeResidencia{ get; set; }

        public String estadoResidencia{ get; set; }

        public String agencia{ get; set; }

        public String conta{ get; set; }

        public String banco{ get; set; }

        public String comprovanteResidencial{ get; set; }

        public String email{ get; set; }

        public byte[] foto{ get; set; }

        public DateTime dataNascimento{ get; set; }

        public String cnh{ get; set; }

        public DateTime dataVencimentoCNH{ get; set; }

        public byte[] docCriminais{ get; set; }

        public String disponivel{ get; set; }

        public String chaveServicos { get; set; }
    }
}
