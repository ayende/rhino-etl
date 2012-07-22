using Rhino.Etl.Core.Operations;

namespace Rhino.Etl.Dsl.Macros
{
    /// <summary>
    /// 
    /// </summary>
    public class InnerJoinMacro : HashJoinMacro
    {
        /// <summary>
        /// 
        /// </summary>
        public InnerJoinMacro()
            : base(JoinType.Inner)
        { }
    }
}