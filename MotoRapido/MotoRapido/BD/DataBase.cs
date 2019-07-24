﻿using MotoRapido.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace MotoRapido.BD
{
    public class DataBase
    {
        private static SQLiteConnection _conexao;

        public DataBase()
        {
            var dependencyService = DependencyService.Get<IConexaoBancoLocal>();

            string stringConexao = dependencyService.Conexao("contatos.sqlite");

            _conexao = new SQLiteConnection(stringConexao);

            _conexao.CreateTable<InformacaoPendente>();
        }

        public void AdicionarInformacaoPendente(InformacaoPendente informacaoPendente)
        {
            _conexao.Insert(informacaoPendente);
        }

        public InformacaoPendente ObterInformacaoPendente(Int64 codInformacaoPendente)
        {
            return _conexao.Table<InformacaoPendente>().FirstOrDefault(c => c.codInformacaoPendente == codInformacaoPendente);
        }

        public List<InformacaoPendente> ObterTodosInformacaoPendentes()
        {
            return (from InformacaoPendente in _conexao.Table<InformacaoPendente>() select InformacaoPendente).ToList();
        }

        public void EditarInformacaoPendente(InformacaoPendente informacaoPendente)
        {
            _conexao.Update(informacaoPendente);
        }

        public void DeletarInformacaoPendente(Int64 codInformacaoPendente)
        {
            _conexao.Delete<InformacaoPendente>(codInformacaoPendente);
        }

        public void DeletarTodosInformacaoPendentes()
        {
            _conexao.DeleteAll<InformacaoPendente>();
        }
    }
}