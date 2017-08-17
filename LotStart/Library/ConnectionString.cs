using System.Configuration;

namespace LotStart.Library
{
    public class ConnectionString
    {
        private static DataLayerLibrary.DataLayer dataLayer = null;

        static ConnectionString()
        {
            if (dataLayer == null)
            {
                dataLayer = new DataLayerLibrary.DataLayer(ConfigurationManager.AppSettings["env"].ToString() + "_lotstart");
            }
        }

        public static DataLayerLibrary.DataLayer returnCon
        {
            get
            {
                return dataLayer;
            }
        }
    }
}