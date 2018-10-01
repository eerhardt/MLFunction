using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;

namespace MLFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            //GitHubIssue issue = data.Issue;
            //List<object> labels = issue.Labels;

            //if (data.Action == "opened" && labels.Count == 0)
            //{
            string title = "msbuild /T:BuildAndTest /P:TargetGroup=netfx throws PNSE with Ref-Emit";
                int number = 31485;
                #region body
                string body = @"Running `msbuild /T:BuildAndTest /P:TargetGroup=netfx` causes a PNSE to be thrown in the following example.

This is a real example from the VB tests that had to be nerfed because of this
```cs
public static IEnumerable<object[]> InvalidBool_TestData()
{
    if (PlatformDetection.IsReflectionEmitSupported)
    {
        object floatEnum = null;
        try
        {
            floatEnum = FloatEnum;
        }
        catch (PlatformNotSupportedException)
        {
            yield break;
        }

        yield return new object[] { floatEnum };
        yield return new object[] { DoubleEnum };
        yield return new object[] { BoolEnum };
        yield return new object[] { CharEnum };
        yield return new object[] { IntPtrEnum };
        yield return new object[] { UIntPtrEnum };
    }
}
private static object s_floatEnum;

public static object FloatEnum
{
    get
    {
        if (s_floatEnum == null)
        {
            AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(""Name""), AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder module = assembly.DefineDynamicModule(""Name"");

                EnumBuilder eb = module.DefineEnum(""CharEnumType"", TypeAttributes.Public, typeof(float));
                eb.DefineLiteral(""A"", 1.0f);
                eb.DefineLiteral(""B"", 2.0f);
                eb.DefineLiteral(""C"", 3.0f);

                s_floatEnum = Activator.CreateInstance(eb.CreateTypeInfo());
            }

            return s_floatEnum;
        }
    }

    private static object s_doubleEnum;

    public static object DoubleEnum
    {
        get
        {
            if (s_doubleEnum == null)
            {
                AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(""Name""), AssemblyBuilderAccess.RunAndCollect);
                ModuleBuilder module = assembly.DefineDynamicModule(""Name"");

                EnumBuilder eb = module.DefineEnum(""CharEnumType"", TypeAttributes.Public, typeof(double));
                eb.DefineLiteral(""A"", 1.0);
                eb.DefineLiteral(""B"", 2.0);
                eb.DefineLiteral(""C"", 3.0);

                s_doubleEnum = Activator.CreateInstance(eb.CreateTypeInfo());
            }

            return s_doubleEnum;
        }
    }

    private static object s_boolEnum;
```";
                #endregion
                log.Info($"A {number.ToString()} issue with {title} has been opened.");

                var corefxIssue = new GitHubIssue
                {
                    ID = number.ToString(),
                    Title = title,
                    Description = body
                };

                string label = await Predictor.PredictAsync(corefxIssue, log);
                log.Info($"Labeling completed: {label}");
            //}
            //else
            //{
            //    log.Info($"The issue {issue.Number.ToString()} is already opened or it already has a label");
            //}

            return new OkObjectResult(label);
        }
    }
}
