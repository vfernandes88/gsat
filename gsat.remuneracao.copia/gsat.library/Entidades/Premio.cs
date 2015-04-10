using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Globosat.Library.Entidades
{
    public class Premio
    {
        public int Ano { get; set; }
        public int Mes { get; set; }
        public string Evento { get; set; }
        public decimal Valor { get; set; }
        public decimal TotalMes { get; set; }
        public decimal TotalEvento { get; set; }
        public decimal Total { get; set; }
        public string Matricula { get; set; }
        public string Nome { get; set; }
    }
}
