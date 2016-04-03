using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace BigData
{
    public static class Excel
    {
        private static readonly string CONNECTION_STRING = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Text;HDR=No;FMT=Delimited\"";

        #region Public Methods
        public static DataTable LoadTable(string strFilePath, string strFileName)
        {
            DataTable dtTableToRet = new DataTable();
            OleDbConnection conConnection = null;

            try
            {
                using (conConnection = new OleDbConnection(string.Format(CONNECTION_STRING, strFilePath)))
                {
                    using (OleDbDataAdapter dtAdapter = new OleDbDataAdapter("SELECT * FROM " + strFileName, conConnection))
                    {
                        // Open the connection
                        OpenConnection(conConnection);

                        dtAdapter.Fill(dtTableToRet);
                    }
                }
            }
            catch (Exception ex)
            {
                return (null);
            }
            finally
            {
                // Close the connection
                CloseConnection(conConnection);
            }

            return (dtTableToRet);
        }
        #endregion

        #region Private Methods
        private static void OpenConnection(OleDbConnection conConnection)
        {
            if ((conConnection != null) && (conConnection.State != ConnectionState.Open))
            {
                conConnection.Open();
            }
        }
        private static void CloseConnection(OleDbConnection conConnection)
        {
            if ((conConnection != null) && (conConnection.State == ConnectionState.Open))
            {
                conConnection.Close();
            }
        }
        #endregion
    }
}
