using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Services.Description;
using System.Web.Services.Protocols;

namespace WacVisitor
{
    public class classWebservice
    {
        public object CallWebService(string webServiceAsmxUrl, string methodName, object[] args)
        {
            try
            {
                System.Net.WebClient client = new System.Net.WebClient();
                System.IO.Stream stream = client.OpenRead(webServiceAsmxUrl + "?wsdl");
                ServiceDescription description = ServiceDescription.Read(stream);
                ServiceDescriptionImporter importer = new ServiceDescriptionImporter();
                importer.ProtocolName = "Soap12"; //' Use SOAP 1.2.
                importer.AddServiceDescription(description, null, null);
                importer.Style = ServiceDescriptionImportStyle.Client;
                importer.CodeGenerationOptions = System.Xml.Serialization.CodeGenerationOptions.GenerateProperties;
                CodeNamespace nwspace = new CodeNamespace();
                CodeCompileUnit unit1 = new CodeCompileUnit();
                unit1.Namespaces.Add(nwspace);
                ServiceDescriptionImportWarnings warning = importer.Import(nwspace, unit1);

                if (warning == 0)
                {
                    CodeDomProvider provider1 = CodeDomProvider.CreateProvider("CSharp");

                    String[] assemblyReferences;
                    assemblyReferences = new String[] { "System.dll", "System.Web.Services.dll", "System.Web.dll", "System.Xml.dll", "System.Data.dll" };

                    CompilerParameters parms = new CompilerParameters(assemblyReferences);
                    parms.GenerateInMemory = true;
                    CompilerResults results = provider1.CompileAssemblyFromDom(parms, unit1);
                    if (results.Errors.Count > 0)
                    {
                    }


                    Type foundType = null;
                    Type[] types = results.CompiledAssembly.GetTypes();
                    foreach (Type type1 in types)
                    {
                        if (type1.BaseType == typeof(SoapHttpClientProtocol))
                        {
                            foundType = type1;
                        }
                    }

                    Object wsvcClass = results.CompiledAssembly.CreateInstance(foundType.ToString());
                    MethodInfo mi = wsvcClass.GetType().GetMethod(methodName);
                    var returnValue = mi.Invoke(wsvcClass, args);

                    return returnValue;

                }
                else
                {
                    return null;
                }

            }
            catch
            {
                return null;
            }

        }
    }
}
