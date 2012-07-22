using Rhino.Etl.Core.Operations;

namespace Rhino.Etl.Dsl.Macros
{
    /// <summary>
    /// 
    /// </summary>
    public class RightJoinMacro : HashJoinMacro
    {
        /// <summary>
        /// 
        /// </summary>
        public RightJoinMacro()
            : base(JoinType.Right)
        { }
    }
}