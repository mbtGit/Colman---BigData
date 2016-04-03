using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigData
{
    class SSH
    {
        public static void Command(UserInfo uiInfo, string command)
        {
            using (var client = new SshClient(uiInfo.Host, uiInfo.User, uiInfo.Pass))
            {
                client.Connect();
                client.RunCommand(command);
                client.Disconnect();
            }
        }

        public static void RunCommands(UserInfo uiInfo, string[] commands)
        {
            using (var client = new SshClient(uiInfo.Host, uiInfo.User, uiInfo.Pass))
            {
                client.Connect();
                foreach (string currCommand in commands)
                {
                    client.RunCommand(currCommand);
                }
                client.Disconnect();
            }
        }

        public static void TransferFilesFromWinToLin(UserInfo uiInfo, string[] local, string remote)
        {
            using (var client = new SftpClient(uiInfo.Host, uiInfo.User, uiInfo.Pass))
            {
                // Connect host
                client.Connect();
                
                foreach (string currLocalFile in local)
                {
                    string[] arrSplit = currLocalFile.Split('\\');
                    string strFileName = arrSplit[arrSplit.Length - 1];
                    
                    using (var file = File.OpenRead(currLocalFile))
                    {
                        client.UploadFile(file, string.Format("{0}/{1}", remote, strFileName));
                    }
                }

                // Disconnect
                client.Disconnect();
            }
        }

        public static void TransferWinToLin(UserInfo uiInfo, string local, string remote)
        {
            using (var client = new SftpClient(uiInfo.Host, uiInfo.User, uiInfo.Pass))
            {
                // Connect host
                client.Connect();

                Console.WriteLine("Transfer Files!");
                using (var file = File.OpenRead(local))
                { 
                    client.UploadFile(file, remote);
                }

                // Disconnect
                client.Disconnect();
            }
        }
        public static void TransferLinToWin(UserInfo uiInfo, string remote, string local)
        {
            using (var client = new SftpClient(uiInfo.Host, uiInfo.User, uiInfo.Pass))
            {
                // Connect host
                client.Connect();

                Console.WriteLine("Transfer Files!");

                using (var file = File.OpenWrite(local))
                {
                    client.DownloadFile(remote, file);
                }

                // Disconnect
                client.Disconnect();
            }
        }
    }
}
