using System;
using System.Collections.Generic;
using System.Text;

namespace MotoRapido.Models
{
    public class RetornoVerificaPosicao
    {
        public Area areaAtual { get; set; }

        public Int32 posicaoNaArea { get; set; }


        public String informacaoPosicao
        {
            get => "Pos: " + posicaoNaArea + "  -  Área:" + areaAtual.descricao;
        }
    }
}
