using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cit.Globosat.Remuneracao.Formularios.Entidades
{
    [Serializable]
    public class ReqPessoal
    {
        public string LogoImageUrl { get; set; }
        public string CentroCusto { get; set; }
        public string Funcionario { get; set; }
        public string DataRequisicao { get; set; }
        public string Diretoria { get; set; }
        public DateTime DataInicio { get; set; }
        public string DataAdmissao { get; set; }
        public bool TransfCentroCusto { get; set; }
        public string Para { get; set; }
        public string Filial { get; set; }
        public string Motivo { get; set; }
        public string TipoContrato { get; set; }
        public string TipoVaga { get; set; }
        public string CandidatoSelecionado { get; set; }
        public string Orcado { get; set; }
        public string Salario { get; set; }
        public string Classe { get; set; }
        public string Nivel { get; set; }
        public string Jornada { get; set; }
        public string Observacao { get; set; }
        public string ResumoAtribuicoes { get; set; }
        public string Justificativa { get; set; }
        public string ParecerRH { get; set; }
        public string ParecerRemuneracao { get; set; }
        public string Requisitante { get; set; }
        public string CargoAPreecher { get; set; }
        public DateTime PrazoDeterminado { get; set; }
        public DateTime PrazoTemporario { get; set; }
        public DateTime DataAssRequisitante { get; set; }
        public DateTime DataAssDiretoriaArea { get; set; }
        public DateTime DataAssRH { get; set; }
        public DateTime DataAssDiretoriaGestao { get; set; }
    }
}
