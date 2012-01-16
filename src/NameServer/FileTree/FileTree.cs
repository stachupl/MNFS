using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileTreeLibrary
{
    // Klasa zawiera informacje o serwerze przechowującym dany plik
    [Serializable]
    public class FileNetworkAddress
    {
        public string id;
        public string ip;
        public string port;

        public FileNetworkAddress()
        {
            this.id = "";
            this.ip = "";
            this.port = "";
        }

        public FileNetworkAddress(string id, string ip, string port)
        {
            this.id = id;
            this.ip = ip;
            this.port = port;
        }
    }

    // Węzeł drzewa będącego strukturą plików
    [Serializable]
    public class FileNode
    {
        public string name; // nazwa pliku
        public bool isDirectory;
        public ArrayList fileLocation; // zawiera informacje o serwerach na których znajduje się plik
        public List<FileNode> children; // jeśli jest katalogiem to zawiera odniesienia do innych węzłów (plików lub katalogów)
        public FileNode parent; // odniesienie do węzła nadrzędnego

        public FileNode()
        {
            this.name = "";
            this.isDirectory = false;
            this.fileLocation = null;
            this.children = null;
            this.parent = null;
        }

        public FileNode(string name)
        {
            this.name = name;
            this.isDirectory = true;
            this.fileLocation = null;
            this.children = new List<FileNode>();
            this.parent = null;
        }

        public FileNode(string name, FileNetworkAddress address)
        {
            this.name = name;
            this.isDirectory = false;
            this.fileLocation = new ArrayList();
            this.fileLocation.Add(address);
            this.children = null;
            this.parent = null;
        }
    }

    // buduje strukturę plików jako drzewo
    [Serializable]
    public class FileTree
    {
        private FileNode root;

        public FileTree()
        {
            root = new FileNode("/");
        }

        private void delete()
        {
            root = new FileNode("/");
        }

        public FileNode locateDirectory(string path)
        {
            string[] empty = { "" };

            IEnumerable<string> splitedPath = path.Split('/').Except(empty);
            FileNode directory = root;

            foreach (string subdirectory in splitedPath)
            {
                directory = directory.children.Find(delegate(FileNode fn)
                {
                    return fn.name == subdirectory && fn.isDirectory == true;
                }
                );

                if (directory == null)
                    return null;
            }

            return directory;
        }

        public string mkdir(string path, string name)
        {
            FileNode newDirectory = new FileNode(name);
            FileNode directory;
            bool exist = false;

            directory = locateDirectory(path);
            if (directory == null)
                return "Path doesn't exist. Can't create directory!";

            foreach (FileNode fn in directory.children)
                if (fn.name == name && fn.isDirectory == true)
                    exist = true;

            if (exist == true)
                return "Directory exist. Can't create!";
            else
            {
                newDirectory.parent = directory;
                directory.children.Add(newDirectory);
                return "Directory created.";
            }
        }

        public string rmdir(string path)
        {
            FileNode directory;

            if (path == "/")
                return "Can't remove root directory!";

            directory = locateDirectory(path);
            if (directory == null)
                return "Path doesn't exist. Can't remove directory!";

            if (directory.isDirectory == false)
                return "Directory doesn't exist. Can't remove!";
            else if (directory.children.Count == 0)
            {
                directory.parent.children.Remove(directory);
                return "Directory successfully removed.";
            }
            else
                return "Directory is not empty. Can't remove!";
        }

        public string rm(string path)
        {
            FileNode file;

            file = locateFile(path);

            if (file == null)
                return "Path doesn't exist. Can't remove file!";

            if (file.isDirectory == true)
                return "File doesn't exist. Can't remove!";
            
            file.parent.children.Remove(file);
            
            return "File successfully removed.";
        }

        public string renamedir(string path, string newName)
        {
            FileNode directory;
            bool exist = false;

            if (path == "/")
                return "Can't rename root directory!";

            directory = locateDirectory(path);
            if (directory == null)
                return "Path doesn't exist. Can't rename directory!";

            foreach (FileNode dir in directory.parent.children)
                if (dir.name == newName && dir.isDirectory == true)
                    exist = true;

            if (exist == true)
                return "File with the same name exist. Can't rename!";
            else
            {
                directory.name = newName;
                return "Directory name successfully renamed.";
            }
        }

        public string create(string path, string name, string locationIP, string locationPort, string id)
        {
            FileNode directory;
            bool exist = false;

            directory = locateDirectory(path);
            if (directory == null)
                return "Path doesn't exist. Can't create file!";

            foreach (FileNode fn in directory.children)
                if (fn.name == name && fn.isDirectory == false)
                    exist = true;

            if (exist == true)
                return "File exist. Can't create!";
            else
            {
                FileNetworkAddress address = new FileNetworkAddress(id, locationIP, locationPort);
                FileNode newFile = new FileNode(name, address);
                newFile.parent = directory;
                directory.children.Add(newFile);

                return "File successfully created.";
            }
        }

        public FileNode locateFile(string path)
        {
            string[] empty = { "" };
            IEnumerable<string> sp = path.Split('/').Except(empty);
            int count = sp.Count();
            FileNode directory;
            string name;

            if (count == 0)
                return null;
            else if (count == 1)
            {
                directory = root;
                name = sp.ElementAt(0);
            }
            else
            {
                IEnumerable<String> splitedPath = sp.Take(count - 1);
                name = sp.ElementAt(count - 1);
                directory = root;

                foreach (string subdirectory in splitedPath)
                {
                    directory = directory.children.Find(delegate(FileNode fn)
                    {
                        return fn.name == subdirectory && fn.isDirectory == true;
                    }
                    );

                    if (directory == null)
                        return null;
                }
            }

            foreach (FileNode fn in directory.children)
            {
                if (fn.name == name && fn.isDirectory == false)
                    return fn;
            }

            return null;
        }

        public string rename(string path, string newName)
        {
            FileNode file;
            bool exist = false;

            file = locateFile(path);
            if (file == null)
                return "File doesn't exist. Can't rename!";

            foreach (FileNode fn in file.parent.children)
                if (fn.name == newName)
                    exist = true;

            if (exist == false)
            {
                file.name = newName;
                return "File name was successfully renamed.";
            }
            else
                return "File with the same name exist. Can't rename!";
        }

        public string addLocation(string path, string id, string ip, string port)
        {
            FileNode file;
            FileNetworkAddress address = new FileNetworkAddress(id, ip, port);

            file = locateFile(path);
            if (file == null)
                return "File doesn't exist. Can't add new location";

            file.fileLocation.Add(address);

            return "New location successfully added";
        }

        public string removeLocation(string path, string id)
        {
            FileNode file;

            file = locateFile(path);
            if (file == null)
                return "File doesn't exist. Can't remove location";

            foreach (FileNetworkAddress fna in file.fileLocation)
                if (fna.id == id)
                {
                    file.fileLocation.Remove(fna);
                    return "Location successfully removed.";
                }

            return "Location doesn't exist. Can't remove!";
        }

        public ArrayList fileServers(string path)
        {
            FileNode fn = locateFile(path);

            if (fn == null)
                return null;
            else
                return fn.fileLocation;
        }

        public void writeLocation(string path)
        {
            FileNode file;

            file = locateFile(path);
            if (file != null)
                foreach (FileNetworkAddress fna in file.fileLocation)
                    Console.WriteLine("{0} {1} {2}", fna.id, fna.ip, fna.port);
        }

        public string mv(string sourcePath, string destinationPath)
        {
            FileNode file;
            FileNode destinationDiretory;
            bool exist = false;

            file = locateFile(sourcePath);
            if (file == null)
            {
                file = locateDirectory(sourcePath);

                if (file == null)
                    return "File or directory doesn't exist. Can't move!";
            }

            destinationDiretory = locateDirectory(destinationPath);
            if (destinationDiretory == null)
                return "Destination directory doesn't exist. Can't move!";

            foreach (FileNode fn in destinationDiretory.children)
                if (fn.name == file.name)
                    exist = true;

            if (exist == true)
                return "In destination directory exist file or directory with the same name. Can't move!";

            file.parent.children.Remove(file);
            destinationDiretory.children.Add(file);
            return "File or directory successfuly moved.";
        }

        public string cpdir(string sourcePath, string destinationPath)
        {
            FileNode file, newFile;
            FileNode destinationDiretory;
            bool exist = false;

            file = locateDirectory(sourcePath);
            if (file == null)
                return "Directory doesn't exist. Can't copy!";

            destinationDiretory = locateDirectory(destinationPath);
            if (destinationDiretory == null)
                return "Destination directory doesn't exist. Can't copy!";

            if (file.children.Count > 0)
                return "Source directory doesn't empty. Can't copy!";

            foreach (FileNode fn in destinationDiretory.children)
                if (fn.name == file.name && fn.isDirectory == true)
                    exist = true;

            if (exist == true)
                return "In destination directory exist directory with the same name. Can't copy!";

            newFile = new FileNode(file.name);
            newFile.parent = destinationDiretory;
            destinationDiretory.children.Add(newFile);

            return "Directory successfully copied.";
        }

        public string cp(string sourcePath, string destinationPath, FileNetworkAddress address)
        {
            FileNode file, newFile;
            FileNode destinationDiretory;
            bool exist = false;

            file = locateFile(sourcePath);
            if (file == null)
                return "File doesn't exist. Can't copy!";

            destinationDiretory = locateDirectory(destinationPath);
            if (destinationDiretory == null)
                return "Destination directory doesn't exist. Can't copy!";

            foreach (FileNode fn in destinationDiretory.children)
                if (fn.name == file.name && fn.isDirectory == false)
                    exist = true;

            if (exist == true)
                return "In destination directory exist file with the same name. Can't copy!";

            newFile = new FileNode(file.name, address);
            newFile.parent = destinationDiretory;
            destinationDiretory.children.Add(newFile);

            return "File successfully copied.";
        }

        public void findDirectory(FileNode directory, string workingDirectory, string name, ArrayList result)
        {
            foreach (FileNode fn in directory.children)
            {
                Console.WriteLine("AAA {0}, {1}, {2},", fn.name, name, fn.isDirectory);
                if (fn.name == name && fn.isDirectory == true)
                {
                    result.Add(workingDirectory + "/" + name);
                    Console.WriteLine("ffff");
                }

                if (fn.isDirectory == true)
                    findDirectory(fn, workingDirectory + "/" + fn.name, name, result);
            }

        }

        public ArrayList findDirectory(string path, string name)
        {
            FileNode directory;
            ArrayList result = new ArrayList();

            directory = locateDirectory(path);

            if (directory == null)
                return result;
            else
            {
                try
                {
                    if (path == "/")
                        findDirectory(directory, "", name, result);
                    else
                        findDirectory(directory, path, name, result);
                }
                catch (Exception)
                {
                }

                return result;
            }
        }

        public void findFile(FileNode directory, string workingDirectory, string name, ArrayList result)
        {
            foreach (FileNode fn in directory.children)
            {
                if (fn.name == name && fn.isDirectory == false)
                    result.Add(workingDirectory + "/" + name);

                if (fn.isDirectory == true)
                    findFile(fn, workingDirectory + "/" + fn.name, name, result);
            }

        }

        public ArrayList findFile(string path, string name)
        {
            FileNode directory;
            ArrayList result = new ArrayList();

            directory = locateDirectory(path);
            if (directory == null)
                return result;
            else
            {
                if (path == "/")
                    findFile(directory, "", name, result);
                else
                    findFile(directory, path, name, result);

                return result;
            }
        }

        public string ls(string path)
        {
            string result = "";
            FileNode directory;

            directory = locateDirectory(path);

            if (directory == null)
                return null;

            foreach (FileNode fn in directory.children)
            {
                if (fn.isDirectory == true)
                    result = result + " dir#" + fn.name;
                else
                    result = result + " file#" + fn.name;
            }

            return result;
        }
    }
}
