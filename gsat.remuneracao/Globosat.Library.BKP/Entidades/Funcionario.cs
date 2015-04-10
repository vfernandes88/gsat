using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Globosat.Library.Entidades
{
    public class Funcionario
    {
        string _matricula;
        public string Matricula
        {
            get { return _matricula; }
            set { _matricula = value; }
        }

        string _nome;
        public string Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        string _data;
        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        string _salario;
        public string Salario
        {
            get { return _salario; }
            set { _salario = value; }
        }

        decimal _salarioNumber;
        public decimal SalarioNumber
        {
            get { return _salarioNumber; }
            set { _salarioNumber = value; }
        }

        string _percentual;
        public string Percentual
        {
            get { return _percentual; }
            set { _percentual = value; }
        }

        decimal _percentualNumber;
        public decimal PercentualNumber
        {
            get { return _percentualNumber; }
            set { _percentualNumber = value; }
        }

        string _motivo;
        public string Motivo
        {
            get { return _motivo; }
            set { _motivo = value; }
        }

        string _funcao;
        public string Funcao
        {
            get { return _funcao; }
            set { _funcao = value; }
        }

        string _classe;
        public string Classe
        {
            get { return _classe; }
            set { _classe = value; }
        }

        string _nivel;
        public string Nivel
        {
            get { return _nivel; }
            set { _nivel = value; }
        }

        string _foto;
        public string Foto
        {
            get { return _foto; }
            set { _foto = value; }
        }

        string _admissao;
        public string Admissao
        {
            get { return _admissao; }
            set { _admissao = value; }
        }
        string _dtNascimento;
        public string DtNascimento
        {
            get { return _dtNascimento; }
            set { _dtNascimento = value; }
        }

    }
}
