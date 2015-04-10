using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Globosat.Remuneracao.FieldType.CCustoCargo
{
    public class Controles
    {
        /// <summary>
        /// Encontra um controle procurando pelo ID recursivamente
        /// </summary>
        /// <param name="controleAtual">Controle onde inicia a busca</param>
        /// <param name="id">ID do controle a ser encontrado</param>
        /// <returns></returns>
        public static Control EncontraControleRecursivo(Control controleAtual, string id)
        {
            if (controleAtual.ID == id)
                return controleAtual;

            foreach (Control controleDaVez in controleAtual.Controls)
            {
                Control controleEncontrado = EncontraControleRecursivo(controleDaVez, id);
                if (controleEncontrado != null)
                    return controleEncontrado;
            }

            return null;
        }
        /// <summary>
        /// Encontra um controle procurando por um tipo recursivamente
        /// </summary>
        /// <param name="controleAtual">Controle onde inicia a busca</param>
        /// <param name="tipo">Tipo do controle a ser encontrado</param>
        /// <returns></returns>
        public static Control EncontraTipoControleRecursivo(Control controleAtual, string tipo)
        {
            if (controleAtual.GetType().Name == tipo)
                return controleAtual;

            foreach (Control controleDaVez in controleAtual.Controls)
            {
                Control controleEncontrado = EncontraTipoControleRecursivo(controleDaVez, tipo);
                if (controleEncontrado != null)
                    return controleEncontrado;
            }

            return null;
        }

    }
}
