using MotoRapido.Models;
using System;
using System.Collections.Generic;

namespace MotoRapido.BD.Repositorio
{
    public interface IInformacaoPendenteRepositorio
    {
        List<InformacaoPendente> ObterTodosInformacaoPendentes();

        InformacaoPendente ObterInformacaoPendente(Int64 codInformacaoPendente);

        void AdicionarInformacaoPendente(InformacaoPendente informacaoPendente);

        void EditarInformacaoPendente(InformacaoPendente informacaoPendente);

        void DeletarInformacaoPendente(Int64 codInformacaoPendente);

        void DeletarTodosInformacaoPendentes();
    }
}
