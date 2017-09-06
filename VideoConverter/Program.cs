using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using NReco.VideoConverter;

namespace VideoConverter
{
    class Program
    {
        static string GetPath(string[] args)
        {
            if (args.Length > 0)
            {
                return args[0];
            }
            throw new Exception("Не указан каталог конвертации");
        }

        static string[] GetFromArgsFormats(string[] args)
        {
            return args.Length > 1 ? args[1].Split(',') : throw new Exception("Не указаны форматы конвертации");
        }

        static bool GetFromArgsDeleteNotConverted(string[] args)
        {
            if (args.Length > 2)
            {
                return args[2] == "-d";
            }
            return false;
        }


        static void Main(string[] args)
        {
            try
            {
                string path = GetPath(args);
                bool isDeleteNotConverted = GetFromArgsDeleteNotConverted(args);
                string[] formats = GetFromArgsFormats(args);
                Convert(path, formats, isDeleteNotConverted);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }
        }

        private static void Convert(string path, string[] ext, bool delNotConv)
        {
            var files = new List<string>();
            GetFiles(path, ref files);
            if(files.Count > 0)
            {
                Console.WriteLine("Конвертиция файлов в mp4 началась");
                var err = new List<string>();
                for (int i = 0; i < files.Count; i++)
                {
                    string tmpFile = string.Empty;
                    try
                    {
                        var finfo = new FileInfo(files[i]);
                        if (ext.Contains(finfo.Extension))
                        {
                            Console.WriteLine($"{i}/{files.Count} - {files[i]} ");
                            tmpFile = files[i] + Guid.NewGuid();
                            FFMpegConverter ffMpeg = new FFMpegConverter();
                            ffMpeg.ConvertMedia(files[i], tmpFile, Format.mp4);
                            File.Delete(files[i]);
                            File.Move(tmpFile, files[i]);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(e.Message);
                        err.Add($"Ошибка конвертации файла {i}/{files.Count} - {files[i]} ({e.Message})");
                        if (delNotConv)
                        {
                            Console.WriteLine($"Файл будет удален {i}/{files.Count} - {files[i]}");
                            try
                            {
                                File.Delete(files[i]);
                                if(!string.IsNullOrEmpty(tmpFile))
                                    try { File.Delete(tmpFile); }catch(Exception ex) { }

                            }
                            catch (Exception ex)
                            {
                                var msg = $"Не удалось удалить файл {i}/{files.Count} - {files[i]} ({ex.Message})";
                                Console.WriteLine(msg);
                                err.Add(msg);
                            }
                        }
                        Console.ResetColor();
                    }
                }
               
                Console.WriteLine("Конвертиция файлов завершена");
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var e in err)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                Console.WriteLine("Файлов для конвертации не обнаружено.");
            }
            Console.WriteLine("Нажмите любую клавишу ....");
            Console.ReadKey();

        }

        private static void GetFiles(string path, ref List<string> res)
        {
            if (Directory.Exists(path))
            {
                foreach (string f in Directory.GetFiles(path))
                {
                    GetFiles(f, ref res);
                }

                foreach (string d in Directory.GetDirectories(path))
                {
                    GetFiles(d, ref res);
                }
            }

            if (new FileInfo(path).Exists)
            {
                res.Add(path);
            }
        }

    }


   
}
