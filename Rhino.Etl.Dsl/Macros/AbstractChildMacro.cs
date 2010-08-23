namespace Rhino.Etl.Dsl.Macros
{
    using System;
    using System.Collections.Generic;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;

    /// <summary>
    /// Base class for child macros, also handle validating the parents of a macro
    /// </summary>
    public abstract class AbstractChildMacro : AbstractAstMacro
    {
        private readonly string[] allowedParents;
        /// <summary>
        /// The parent macro statement
        /// </summary>
        protected MacroStatement parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractChildMacro"/> class.
        /// </summary>
        /// <param name="allowedParents">The allowed parents.</param>
        public AbstractChildMacro(params string[] allowedParents)
        {
            this.allowedParents = allowedParents;
        }

        /// <summary>
        /// Expands the specified macro, validate that the parent is a valid parent, 
        /// leave the actual processing to a base class
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <returns></returns>
        public override Statement Expand(MacroStatement macro)
        {
            if (ValidateParent(macro, out parent) == false)
                return null;

            return DoExpand(macro);
        }

        /// <summary>
        /// Perform the actual expansion of the macro
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <returns></returns>
        protected abstract Statement DoExpand(MacroStatement macro);

        private bool ValidateParent(MacroStatement macro, out MacroStatement parent)
        {
            parent = macro.ParentNode.ParentNode as MacroStatement;

            if (parent != null)
            {
                foreach (string validParent in allowedParents)
                {
                    if (parent.Name.Equals(validParent, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            string msg = string.Format("A {0} statement can appear only under a {1}",
                                       macro.Name,
                                       string.Join(" | ", allowedParents));
            Errors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo, msg));

            return false;
        }


        /// <summary>
        /// Gets the parent methods collection
        /// </summary>
        /// <value>The parent methods.</value>
        protected IList<Method> ParentMethods
        {
            get
            {
                IList<Method> members = (IList<Method>)parent["members"];
                if (members == null)
                    parent["members"] = members = new List<Method>();
                return members;

            }
        }
    }
}