using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cit.Globosat.Remuneracao.Formularios.Entidades
{
    [Serializable]
    public class ReqPessoalEstag
    {
        public string LogoImageUrl { get; set; }
        public string EstagioEm { get; set; }
        public DateTime DataRequisicao { get; set; }
        public string EmSubstituicao { get; set; }
        public string Diretoria { get; set; }
        public string DepartamentoArea { get; set; }
        public string CodigoCentroCusto { get; set; }
        public string ValorBolsaAuxilio { get; set; }
        public string StatusVaga { get; set; }
        public string Orcado { get; set; }
        public string Justificativa { get; set; }
        public string EstudanteCurso { get; set; }
        public string Periodo { get; set; }
        public string Nivel { get; set; }
        public string HorarioEstagio { get; set; }
        public string SupervisorEstagio { get; set; }
        public string Cargo { get; set; }
        public string FormacaoProfissional { get; set; }
        public string Requisitante { get; set; }
        public string DiretoriaArea { get; set; }
        public string RecursosHumanos { get; set; }
        public string DiretoriaGestao { get; set; }
    }
}
