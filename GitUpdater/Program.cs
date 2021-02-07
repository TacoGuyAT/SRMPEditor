using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace GitUpdater
{
    class Program
    {
        public static string owner = "TacoGuyAT";
        public static string project = "SRMPEditor";
        public static WebClient client;
        public static ZipArchive archive;
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            string repo = owner + "/" + project;
            client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:85.0) Gecko/20100101 Firefox/85.0");
            client.DownloadDataCompleted += (s, e) =>
            {
                Console.WriteLine();
                int i = 0;
                archive = new ZipArchive(new MemoryStream(e.Result));
                if (Process.GetProcessesByName(project).Length != 0)
                {
                    if (Process.GetProcessesByName(project).Length == 1)
                        Console.WriteLine("Waiting while " + project + " is shutting down");
                    else
                        Console.WriteLine("Waiting while " + Process.GetProcessesByName(project).Length + " processes of " + project + " are shutting down");
                    foreach (Process p in Process.GetProcessesByName(project))
                        if (!p.HasExited)
                            p.WaitForExit();
                }
                foreach (ZipArchiveEntry file in archive.Entries)
                {
                    try
                    {
                        if (file.FullName != Path.GetDirectoryName(Assembly.GetExecutingAssembly().FullName))
                            file.ExtractToFile(file.Name, true);
                    }
                    catch (Exception e1)
                    {
                        i++;
                        if (!Directory.Exists("update")) Directory.CreateDirectory("update");
                        file.ExtractToFile("update\\" + file.Name, true);
                    }
                }
                Console.WriteLine("Update was downloaded");
                if (i != 0)
                {
                    Console.WriteLine("Couldn't replace " + i + " files");
                    Console.WriteLine("Moved failed files to \"update\" directory");
                }
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                Environment.Exit(0);
            };

            client.DownloadStringCompleted += (s, e) =>
            {
                Dictionary<string, object> repoInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(e.Result);
                bool allowSend = true; // i'm really lazy to make it properly
                client.DownloadProgressChanged += async (s, e) =>
                {
                    if (allowSend)
                    {
                        allowSend = false;
                        string bar = "";
                        for (int i = 0; i < 20; i++)
                            if (Convert.ToDouble(i) / Convert.ToDouble(20) < ((double)e.BytesReceived / 1048576) / ((double)e.TotalBytesToReceive / 1048576))
                                bar = bar + "#";
                            else
                                bar = bar + "-";
                        Console.WriteLine("[" + bar + "] | " + Math.Round((double)e.BytesReceived / 1048576, 2, MidpointRounding.ToZero).ToString() + "/" + Math.Round((double)e.TotalBytesToReceive / 1048576, 2, MidpointRounding.ToZero).ToString() + " MB       ");
                        Console.SetCursorPosition(0, Console.CursorTop - 1);
                        await Task.Delay(64);
                        allowSend = true;
                    }
                };
                try
                {
                    if (File.Exists(project + ".exe"))
                        if ((string)repoInfo["tag_name"] != FileVersionInfo.GetVersionInfo(project + ".exe").FileVersion)
                        {
                            Console.WriteLine("Latest version is v" + repoInfo["tag_name"]);
                            Console.WriteLine("Started downloading the update:");
                            client.DownloadDataAsync(new Uri("https://github.com/" + repo + "/releases/latest/download/release.zip"));
                        }
                        else
                        {
                            Console.WriteLine("Latest version of " + repo + " is already installed!");
                            Console.WriteLine("Press any key to continue");
                            Console.ReadKey();
                            Environment.Exit(0);
                        }
                    else
                    {
                        Console.WriteLine("Latest version is v" + repoInfo["tag_name"]);
                        Console.WriteLine("Couldn't find " + project + ".exe");
                        Console.WriteLine("Started downloading the update:");
                        client.DownloadDataAsync(new Uri("https://github.com/" + repo + "/releases/latest/download/release.zip"));
                    }
                }
                catch (Exception e1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nCouldn't download the update:\n" + e1.StackTrace + "\n\nPress any key to continue");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            };

            Console.WriteLine("Checking repo " + repo + " for updates");
            client.DownloadStringAsync(new Uri("https://api.github.com/repos/" + repo + "/releases/latest"));
            Thread.Sleep(-1);
        }
    }
}
