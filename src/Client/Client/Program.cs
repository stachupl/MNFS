using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using nameServerClass;
using FileTreeLibrary;

namespace Client
{
    class Program
    {
        static string path = "/";
        static NameServer nameServer;

        static string makeAbsolutePath(string directoryPath)
        {
            if (directoryPath.Length == 0)
                return "";
            else if (directoryPath[0] == '/')
                return directoryPath;
            else
                return path + '/' + directoryPath;
        }

        static void ls(string path)
        {
            string result;
            string[] toPrintf;
            string[] file = new string[2];

            try
            {
                result = nameServer.ls(path);
            }
            catch (Exception)
            {
                Console.WriteLine("Device temporarily unavailable. Try again later.");
                return;
            }

            if (result != "" && result != null)
            {
                toPrintf = result.Split(' ');
                foreach (string elem in toPrintf)
                {
                    if (elem != "")
                    {
                        file = elem.Split('#');
                        if (file[0] == "dir")
                        {
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            Console.Write(file[1]);
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write(" ");
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                            Console.Write(file[1]);
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write(" ");
                        }
                    }                    
                }
                
                Console.WriteLine();
            }
        }

        static void mkdir(string path)
        {
            string name;
            string directory;

            name = path.Split('/').Last();
            directory = path.Substring(0, path.Length - name.Length - 1);

            try
            {
                if (directory.Trim().Length == 0)
                    Console.WriteLine(nameServer.mkdir("/", name));
                else
                    Console.WriteLine(nameServer.mkdir(directory, name));
            }
            catch (Exception)
            {
                Console.WriteLine("Device temporarily unavailable. Try again later.");
            }
        }

        static void rename(string path, string name)
        {
            try
            {
                Console.WriteLine(nameServer.rename(path, name));
            }
            catch (Exception)
            {
                Console.WriteLine("Device temporarily unavailable. Try again later.");
            }
        }

        static void renamedir(string path, string name)
        {
            try
            {
                Console.WriteLine(nameServer.renamedir(path, name));
            }
            catch (Exception)
            {
                Console.WriteLine("Device temporarily unavailable. Try again later.");
            }
        }

        static void rmdir(string path)
        {
            try
            {
                Console.WriteLine(nameServer.rmdir(path));
            }
            catch
            {
                Console.WriteLine("Device temporarily unavailable. Try again later.");
            }
        }

        static void mv(string sourcePath, string destinationPath)
        {
            try
            {
                Console.WriteLine(nameServer.mv(sourcePath, destinationPath));
            }
            catch (Exception)
            {
                Console.WriteLine("Device temporarily unavailable. Try again later.");
            }
        }

        static void find(string path, string name)
        {
            ArrayList foundedFiles = new ArrayList();

            try
            {
               foundedFiles = nameServer.findfile(path, name);
            }
            catch (Exception)
            {
                Console.WriteLine("Device temporarily unavailable. Try again later.");
                return;
            }

            Console.BackgroundColor = ConsoleColor.DarkGreen;
            foreach (string file in foundedFiles)
                Console.Write(file);

            Console.BackgroundColor = ConsoleColor.Black;
        }

        static void finddir(string path, string name)
        {
            ArrayList foundedFiles = new ArrayList();

            try
            {
                foundedFiles = nameServer.finddir(path, name);
            }
            catch(Exception e)
            {
                Console.WriteLine("Device temporarily unavailable. Try again later.");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Data);
                Console.WriteLine("FFF");
            }

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            foreach (string file in foundedFiles)
                Console.WriteLine(file);

            Console.BackgroundColor = ConsoleColor.Black;
        }

        static void touch(string path)
        {
            string name;
            string directory;

            name = path.Split('/').Last();
            directory = path.Substring(0, path.Length - name.Length - 1);

            try
            {
                if (directory.Trim().Length == 0)
                    Console.WriteLine(nameServer.touch("/", name));
                else
                    Console.WriteLine(nameServer.touch(directory, name));
            }
            catch (Exception)
            {
                Console.WriteLine("Device temporarily unavailable. Try again later.");
            }
        }

