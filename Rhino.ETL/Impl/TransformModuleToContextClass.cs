using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;

namespace Rhino.ETL.Impl
{
	using Engine;

	public class TransformModuleToContextClass : AbstractNamespaceSensitiveVisitorCompilerStep
	{
		private readonly string[] imports;

		public TransformModuleToContextClass(string[] imports)
		{
			this.imports = imports;
		}

		public override void Run()
		{
			Visit(this.CompileUnit);

			ClassDefinition definition = new ClassDefinition();
			definition.Name = CompileUnit.Modules[0].Name;
			definition.BaseTypes.Add(new SimpleTypeReference(typeof(EtlConfigurationContext).FullName));
			Method method = new Method("BuildConfig");
			definition.Members.Add(method);

			foreach (Module module in this.CompileUnit.Modules)
			{
				method.Body.Add(module.Globals);
				module.Globals= new Block();
			}

			Property property = new Property("Name");
			property.Getter = new Method("getter_Name");
			property.Getter.Body.Add(
				new ReturnStatement(
					new StringLiteralExpression(definition.Name)
					)
				);
			definition.Members.Add(property);
			CompileUnit.Modules[0].Members.Add(definition);
		}
		public override void OnModule(Module module)
		{
			foreach (string theNamespace in imports)
			{
				Import import = new Import();
				import.Namespace = theNamespace;
				module.Imports.Add(import);
			}
		}
	}
}