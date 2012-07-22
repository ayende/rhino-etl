using Rhino.Etl.Core.Operations;

namespace Rhino.Etl.Dsl.Macros
{
    /// <summary>
    /// 
    /// </summary>
    public class FullJoinMacro : HashJoinMacro
    {
        /// <summary>
        /// 
        /// </summary>
        public FullJoinMacro()
            : base(JoinType.Full)
        { }
    }
}