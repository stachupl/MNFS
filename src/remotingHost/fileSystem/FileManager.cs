using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Threading;
using System.Text.RegularExpressions;

namespace FileSystem
{
    public class FileManager : MarshalByRefObject
    {
        private string path;

        public FileManager()
        {
            path = "D:\\fileSystem";
        }

        public string create()
        {
            Random randomNumber = new Random();
            string fileName = Regex.Replace(DateTime.Now.ToLongDateString(), @"\s", "");
            FileStream fs;

            while (File.Exists(path + "\\" + fileName) == true)
                fileName += randomNumber.Next().ToString();

            try
            {
                fs = File.Create(path + "\\" + fileName);
            }
            catch(Exception e)
            {
                Console.WriteLine("Can't create file!");
                Console.WriteLine(e.Message);

                return "";
            }

            fs.Close();

            return fileName;
        }

        public bool remove(string fileName)
        {
            bool removed = false;

            lock(this)
            {
                try
                {
                    File.Delete(path + "\\" + fileName);
                    removed = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Can't remove file: " + fileName);
                    Console.WriteLine(e.Message);
                }
            }

            return removed;            
        }

        public string[] read(string name, int i, int j)
        {
            string[] result = new string[j];
            int k;
            string readedLine;

            lock (this)
            {
                try
                {
                    StreamReader sr = new StreamReader(path + "\\" + name);
                    for (k = 1; k < i; k++)
                        sr.ReadLine();

                    for (k = 0; k < j; k++)
                    {
                        readedLine = sr.ReadLine();
                        if (readedLine != null)
                            result[k] = readedLine;
                        else
                            result[k] = null;
                    }

                    sr.Close();

                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Can't read file: " + name);
                    Console.WriteLine(e.Message);

                    return null;
                }
            }
        }

        public bool append(string name, string[] toAppend)
        {
            int i;

            lock (this)
            {
                try
                {
                    StreamWriter w = File.AppendText(path + "\\" + name);

                    //w.WriteLine("");
                    for (i = 0; i < toAppend.Length; i++)
                        w.WriteLine(toAppend[i]);

                    w.Close();

                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Can't append to file: " + name);
                    Console.WriteLine(e.Message);

                    return false;
                }
            }
        }

        public bool write(string name, string[] toWrite)
        {
            lock (this)
            {
                try
                {

                    StreamWriter sw = new StreamWriter(path + "\\" + name);
                    
                    foreach (string line in toWrite)
                        sw.WriteLine(line);

                    sw.Close();

                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Can't write to file: " + name);
                    Console.WriteLine(e.Message);

                    return false;
                }
            }
        }

        public ArrayList cat(string name)
        {
            ArrayList result = new ArrayList();

            lock(this)
            {
                try
                {
                    StreamReader sr = new StreamReader(path + "\\" + name);

                    while (sr.Peek() >= 0)
                        result.Add(sr.ReadLine());

                    return result;
                }
                catch(Exception e)
                {
                    Console.WriteLine("Can't cat file: " + name);
                    Console.WriteLine(e.Message);

                    return null;
                }
            }
        }       
    }
}

/* TO do */
// obczaić czy dobrze działają blokady przy plikach