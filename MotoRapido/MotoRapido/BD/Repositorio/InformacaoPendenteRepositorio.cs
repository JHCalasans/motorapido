using MotoRapido.Models;
using System.Collections.Generic;

namespace MotoRapido.BD.Repositorio
{
    public class InformacaoPendenteRepositorio : IInformacaoPendenteRepositorio
    {
        DataBase _dataBase;

        public InformacaoPendenteRepositorio()
        {
            _dataBase = new DataBase();
        }

        public void AdicionarInformacaoPendente(InformacaoPendente informacaoPendente)
        {
            _dataBase.AdicionarInformacaoPendente(informacaoPendente);
        }

        public void DeletarInformacaoPendente(long codInformacaoPendente)
        {
            _dataBase.DeletarInformacaoPendente(codInformacaoPendente);
        }

        public void DeletarTodosInformacaoPendentes()
        {
            _dataBase.DeletarTodosInformacaoPendentes();
        }

        public void EditarInformacaoPendente(InformacaoPendente informacaoPendente)
        {
            _dataBase.EditarInformacaoPendente(informacaoPendente);
        }

        public InformacaoPendente ObterInformacaoPendente(long codInformacaoPendente)
        {
            return _dataBase.ObterInformacaoPendente(codInformacaoPendente);
        }

        public List<InformacaoPendente> ObterTodosInformacaoPendentes()
        {
            return _dataBase.ObterTodosInformacaoPendentes();
        }
    }
}
