using System;
using System.Linq;

namespace FactorioModManager.Lib.Models
{
    public class JobResult
    {
        public virtual string[] ErrorMessages { get; private set; }

        public virtual bool IsSuccess => !ErrorMessages.Any();

        public static readonly JobResult Success = new JobResult();

        private static readonly string[] EmptyErrorMessagesSingleton = new string[0];

        public JobResult()
        {
            ErrorMessages = EmptyErrorMessagesSingleton;
        }

        public JobResult(params string[] errorMessages)
        {
            if (errorMessages == null)
                throw new ArgumentNullException("errorMessages");

            if(errorMessages.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentNullException("errorMessages", "The array contained an element that was null or whitespace.");
            
            ErrorMessages = errorMessages;
        }
    }
}
