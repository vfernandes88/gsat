using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Globosat.Library.Entidades
{
    public class Gerente
    {
        string _matricula;
        public string Matricula
        {
            get { return _matricula; }
            set { _matricula = value; }
        }

        string _coligada;
        public string Coligada
        {
            get { return _coligada; }
            set { _coligada = value; }
        }
        
        string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
    }
}
