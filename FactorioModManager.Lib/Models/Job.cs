using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FactorioModManager.Lib.Models
{
    public class Job
    {
        public Task Task { get; private set; }

        public Job(Task task)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            
            Task = task;
        }
        
        public TaskAwaiter GetAwaiter()
        {
            return Task.GetAwaiter();
        }
    }

    public class Job<TResult> : Job
    {
        public new Task<TResult> Task { get; private set; }

        public Job(Task<TResult> task)
            : base(task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            Task = task;
        }

        public new TaskAwaiter<TResult> GetAwaiter()
        {
            return Task.GetAwaiter();
        } 
    }
}
