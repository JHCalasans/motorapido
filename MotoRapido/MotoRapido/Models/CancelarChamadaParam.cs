using System;
using System.Collections.Generic;
using System.Text;

namespace MotoRapido.Models
{
    public class CancelarChamadaParam
    {

        public Chamada chamada { get; set; }

        public DateTime dataCancelamento { get; set; }

        public Int32 codChamadaVeiculo { get; set; }

        public int codVeiculo { get; set; }

        public String latitudeAtual { get; set; }

        public String longitudeAtual { get; set; }
    }
}
