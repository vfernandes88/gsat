using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cit.Globosat.Remuneracao.Formularios.Entidades
{
    public class DadosProfile
    {
        string _coligada;
        public string Coligada
        {
            get { return _coligada; }
            set { _coligada = value; }
        }

        string _matricula;
        public string Matricula
        {
            get { return _matricula; }
            set { _matricula = value; }
        }

        string _classe;
        public string Classe
        {
            get { return _classe; }
            set { _classe = value; }
        }

        int _faixaSalarial;
        public int FaixaSalarial
        {
            get { return _faixaSalarial; }
            set { _faixaSalarial = value; }
        }

        private String _CentroCusto;

        public String CentroCusto
        {
            get { return _CentroCusto; }
            set { _CentroCusto = value; }
        }
    }
}
