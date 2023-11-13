using System.Threading.Tasks;

namespace AwsFeatureFlags;

internal static class TaskExtensions
{
    /// <summary>
    /// Wait for a task to finish, returning it's result.
    /// </summary>
    /// <param name="task">The task to wait on.</param>
    /// <typeparam name="T">The return type of the task.</typeparam>
    /// <returns>The result of the task, or throws any exception the task throws.</returns>
    public static T WaitFor<T>(this Task<T> task)
    {
        task.Wait();
        return task.Result;
    }
}