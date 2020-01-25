using MotoRapido.Models;
using System.Collections.Generic;

namespace MotoRapido.BD.Repositorio
{
    public class MessageRepositorio : IMessageRepositorio
    {
        DataBase _dataBase;

        public MessageRepositorio()
        {
            _dataBase = new DataBase();
        }

        public void DeletarMensagem(Message mensagem)
        {
            _dataBase.DeletarMensagem(mensagem);
        }

        public void GravarMensagem(Message mensagem)
        {
            _dataBase.GravarMensagem(mensagem);
        }

        public List<Message> ObterTodasAsMensagens()
        {
           return  _dataBase.ObterTodasAsMensagens();
        }
    }
}
