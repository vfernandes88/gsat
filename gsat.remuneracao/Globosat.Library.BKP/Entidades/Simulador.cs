using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Globosat.Library.Entidades
{
    public class Simulador
    {
        private string _nomeFuncionario;            
        public string NomeFuncionario
        {
            get { return _nomeFuncionario; }
            set { _nomeFuncionario = value; }
        }
        
        private string _classe;
        public string Classe
        {
            get { return _classe; }
            set { _classe = value; }
        }
        private decimal _salarioBase;
        public decimal SalarioBase
        {
            get { return _salarioBase; }
            set { _salarioBase = value; }
        }
        private decimal _ferias13Mensal;
        public decimal Ferias31Mensal
        {
            get { return _ferias13Mensal; }
            set { _ferias13Mensal = value; }
        }
        private decimal _ferias13Anual;
        public decimal Ferias31Anual
        {
            get { return _ferias13Anual; }
            set { _ferias13Anual = value; }
        }
        private decimal _remuneracaoVariavel;
        public decimal RemuneracaoVariavel
        {
            get { return _remuneracaoVariavel; }
            set { _remuneracaoVariavel = value; }
        }
        private decimal _planoSaude;
        public decimal PlanoSaude
        {
            get { return _planoSaude; }
            set { _planoSaude = value; }
        }
        private decimal _planoOdontologico;
        public decimal PlanoOdontologico
        {
            get { return _planoOdontologico; }
            set { _planoOdontologico = value; }
        }
        private int _dependentes;

        public int Dependentes
        {
            get { return _dependentes; }
            set { _dependentes = value; }
        }

        private decimal _pensePrev;
        public decimal PensePrev
        {
            get { return _pensePrev; }
            set { _pensePrev = value; }
        }
        private decimal _remuneracaoFixa;
        public decimal RemuneracaoFixa
        {
            get { return _remuneracaoFixa; }
            set { _remuneracaoFixa = value; }
        }
        private decimal _remuneracaoDireta;
        public decimal RemuneracaoDireta
        {
            get { return _remuneracaoDireta; }
            set { _remuneracaoDireta = value; }
        }
        private decimal _remuneracaoTotal;
        public decimal RemuneracaoTotal
        {
            get { return _remuneracaoTotal; }
            set { _remuneracaoTotal = value; }
        }
    }
}
