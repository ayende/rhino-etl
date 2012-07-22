using Rhino.Etl.Core.Operations;

namespace Rhino.Etl.Dsl.Macros
{
    /// <summary>
    /// 
    /// </summary>
    public class LeftJoinMacro : HashJoinMacro
    {
        /// <summary>
        /// 
        /// </summary>
        public LeftJoinMacro()
            : base(JoinType.Left)
        { }
    }
}