using System;
using System.Collections.Generic;
using System.Text;

namespace MotoRapido.BD
{
    public interface IConexaoBancoLocal
    {
        string Conexao(string NomeArquivoBD);
    }
}
