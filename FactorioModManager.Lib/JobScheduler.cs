using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FactorioModManager.Lib.Contracts;
using FactorioModManager.Lib.Models;

namespace FactorioModManager.Lib
{
    class JobScheduler : IJobTracker, IJobDirector
    {
        private readonly ConcurrentBag<Job> _runningJobs = new ConcurrentBag<Job>();

        // Like TaskScheduler, but for tasks wrapped in jobs

        public void TrackProgress(Job job)
        {
            if (job == null)
                throw new ArgumentNullException("job");

            _runningJobs.Add(job);
        }
    }
}
