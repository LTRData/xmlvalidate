using System;
using System.Xml;
using System.Xml.Linq;

namespace xmlvalidate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                try
                {
                    using var reader = XmlReader.Create(Console.OpenStandardInput());
                    var xdoc = XDocument.Load(reader);
                    Console.WriteLine(xdoc.ToString());
                }
                catch (Exception ex)
                {
                    while (ex != null)
                    {
                        Console.Error.WriteLine($"{ex.GetType().Name}: {ex.Message}");
                        ex = ex.InnerException;
                    }
                }
            }
            else
            {
                foreach (var arg in args)
                {
                    try
                    {
                        var xdoc = XDocument.Load(arg);
                        Console.WriteLine(xdoc.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Errors in {arg}");
                        while (ex != null)
                        {
                            Console.Error.WriteLine($"{ex.GetType().Name}: {ex.Message}");
                            ex = ex.InnerException;
                        }
                    }
                }
            }
        }
    }
}
