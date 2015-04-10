using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cit.Globosat.Remuneracao.Formularios.Entidades
{
    [Serializable]
    public class AltFuncCargo
    {
        public string LogoImageUrl { get; set; }
        public string CentroCusto { get; set; }
        public string Funcionario { get; set; }
        public string DataRequisicao { get; set; }
        public string Diretoria { get; set; }
        public string Matricula { get; set; }
        public string DataAdmissao { get; set; }
        public bool TransfCentroCusto { get; set; }
        public string Para { get; set; }
        public string Filial { get; set; }
        public int Motivo { get; set; }
        public DateTime AltValidaPartirDe { get; set; }
        public string CodigoCargoAtual { get; set; }
        public string CargoAtual { get; set; }
        public string SalarioAtual { get; set; }
        public string SalarioNivelAtual { get; set; }
        public string Diferenca { get; set; }
        public string CodigoCargoProposto { get; set; }
        public string CargoProposto { get; set; }
        public string NovaJornada { get; set; }
        public string NovaJornadaDiferenteAtual { get; set; }
        public string SalarioProposto { get; set; }
        public string ClasseProposto { get; set; }
        public string NivelProposto { get; set; }
        public string PorcentagemAumento { get; set; }
        public List<DadosRemuneracao> Historico = new List<DadosRemuneracao>();
        public string Justificativa { get; set; }
        public string Nome { get; set; }
    }

    public class Historico
    {
    }
}
