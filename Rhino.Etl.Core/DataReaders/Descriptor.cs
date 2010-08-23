using System;

namespace Rhino.Etl.Core
{
    /// <summary>
    /// Represent a virtual property, with a type and name. 
    /// It also exposes the ability to get the "property" from a container.
    /// </summary>
    /// <remarks>
    /// This is needed because we want to use both types and untyped containers.
    /// Those can be entities, hashtables, etc.
    /// </remarks>
    public abstract class Descriptor
    {
        /// <summary>
        /// The name of this descriptor
        /// </summary>
        public string Name;
        /// <summary>
        /// The type fo this descriptor
        /// </summary>
        public Type Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="Descriptor"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        public Descriptor(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Gets the value from the container
        /// </summary>
        /// <param name="container">The container.</param>
        public abstract object GetValue(object container);
    }
}