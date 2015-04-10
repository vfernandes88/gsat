using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Globosat.Remuneracao.SimRemTotal
{
    public class EixosGrafico
    {
        private string _x;
                        
        public string X
        {
            get { return _x; }
            set { _x = value; }
        }
        private decimal _y;

        public decimal Y
        {
            get { return _y; }
            set { _y = value; }
        }

    }
}
