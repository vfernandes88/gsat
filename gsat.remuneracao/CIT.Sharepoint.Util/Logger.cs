using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.SharePoint;

namespace CIT.Sharepoint.Util
{
    public enum EventID: int
    {
        Remuneracao = 2,
        Megafone = 3, 
        ContraCheque = 4,
        Convenios = 5, 
        InformeRedimentos = 6,
        SistemaFerias = 7,
        WPGadgetPerfil = 8,
        Promocoes = 9
    }

    public enum CategoryID : short
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }


    public class Logger
    {
        // Tamanho máximo da informação a ser logada no Event Viewer, em bytes
        private const int MAX_SIZE = 32766;

        //Nome da Source do Portal Internet
        private const string SOURCE = "Sharepoint";

        /// <summary>
        /// Grava uma exceção no Event Viewer.
        /// </summary>
        /// <param name="info">Descrição da exceção.</param>
        /// <param name="eventLogEntryType">Tipo da exceção.</param>
        /// <param name="eventID">ID do evento do erro.</param>
        /// <param name="categoryID">ID da categoria do erro.</param>
        public static void Write(string info, EventLogEntryType eventLogEntryType, short categoryID, int eventID)
        {
            if (!EventLog.SourceExists(SOURCE))
            {
                CreateSource();
            }
            SPSecurity.RunWithElevatedPrivileges(
                delegate()
                {
                    if (info.Length > MAX_SIZE)
                    {
                        info = info.Substring(0, MAX_SIZE);
                    }

                    EventLog.WriteEntry(SOURCE, info, eventLogEntryType, eventID, categoryID);
                }
            );
        }

        /// <summary>
        /// Grava uma exceção no Event Viewer.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="eventLogEntryType"></param>
        /// <param name="categoryID"></param>
        /// <param name="eventID"></param>
        public static void Write(string info, EventLogEntryType eventLogEntryType, CategoryID categoryID, EventID eventID)
        {
            if (!EventLog.SourceExists(SOURCE))
            {
                CreateSource();
            }
            SPSecurity.RunWithElevatedPrivileges(
                delegate()
                {
                    if (info.Length > MAX_SIZE)
                    {
                        info = info.Substring(0, MAX_SIZE);
                    }

                    EventLog.WriteEntry(SOURCE, info, eventLogEntryType, Convert.ToInt32(eventID), short.Parse(Convert.ToInt32(categoryID).ToString()));
                }
            );
        }

        /// <summary>
        /// Grava uma exceção no Event Viewer.
        /// </summary>
        /// <param name="info">Descrição da exceção.</param>
        /// <param name="source">Fonte da exceção. Caso não exista, passe String.Empty.</param>
        public static void WriteError(string info, short categoryID, int eventID)
        {
            Write(info, EventLogEntryType.Error, categoryID, eventID);
        }

        private static void CreateSource()
        {
            EventLog.CreateEventSource(SOURCE, SOURCE);
            Console.WriteLine(string.Format("O event source {0} foi criado com sucesso."), SOURCE);
            Console.ReadKey();
        }

    }
}
