using System;

namespace MotoRapido.Models
{
    public class RetornoVerificaPosicao 
    {

        public RetornoVerificaPosicao()
        {

        }

        public RetornoVerificaPosicao(String msgErro)
        {
            this.msgErro = msgErro;
        }
        public Area areaAtual { get; set; }

        public Int32 posicaoNaArea { get; set; }

        public int codMotorista { get; set; }
        public String informacaoPosicao
        {
            get => msgErro != null ? msgErro : "Pos: " + posicaoNaArea + "  -  Área:" + areaAtual.descricao;
            
        }

        public String msgErro { get; set; }
    }
}
