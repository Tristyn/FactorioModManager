using System.Threading.Tasks;

namespace FactorioModManager.Lib.Models
{
    internal class JobFactory
    {
        private readonly JobScheduler _scheduler;

        public JobFactory(JobScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public Job FromTask(Task task)
        {
            var job = new Job(task);

            _scheduler.TrackProgress(job);

            return job;
        }

        public Job<TResult> FromTask<TResult>(Task<TResult> task)
        {
            var job = new Job<TResult>(task);

            _scheduler.TrackProgress(job);

            return job;
        }

        public Job<TResult> FromResult<TResult>(TResult result)
        {
            var job = new Job<TResult>(Task.FromResult(result));

            // Don't schedule and track a completed task

            return job;
        }

        public static Job CompletedJob => new Job(Task.CompletedTask);
    }
}
