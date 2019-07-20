
using MotoRapido.BD;
using MotoRapido.Droid.BD;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(ConexaoBancoLocal))]
namespace MotoRapido.Droid.BD
{
    public class ConexaoBancoLocal : IConexaoBancoLocal
    {
        public string Conexao(string NomeArquivoBD)
        {
            string stringConexao = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string bancoDados = Path.Combine(stringConexao, NomeArquivoBD);
            return bancoDados;
        }
    }
}