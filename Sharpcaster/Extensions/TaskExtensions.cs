using System;
using System.Threading.Tasks;

namespace Sharpcaster.Extensions
{
    /// <summary>
    /// Extensions methods for Task class
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Throws a TimeoutException if a task is not completed before a delay
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="task">task</param>
        /// <param name="delay">the delay in milliseconds</param>
        /// <returns>T</returns>
        public static async Task<T> TimeoutAfter<T>(this Task<T> task, int delay)
        {
            await Task.WhenAny(task, Task.Delay(delay));
            if (!task.IsCompleted)
            {
                throw new TimeoutException();
            }

            return await task;
        }

        /// <summary>
        /// Throws a TimeoutException if a task is not completed before a delay
        /// </summary>
        /// <param name="task">task</param>
        /// <param name="delay">the delay in milliseconds</param>
        /// <returns>T</returns>
        public static async Task TimeoutAfter(this Task task, int delay)
        {
            await Task.WhenAny(task, Task.Delay(delay));
            if (!task.IsCompleted)
            {
                throw new TimeoutException();
            }

            await task;
        }
    }
}
