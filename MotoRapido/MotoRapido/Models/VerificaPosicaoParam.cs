using System;
using System.Collections.Generic;
using System.Text;

namespace MotoRapido.Models
{
    public class VerificaPosicaoParam
    {

        public String latitude { get; set; }

        public String longitude { get; set; }

        public int codMotorista { get; set; }

        public int codUltimaArea { get; set; }

        public double direcao { get; set; }

        public String loginMotorista { get; set; }

        public Boolean corridaRecusada { get; set; }

        public Boolean corridaPendenteRecusada { get; set; }
    }
}
