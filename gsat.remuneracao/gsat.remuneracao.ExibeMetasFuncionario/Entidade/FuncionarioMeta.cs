using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Globosat.Remuneracao.ExibeMetasFuncionario.WPExibeMetasFuncionario
{
    class FuncionarioMeta
    {
        private string _nome;
        public string Nome
        {
            get { return _nome; }
            set { _nome = value; }           
        }
        private string _cargo;
        public string Cargo
        {
            get { return _cargo; }
            set { _cargo = value; }
        }
        private string _classe;
        public string Classe
        {
            get { return _classe; }
            set { _classe = value; }
        }
        private double _participe;
        public double Participe
        {
            get { return _participe; }
            set { _participe = value; }
        }
        private double _bonus;
        public double Bonus
        {
            get { return _bonus; }
            set { _bonus = value; }
        }
        private string _matricula;
        public string Matricula
        {
            get { return _matricula; }
            set { _matricula = value; }
        }
        private string _notaDiretoria;
        public string NotaDiretoria
        {
            get { return _notaDiretoria; }
            set { _notaDiretoria = value; }
        }
        private string _notaProjetosNegociados;
        public string ProjetosNegociados
        {
            get { return _notaProjetosNegociados; }
            set { _notaProjetosNegociados = value; }
        }
        private string _notaAno;
        public string Ano
        {
            get { return _notaAno; }
            set { _notaAno = value; }
        }

    }
}
