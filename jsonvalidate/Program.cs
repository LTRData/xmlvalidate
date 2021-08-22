using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using JsonFormatting = Newtonsoft.Json.Formatting;

namespace jsonvalidate
{
    public class Program
    {
        public static void Main(params string[] args)
        {
            var fromxml = false;
            var toxml = false;
            var xnodeSaveOptions = SaveOptions.None;
            var jtokenFormatting = JsonFormatting.Indented;

            while (args != null && args.Length >= 1)
            {
                if (args[0].Equals("--fromxml", StringComparison.Ordinal))
                {
                    fromxml = true;
                }
                else if (args[0].Equals("--toxml", StringComparison.Ordinal))
                {
                    toxml = true;
                }
                else if (args[0].Equals("--noindent", StringComparison.Ordinal))
                {
                    jtokenFormatting = JsonFormatting.None;
                    xnodeSaveOptions = SaveOptions.DisableFormatting;
                }
                else if (args[0].Equals("--", StringComparison.Ordinal))
                {
                    args = args.Skip(1).ToArray();
                    break;
                }
                else
                {
                    break;
                }

                args = args.Skip(1).ToArray();
            }

            Func<string, string> in_converter = input => input;

            if (fromxml)
            {
                in_converter += input => JsonConvert.SerializeXNode(XElement.Parse(input));
            }

            Func<string, JToken> in_deserializer = JToken.Parse;

            Func<JToken, string> out_serializer = output => output.ToString(jtokenFormatting);

            if (toxml)
            {
                out_serializer = jtoken =>
                {
                    if (jtoken is not JObject jobject)
                    {
                        jobject = new JObject
                        {
                            { "arrayElement", jtoken }
                        };
                        return JsonConvert.DeserializeXNode(jobject.ToString(), "array").ToString(xnodeSaveOptions);
                    }
                    else if (jobject.Count > 1)
                    {
                        return JsonConvert.DeserializeXNode(jobject.ToString(), "root").ToString(xnodeSaveOptions);
                    }
                    else
                    {
                        return JsonConvert.DeserializeXNode(jobject.ToString()).ToString(xnodeSaveOptions);
                    }
                };
            }

            if (args == null || args.Length == 0)
            {
                try
                {
                    var jdoc = in_deserializer(in_converter(new StreamReader(Console.OpenStandardInput(), detectEncodingFromByteOrderMarks: true).ReadToEnd()));

                    Console.Write(out_serializer(jdoc));
                }
                catch (Exception ex)
                {
                    while (ex != null)
                    {
                        Console.Error.WriteLine(ex.GetType().Name + ": " + ex.Message);
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
                        var jdoc = in_deserializer(in_converter(arg));

                        Console.Write(out_serializer(jdoc));
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("Errors in '" + arg + "'");
                        while (ex != null)
                        {
                            Console.Error.WriteLine(ex.GetType().Name + ": " + ex.Message);
                            ex = ex.InnerException;
                        }
                    }
                }
            }

        }
    }
}
