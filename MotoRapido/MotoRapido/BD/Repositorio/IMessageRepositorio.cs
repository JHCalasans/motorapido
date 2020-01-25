using MotoRapido.Models;
using System.Collections.Generic;

namespace MotoRapido.BD.Repositorio
{
    public interface IMessageRepositorio
    {
        List<Message> ObterTodasAsMensagens();
        void GravarMensagem(Message mensagem);
        void DeletarMensagem(Message mensagem);
    }
}
