using System;
using System.Runtime.Serialization;

namespace Summer.Batch.Core.Unity
{
    /// <summary>
    /// Exception thrown when there are errors in the Unity configuration of the batch.
    /// </summary>
    public class RegistrationException : Exception
    {
        /// <summary>
        /// Constructs a new <see cref="RegistrationException"/> with the specified message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public RegistrationException(string message) : base(message) { }

        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        /// <param name="info">The info holding the serialization data.</param>
        /// <param name="context">The serialization context.</param>
        protected RegistrationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}