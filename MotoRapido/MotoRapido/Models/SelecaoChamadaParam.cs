using System;
using System.Collections.Generic;
using System.Text;

namespace MotoRapido.Models
{
    public class SelecaoChamadaParam
    {
        public Chamada chamada { get; set; }

        public DateTime dataDecisao { get; set; }

        public Int32 codChamadaVeiculo { get; set; }

        public DateTime dataRecebimento { get; set; }

        public DateTime inicioCorrida { get; set; }

        public int codVeiculo { get; set; }

        public String latitudeAtual { get; set; }

        public String longitudeAtual { get; set; }

    }
}
