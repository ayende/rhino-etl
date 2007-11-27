using Boo.Lang.Compiler.Ast;

namespace Rhino.ETL.Impl
{
	public class CodeHelper
	{
		public static Method GetMethod(Block block)
		{
			Node node = block.ParentNode;
			while (!(node is Method))
				node = node.ParentNode;
			return (Method)node;
		}

	}
}