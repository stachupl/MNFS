using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileTreeLibrary;
using serializationNameServer;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using FileSystem;

namespace nameServerClass
{
    public class NameServer : MarshalByRefObject
    {
        FileTree fileTree;
        List<fileServers> listOfFileServers;
        serialization sr;

        public NameServer()
        {
            sr = new serialization();

            try
            {
                fileTree = sr.deserialize();
            }
            catch (Exception e)
            {
                Console.WriteLine("Can't deserialize file tree!");
                Console.WriteLine(e.Message);

                throw new Exception("Can't deserialize!");
            }

            try
            {
                listOfFileServers = sr.readFileServersList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Can't read file servers list!");
                Console.WriteLine(e.Message);

                throw new Exception("Can't read list of file servers!");
            }
        }

        private string createFile(string ip, string port)
        {
            //HttpChannel c = new HttpChannel();
            //ChannelServices.RegisterChannel(c, true);
            Type ServerType = typeof(FileSystem.FileManager);
            string url = ip + ":" + port + "/Object";
            
            if (RemotingConfiguration.IsWellKnownClientType(ServerType) == null)
                RemotingConfiguration.RegisterWellKnownClientType(ServerType, url);
            
            FileManager vfs = new FileManager();

            try
            {
                return vfs.create();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private bool removeFile(string ip, string port, string name)
        {
            //HttpChannel c = new HttpChannel();
            //ChannelServices.RegisterChannel(c, true);
            Type ServerType = typeof(FileSystem.FileManager);
            string url = ip + ":" + port + "/Object";
            
            if (RemotingConfiguration.IsWellKnownClientType(ServerType) == null)
                RemotingConfiguration.RegisterWellKnownClientType(ServerType, url);
            
            FileManager vfs = new FileManager();

            try
            {
                return vfs.remove(name);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string mkdir(string path, string name)
        {
            return fileTree.mkdir(path, name);
        }

        public string rename(string path, string name)
        {
            return fileTree.rename(path, name);
        }

        public string renamedir(string path, string name)
        {
            return fileTree.renamedir(path, name);
        }

        public string rmdir(string path)
        {
            return fileTree.rmdir(path);
        }

        public ArrayList finddir(string path, string name)
        {
            return fileTree.findDirectory(path, name);
        }

        public ArrayList findfile(string path, string name)
        {
            return fileTree.findFile(path, name);
        }

        public string mv(string sourcePath, string destinationPath)
        {
            return fileTree.mv(sourcePath, destinationPath);
        }

        public string ls(string path)
        {
            return fileTree.ls(path);
        }

        public string touch(string path, string name)
        {
            int seed = System.DateTime.Now.Millisecond;
            Random rand = new Random(seed);
            int startServer = rand.Next(1, listOfFileServers.Count);
            string result, createdFileName = "";
            fileServers server = new fileServers();

            int i = 0;
            while (i < listOfFileServers.Count && createdFileName == "") 
            {
                server = listOfFileServers.ElementAt((startServer + i) % listOfFileServers.Count);
                createdFileName = createFile(server.ip, server.port);
                i++;
            }

            if (createdFileName == "")
                return "Device full! Try again later.";

            result = fileTree.create(path, name, server.ip, server.port, createdFileName);

            if (result != "File successfully created.")
                removeFile(server.ip, server.port, createdFileName);
            
            return result;
        }

        public string rm(string path)
        {
            string result;
            ArrayList fileNetworkAddress = fileTree.fileServers(path);

            result = fileTree.rm(path);

            if (result == "File successfully removed." && fileNetworkAddress != null)
                foreach (FileNetworkAddress fna in fileNetworkAddress)
                    removeFile(fna.ip, fna.port, fna.id);

            return result;
        }

        public ArrayList read(string path)
        {
            FileNode fn = fileTree.locateFile(path);
            ArrayList serverList = new ArrayList();
            
            if (fn == null)
                return null;
            else
            {
                foreach (FileNetworkAddress f in fn.fileLocation)
                    serverList.Add(f.ip + "#" + f.port + "#" + f.id);

                return serverList;
            }
        }

        public ArrayList cat(string path)
        {
            FileNode fn = fileTree.locateFile(path);

            if (fn == null)
                return null;
            else
                return fn.fileLocation;
        }

        public FileNetworkAddress write(string path)
        {
            FileNode file = fileTree.locateFile(path);

            if (file != null)
            {
                for (int i = 1; i < file.fileLocation.Count; i++)
                    file.fileLocation.RemoveAt(1);

                return (FileNetworkAddress) file.fileLocation[0];
            }
            else
                return null;
        }

        public ArrayList cp(string filePath, string destination)
        {
            FileNode file= fileTree.locateFile(filePath);
            
            string result;

            if (file == null)
                return null;

            result = touch(destination, filePath.Split('/').Last());

            if (result != "File successfully created.")
                return null;
            else
                return file.fileLocation;
        }
    }
}
