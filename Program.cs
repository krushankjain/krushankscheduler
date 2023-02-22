using System;
using System.IO;
using System.Configuration;
using Azure.Storage.Blobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Linq;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace DeleteSitefinityLogs
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            DeleteFiles();
        }

        public static void DeleteFiles()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(ConfigurationSettings.AppSettings["LogPath"]);
                //DirectoryInfo di = new DirectoryInfo(@"C:\Users\Rachel_Dsouza\OneDrive - Idealake Information Technologies Pvt. ltd\Documents\Canara\DeleteSitefinityLogs\App_Data\Sitefinity\Logs");
                string current_logs = System.Environment.CurrentDirectory;
                Console.WriteLine(current_logs);
                if (!di.Exists)
                {
                    Console.WriteLine(di.FullName + " Directory does not exists");
                   // Console.ReadKey();
                }
                else
                    foreach (FileInfo file in di.GetFiles())
                    {
                        int value = DateTime.Now.Date.Day -1 ;
                        if (file.FullName.Contains("Error") && file.Extension == ".log" && (file.CreationTime.Date.Day == value))
                        {
                            if (MoveFilesToAzureBlob(file.FullName, file.Name))
                            {
                                //file.Delete();
                                Console.WriteLine(file.FullName + " File moved to Blob");
                                file.Delete();
                                Console.WriteLine(file.FullName + " File deleted from folder");
                                //Console.ReadKey();
                            }
                        }
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
                throw;
            }

        }

        /// <summary>
        /// Move error log files to Azure blob
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool MoveFilesToAzureBlob(string filePath, string fileName)
        {
            try
            {
                var ConnString = ConfigurationSettings.AppSettings["AzureBlobConnectionString"].ToString();
                BlobClient objClient = new BlobClient(connectionString: ConnString, blobContainerName: "prod-sitefinitylogs-backup/" + DateTime.Now.Year+"/"+DateTime.Now.Date.ToString().Split(' ')[0].Replace("-",""), blobName: fileName);
                //BlobClient objClient = new BlobClient(connectionString: ConnString, blobContainerName: "uat-sitefinitylogs-backup/" + DateTime.Now.Year + "/" + DateTime.Now.Date.ToString().Split(' ')[0].Replace("-", ""), blobName: fileName);
 
                objClient.Upload(filePath);
                Console.WriteLine("Fileuploaded");
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
    }
}
