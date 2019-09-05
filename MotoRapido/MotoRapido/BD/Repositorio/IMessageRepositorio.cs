using MotoRapido.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MotoRapido.BD.Repositorio
{
    public interface IMessageRepositorio
    {
        List<Message> ObterTodasAsMensagens();

        void GravarMensagem(Message mensagem);
    }
}
