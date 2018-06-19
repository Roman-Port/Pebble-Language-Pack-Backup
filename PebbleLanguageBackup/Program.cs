using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace PebbleLanguageBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            //Prompt the user for info
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Please type in or copy (right click on the window) where you'd like these files to be on the web server.\r\nFor example, typing \"");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("http://example.com/");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\" will place the language files in \"");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("http://example.com/packs/[pack id].pbl");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\".\r\n");
            string rootUrl = Console.ReadLine().TrimEnd('/')+"/";
            Console.Clear();
            Console.Write("Please type in or paste (right click on the window) the folder you'd like to place the files in on this machine.\r\nThis is where they will be saved.\r\n");
            string rootSavePath = Console.ReadLine().Trim('\\')+"\\";
            //Add the save directory if it doesn't exist.
            Directory.CreateDirectory(rootSavePath + "packs\\");
            
            //First, download the current language list.
            Console.Write("Downloading lang list......");
            string rawLang = DownloadString("http://lp.getpebble.com/v1/languages/");
            //Deserialize this.
            JSONRoot langRoot = DeserializeObject<JSONRoot>(rawLang);
            //Loop through each of the languages and download the pack.
            Console.Write("Done\r\n");
            int i = 0;
            while(i<langRoot.languages.Length)
            {
                Console.Write("Downloading language pack " + i.ToString() + " of " + langRoot.languages.Length.ToString() + " (" + langRoot.languages[i].name + ")......");
                string urlOfLang = langRoot.languages[i].file;
                //Change the file path and get the save location
                string saveLocation = rootSavePath + "packs\\" + langRoot.languages[i].id+".pbl";
                langRoot.languages[i].file = rootUrl + "packs/" + langRoot.languages[i].id;
                //Download the file.
                try
                {
                    //Check if this file already exists.
                    if (File.Exists(saveLocation))
                        throw new Exception("The file already exists locally. Are you redownloading to an old folder?");
                    byte[] data = DownloadFile(urlOfLang);
                    //Save
                    File.WriteAllBytes(saveLocation, data);
                    Console.Write("Done\r\n");
                } catch (Exception ex)
                {
                    string message = "Failed! " + ex.Message;
                    Console.Write(message + "\r\n");
                }
                i++;

            }
            //Serialize and save the language packs.
            Console.Write("Saving language packs index......");
            string ser = SerializeObject(langRoot);
            File.WriteAllText(rootSavePath+"index.json", ser);
            //Done.
            Console.Write("Done.\r\nCompleted backup.");
            Console.ReadLine();
        }

        public static T DeserializeObject<T>(string value)
        {
            //Get a data stream
            MemoryStream mainStream = GenerateStreamFromString(value);

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            //Load it in.
            mainStream.Position = 0;
            var obj = ser.ReadObject(mainStream);
            return (T)obj;
        }

        public static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }

        public static string SerializeObject(object obj)
        {
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());
            ser.WriteObject(stream1, obj);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            return sr.ReadToEnd();
        }

        static byte[] DownloadFile(string url)
        {
            byte[] data;
            using (WebClient wc = new WebClient())
            {
                data = wc.DownloadData(url);
            }
            return data;
        }

        static string DownloadString(string url)
        {
            //Download data
            byte[] data = DownloadFile(url);
            //Decode it
            return Encoding.UTF8.GetString(data);
        }
    }
}