        static void rm(string path)
        {
            try
            {
                Console.WriteLine(nameServer.rm(path));
            }
            catch (Exception)
            {
                Console.WriteLine("Device temporarily unavailable. Try again later.");
            }
        }

        static void read(string path, string startLine, string nrOfLine)
        {
            ArrayList servers = new ArrayList();

            try
            {
                servers = nameServer.read(path);
            }
            catch (Exception)
            {
                Console.WriteLine("Device temporarily unavailable. Try again later.");
            }

            Console.WriteLine(servers.Count);
            foreach (string a in servers)
                Console.WriteLine(a);

            //foreach (FileNetworkAddress fna in servers)
              //  Console.WriteLine(fna.id, fna.ip, fna.port);
        }

        static void write(string path)
        {
            FileNetworkAddress servers = new FileNetworkAddress();

            try
            {
                //servers = (FileNetworkAddress) nameServer.write(path);
            }
            catch (Exception)
            {
                Console.WriteLine("Device temporarily unavailable. Try again later.");
            }
        }

        static void cat(string path)
        {
            List<FileNetworkAddress> servers = new List<FileNetworkAddress>();

            try
            {
                //servers = nameServer.read(path);
            }
            catch (Exception)
            {
                Console.WriteLine("Device temporarily unavailable. Try again later.");
            }
        }

        static void Main(string[] args)
        {
            string[] command;
            string[] incorrectChar = {" ", "" };
            HttpChannel c = new HttpChannel();
            ChannelServices.RegisterChannel(c, true);
            Type ServerType = typeof(nameServerClass.NameServer);
            string url = "http://localhost:3300/Object";
            RemotingConfiguration.RegisterWellKnownClientType(ServerType, url);
            nameServer = new NameServer();

            while (true)
            {
                Console.Write(path + " ");
                command = Console.ReadLine().Split(' ').Except(incorrectChar).ToArray();
                
                if (command.Count() == 0)
                    continue;

                switch (command[0])
                {
                    case "ls":
                        if (command.Count() == 1)
                            ls(path);
                        else
                            ls(makeAbsolutePath(command[1]));
                        break;

                    case "mkdir":
                        if (command.Count() > 1)
                            mkdir(makeAbsolutePath(command[1]));
                        else
                            Console.WriteLine("Wrong number of parameter!");
                        break;

                    case "rename":
                        if (command.Count() >= 3)
                            rename(makeAbsolutePath(command[1]), command[2]);
                        else
                            Console.WriteLine("Wrong number of parameter!");
                        break;

                    case "renamedir":
                        if (command.Count() >= 3)
                            renamedir(makeAbsolutePath(command[1]), command[2]);
                        else
                            Console.WriteLine("Wrong number of parameter!");
                        break;

                    case "rmdir":
                        if (command.Count() >= 2)
                            rmdir(makeAbsolutePath(command[1]));
                        else
                            Console.WriteLine("Wrong number of parameter!");
                        break;

                    case "mv":
                        if (command.Count() >= 3)
                            mv(makeAbsolutePath(command[1]), makeAbsolutePath(command[2]));
                        else
                            Console.WriteLine("Wrong number of parameter!");
                        break;

                    case "find":
                        if (command.Count() >= 3)
                            find(makeAbsolutePath(command[1]), makeAbsolutePath(command[2]));
                        else
                            Console.WriteLine("Wrong number of parameter!");
                        break;
                    
                    case "finddir":
                        if (command.Count() >= 3)
                            finddir(makeAbsolutePath(command[1]), command[2]);
                        else
                            Console.WriteLine("Wrong number of parameter!");
                        break;

                    case "rm":
                        if (command.Count() >= 2)
                            rm(makeAbsolutePath(command[1]));
                        else
                            Console.WriteLine("Wrong number of parameter!");
                        break;

                    case "touch":
                        if (command.Count() >= 2)
                            touch(makeAbsolutePath(command[1]));
                        else
                            Console.WriteLine("Wrong number of parameter!");
                        break;

                    case "read":
                        if (command.Count() >= 4)
                            read(makeAbsolutePath(command[1]), command[2], command[3]);
                        else
                            Console.WriteLine("Wrong number of parameter!");
                        break;
                }
            }
        }
    }
}
