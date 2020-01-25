using SQLite;
using System;

namespace MotoRapido.Models
{
    [Table("InformacaoPendente")]
    public class InformacaoPendente
    {
        [PrimaryKey, AutoIncrement]
        public Int64 codInformacaoPendente { get; set; }
        public DateTime dtHora { get; set; }
        public String conteudo { get; set; }
        public String servico { get; set; }
    }
}
