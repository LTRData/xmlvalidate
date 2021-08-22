using Microsoft.JScript;
using Microsoft.JScript.Vsa;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace jsonvalidate
{
    public class Program
    {
        public static int Main(params string[] args)
        {
            if (args == null || args.Length == 0)
            {
                try
                {
                    var provider = new JScriptCodeProvider();

                    var parameters = new CompilerParameters
                    {
                        GenerateExecutable = true,
                        GenerateInMemory = true
                    };

                    var results = provider.CompileAssemblyFromSource(parameters, Console.In.ReadToEnd());

                    if (results.Errors.Count >= 1)
                    {
                        foreach (var str in results.Errors)
                        {
                            Console.Error.WriteLine(str);
                        }

                        return -1;
                    }

                    foreach (var str in results.Output)
                    {
                        Console.Out.WriteLine(str);
                    }

                    return (int)results.CompiledAssembly.EntryPoint.Invoke(null, new object[] { new string[] { } });
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
                        var provider = new JScriptCodeProvider();

                        var parameters = new CompilerParameters
                        {
                            GenerateExecutable = true,
                            GenerateInMemory = true
                        };

                        var results = provider.CompileAssemblyFromSource(parameters, arg);

                        if (results.Errors.Count >= 1)
                        {
                            foreach (var str in results.Errors)
                            {
                                Console.Error.WriteLine(str);
                            }

                            continue;
                        }

                        foreach (var str in results.Output)
                        {
                            Console.Out.WriteLine(str);
                        }

                        if (results.CompiledAssembly.EntryPoint == null)
                        {
                            Console.Out.WriteLine("No entry point defined.");
                            return 0;
                        }

                        return (int)results.CompiledAssembly.EntryPoint.Invoke(null, new object[] { });
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Errors in '{arg}'");

                        Console.Error.WriteLine(ex.ToString());
                    }

                }
            }

            return 0;
        }
    }
}
