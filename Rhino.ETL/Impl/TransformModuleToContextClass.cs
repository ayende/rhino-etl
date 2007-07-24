using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;

namespace Rhino.ETL.Impl
{
	public class TransformModuleToContextClass : AbstractNamespaceSensitiveVisitorCompilerStep
	{
		private string[] imports;

		public TransformModuleToContextClass(string[] imports)
		{
			this.imports = imports;
		}

		public override void Run()
		{
			Visit(CompileUnit);
		}

		public override void OnModule(Module module)
		{
			foreach (string theNamespace in imports)
			{
				Import import = new Import();
				import.Namespace = theNamespace;
				module.Imports.Add(import);
			}
			ClassDefinition definition = new ClassDefinition();
			definition.Name = module.FullName;
			definition.BaseTypes.Add(new SimpleTypeReference(typeof(EtlConfigurationContext).FullName));
			Method method = new Method("BuildConfig");
			method.Body = module.Globals;
			module.Globals = new Block();
			definition.Members.Add(method);

			Property property = new Property("Name");
			property.Getter = new Method("getter_Name");
			property.Getter.Body.Add(
				new ReturnStatement(
					new StringLiteralExpression(definition.Name)
					)
				);
			definition.Members.Add(property);

			module.Members.Add(definition);
		}
	}
}