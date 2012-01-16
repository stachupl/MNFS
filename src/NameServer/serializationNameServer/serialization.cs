using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using FileTreeLibrary;

namespace serializationNameServer
{
    public class fileServers
    {
        public fileServers()
        {
            ip = "";
            port = "";
        }

        public fileServers(string ip, string port)
        {
            this.ip = ip;
            this.port = port;
        }

        public string ip
        {
            get { return fs_ip; }
            set { fs_ip = value; }
        }

        public string port
        {
            get { return fs_port; }
            set { fs_port = value; }
        }

        private string fs_ip;
        private string fs_port;
    }

    public class serialization
    {
        public FileTree deserialize()
        {
            FileTree fileTree;
            FileStream fs;
            BinaryFormatter bf = new BinaryFormatter();

            if (File.Exists("D:\\nameServer\\fileTree.dat"))
            {
                try
                {
                    fs = new FileStream("D:\\nameServer\\fileTree.dat", FileMode.Open);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Can't open existing file with file system tree!");
                    Console.WriteLine(e.Message);
                    return null;
                }

                fileTree = (FileTree)bf.Deserialize(fs);
                fs.Close();
            }
            else
            {
                try
                {
                    fs = new FileStream("D:\\nameServer\\fileTree.dat", FileMode.Create);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Can't create file with file system tree!");
                    Console.WriteLine(e.Message);
                    return null;
                }

                fileTree = new FileTree();
                bf.Serialize(fs, fileTree);
                fs.Close();
            }

            return fileTree;
        }

        public bool serialize(FileTree tree)
        {
            try
            {
                FileStream fs = new FileStream("D:\\nameServer\\fileTree.dat", FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(fs, tree);
                fs.Close();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Can't serialize file!");
                Console.WriteLine(e.Message);

                return false;
            }            
        }

        public List<fileServers> readFileServersList()
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load("D:\\nameServer\\fileServers.xml");
            }
            catch (Exception e)
            {
                Console.WriteLine("Can't open fileServer.xml!");
                Console.WriteLine(e.Message);
                return null;
            }

            List<fileServers> listOfFileServers = new List<fileServers>();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            XmlNodeReader nodder = new XmlNodeReader(doc);
            XmlReader reader = XmlReader.Create(nodder, settings);
            string ip = "";
            string port = "";

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "ip")
                    {
                        reader.Read();
                        ip = reader.Value;
                    }

                    if (reader.Name == "port")
                    {
                        reader.Read();
                        port = reader.Value;
                    }
                }

                if (ip != "" && port != "")
                {
                    listOfFileServers.Add(new fileServers(ip, port));
                    ip = "";
                    port = "";
                }
            }

            return listOfFileServers;
        }
    }
}
