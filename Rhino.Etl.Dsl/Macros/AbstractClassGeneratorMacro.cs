namespace Rhino.Etl.Dsl.Macros
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Module = Boo.Lang.Compiler.Ast.Module;

    /// <summary>
    /// Allow to easily generate a class from the DSL file
    /// </summary>
    /// <typeparam name="T">Base class</typeparam>
    public abstract class AbstractClassGeneratorMacro<T> : AbstractAstMacro
    {
        private readonly string blockMethodName;

        /// <summary>
        /// Gets the start index of the arguments collection.
        /// </summary>
        protected int argumentStartIndex = 0;

        private ClassDefinition classDefinition;

        private bool isAnonymous = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractClassGeneratorMacro&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="blockMethodName">Name of the method to move the block to, null if this is not permitted</param>
        protected AbstractClassGeneratorMacro(string blockMethodName)
        {
            this.blockMethodName = blockMethodName;
        }

        /// <summary>
        /// Gets the name of the macro.
        /// </summary>
        /// <value>The name of the macro.</value>
        private string MacroName
        {
            get { return GetType().Name; }
        }

        private IList<ParameterDeclaration> BuildParameters(MethodInfo method)
        {
            List<ParameterDeclaration> list = new List<ParameterDeclaration>();
            foreach (ParameterInfo info in method.GetParameters())
            {
                ParameterDeclaration declaration =
                    new ParameterDeclaration(info.Name, CodeBuilder.CreateTypeReference(info.ParameterType));
                list.Add(declaration);
            }
            return list;
        }

        /// <summary>
        /// Expands the specified macro
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <returns></returns>
        public override Statement Expand(MacroStatement macro)
        {
            classDefinition = CreateClassDefinition(macro);

            CreateMethodFromMacroBlock(macro, classDefinition);

            Module ancestor = (Module)macro.GetAncestor(NodeType.Module);
            ancestor.Members.Add(classDefinition);

            AddMemberMethods(macro);

            if (isAnonymous == false)
                return null;

            ReferenceExpression typeName = AstUtil.CreateReferenceExpression(GetClassName(macro));
            MethodInvocationExpression createInstance = new MethodInvocationExpression(typeName);
            return new ExpressionStatement(createInstance);
        }

        private void AddMemberMethods(MacroStatement macro)
        {
            if (Members(macro) != null)
            {
                foreach (Method method in Members(macro))
                {
                    classDefinition.Members.Add(method);
                }
            }
        }

        private ClassDefinition CreateClassDefinition(MacroStatement macro)
        {
            ClassDefinition classDefinition = new ClassDefinition(macro.LexicalInfo);
            classDefinition.BaseTypes.Add(new SimpleTypeReference(typeof(T).FullName));
            classDefinition.Name = GetClassName(macro);
            Constructor ctor = new Constructor();
            MethodInvocationExpression super = new MethodInvocationExpression(new SuperLiteralExpression());
            ctor.Body.Add(super);

            MoveConstructorArguments(ctor, super, macro);
            classDefinition.Members.Add(ctor);
            return classDefinition;
        }

        /// <summary>
        /// Gets the method to override.
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <returns></returns>
        private MethodInfo GetMethodToOverride(Node macro)
        {
            MethodInfo overridenMethod = null;
            if (blockMethodName != null)
            {
                try
                {
                    overridenMethod =
                        typeof(T).GetMethod(blockMethodName,
                                             BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                }
                catch (AmbiguousMatchException)
                {
                    string msg = typeof (T).Name + " has more than one overload for method " + blockMethodName;
                    Errors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo, msg));
                
                }
                catch (Exception exception)
                {
                    string msg =
                        string.Format("Error occured when trying to get method '{0}' on {1}. {2}",
                        blockMethodName, typeof(T).Name, exception);
                    Errors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo, msg));
                }
                if (overridenMethod == null)
                {
                    Errors.Add(
                        CompilerErrorFactory.CustomError(macro.LexicalInfo,
                                                         "Could not find " + blockMethodName + " on " +
                                                         typeof(T).FullName));
                }
            }
            return overridenMethod;
        }

        private void CreateMethodFromMacroBlock(MacroStatement macro, TypeDefinition classDefinition)
        {
            MethodInfo methodToOverride = GetMethodToOverride(macro);

            if (macro.Body != null && macro.Body.Statements.Count > 0)
            {
                if (methodToOverride == null)
                {
                    Errors.Add(
                        CompilerErrorFactory.CustomError(macro.LexicalInfo, MacroName + " cannot be use with a block"));
                }
                Method method = new Method(blockMethodName);
                method.Modifiers = TypeMemberModifiers.Override;
                method.Body = macro.Body;
                classDefinition.Members.Add(method);
                method.Parameters.Extend(BuildParameters(methodToOverride));
            }
        }

        /// <summary>
        /// Gets the name of the class that we will generate
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <returns></returns>
        protected virtual string GetClassName(MacroStatement macro)
        {
            if (macro.Arguments.Count == 0 || (macro.Arguments[0] is ReferenceExpression) == false)
            {
                return GetAnonymousClassName(macro);
            }
            argumentStartIndex = 1;

            ReferenceExpression referenceExpression = macro.Arguments[0] as ReferenceExpression;
            if (referenceExpression == null)
            {
                Errors.Add(
                    CompilerErrorFactory.CustomError(macro.LexicalInfo,
                                                     GetType().Name + " first parameter must be a valid name."));
                return null;
            }
            return referenceExpression.Name;
        }

        /// <summary>
        /// Gets the name of the anonymous class.
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <returns></returns>
        private string GetAnonymousClassName(MacroStatement macro)
        {
            if (macro.GetAncestor(NodeType.MacroStatement) == null)
            {
                Errors.Add(
                    CompilerErrorFactory.CustomError(macro.LexicalInfo,
                                                     GetType().Name + " must have a single parameter, the name of the " +
                                                     GetType().Name));
                return null;
            }
            isAnonymous = true;
            string name = typeof(T).Name.Replace("Abstract", "").Replace("Operation", "");
            if (macro["anonymous_name_index"] == null)
                macro["anonymous_name_index"] = Context.GetUniqueName();
            return "Anonymous_" + name + "_" + macro["anonymous_name_index"];
        }

        /// <summary>
        /// Moves the constructor arguments from the macro to the superInvocation method invocation
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <param name="superInvocation">The create.</param>
        /// <param name="macro">The macro.</param>
        protected void MoveConstructorArguments(
            Constructor constructor,
            MethodInvocationExpression superInvocation,
            MacroStatement macro)
        {
            for (int i = argumentStartIndex; i < macro.Arguments.Count; i++)
            {
                Expression argument = macro.Arguments[i];
                BinaryExpression assign;
                MethodInvocationExpression call;
                if (TryGetAssignment(argument, out assign))
                {
                    constructor.Body.Add(assign);
                }
                else if(TryGetCall(argument, out call))
                {
                    constructor.Body.Add(call);
                }
                else
                {
                    superInvocation.Arguments.Add(argument);
                }
            }
        }


        /// <summary>
        /// Tries to get an assignment from the expression
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="assign">The assign.</param>
        /// <returns></returns>
        protected static bool TryGetAssignment(Expression expression, out BinaryExpression assign)
        {
            assign = expression as BinaryExpression;
            return (assign != null && assign.Operator == BinaryOperatorType.Assign);
        }

        /// <summary>
        /// Tries to get a method call from the expression
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="call">The method call.</param>
        /// <returns></returns>
        protected static bool TryGetCall(Expression expression, out MethodInvocationExpression call)
        {
            call = expression as MethodInvocationExpression;
            return (call != null && call.Target is ReferenceExpression);
        }

        /// <summary>
        /// Add a method definition to the resultant class definition
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="method"></param>
        protected void AddMethodDefinitionToClassDefinition(MacroStatement macro, Method method)
        {
            var members = (IList<Method>)macro["members"];
            if (members == null)
                macro["members"] = members = new List<Method>();
            members.Add(method);
        }

        /// <summary>
        /// Get the members collection from this macro
        /// </summary>
        /// <param name="macro">The macro.</param>
        /// <returns></returns>
        protected static IList<Method> Members(MacroStatement macro)
        {
            return (IList<Method>)macro["members"];
        }
    }
}