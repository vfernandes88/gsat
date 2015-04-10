using System;
using System.Collections.Generic;
using System.Text;

namespace Globosat.Library.Entidades
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
        string _nivel;
        public string Nivel
        {
            get { return _nivel; }
            set { _nivel = value; }
        }

        private string _foto;

        public string Foto
        {
            get { return _foto; }
            set { _foto = value; }
        }

        private string _dtNascimento;
        public string DtNascimento
        {
            get { return _dtNascimento; }
            set { _dtNascimento = value; }
        }
                   
    }
}
