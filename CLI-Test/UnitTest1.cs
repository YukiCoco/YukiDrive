using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YukiDrive.CLI.Services;
using System.Collections.Generic;

namespace YukiDrive.CLI.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestFile()
        {
            string fileName = "/Users/yukino/Desktop/NewFile.txt";
            int maxBuffer = 6553600; //6400 kib 微软推荐使用
            if (!File.Exists(fileName))
                throw new ArgumentException("找不到文件所在路径");

            //httpClient.DefaultRequestHeaders.Add("Content-Range","bytes ");
            var fileStream = File.OpenRead(fileName);
            if (!fileStream.CanRead)
            {
                throw new ArgumentException("缺少文件读取权限");
            }
            int result = 0;
            long offset = 0;
            //判断文件大小
            if (maxBuffer > fileStream.Length)
                maxBuffer = (int)fileStream.Length;
            //Debug.WriteLine(fileStream.Length);
            do
            {
                byte[] fileBytes = new byte[maxBuffer];
                result = fileStream.Read(fileBytes, 0, maxBuffer);
                //ByteArrayContent uploadContent = new ByteArrayContent(fileBytes);
                //await httpClient.PutAsync(url,uploadContent);
                long nextBytes = offset + result;
                if (result > 0)
                {
                    Debug.WriteLine("Content-Range", $"{offset}-{nextBytes - 1}/{fileStream.Length}");
                    double progress = Math.Round((double)(offset + result) / (double)fileStream.Length, 2);
                    Debug.WriteLine($"当前上传进度为：{progress}");
                    offset += result;
                }
            }
            while (result > 0);
        }
        [TestMethod]
        public void TestMethod1()
        {
            byte[] bytes = new byte[3];
            Debug.WriteLine(bytes.Length);
        }

        [TestMethod]
        public void TestService()
        {
            HttpService service = new HttpService(new HttpClient(), new SettingService());
            //string uploadUrl = service.GetUploadUrl("upload/October - Time To Love.mp3").Result;
            //Debug.WriteLine(uploadUrl);
            service.UploadFolder("/Users/yukino/Desktop/TestUpload","TestUpload","onedrive",3);
            while (true)
            {
                Thread.Sleep(1000);
            }
            //service.UploadFile(uploadUrl, "/Users/yukino/Desktop/October - Time To Love.mp3").Wait();
        }

        [TestMethod]
        public void TestUploadTask()
        {
            // TaskScheduler taskScheduler = new LimitedConcurrencyLevelTaskScheduler(2);
            // TaskFactory taskFactory = new TaskFactory(taskScheduler);
            // for (int i = 0; i < 10; i++)
            // {
            //     taskFactory.StartNew(() =>
            //     {
            //         Thread.Sleep(TimeSpan.FromSeconds(2));
            //         Debug.WriteLine($"{Thread.CurrentThread.ManagedThreadId}");
            //     });
            // }

            // Thread.Sleep(TimeSpan.FromSeconds(20));
            Debug.WriteLine(Path.GetFileName("/Users/yukino/Desktop/85677DCC-F7E1-4153-BE07-63D3FDA50365_1_105_c.jpeg"));
        }

        // Provides a task scheduler that ensures a maximum concurrency level while 
        // running on top of the thread pool.
        public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
        {
            // Indicates whether the current thread is processing work items.
            [ThreadStatic]
            private static bool _currentThreadIsProcessingItems;

            // The list of tasks to be executed 
            private readonly LinkedList<Task> _tasks = new LinkedList<Task>(); // protected by lock(_tasks)

            // The maximum concurrency level allowed by this scheduler. 
            private readonly int _maxDegreeOfParallelism;

            // Indicates whether the scheduler is currently processing work items. 
            private int _delegatesQueuedOrRunning = 0;

            // Creates a new instance with the specified degree of parallelism. 
            public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
            {
                if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
                _maxDegreeOfParallelism = maxDegreeOfParallelism;
            }

            // Queues a task to the scheduler. 
            protected sealed override void QueueTask(Task task)
            {
                // Add the task to the list of tasks to be processed.  If there aren't enough 
                // delegates currently queued or running to process tasks, schedule another. 
                lock (_tasks)
                {
                    _tasks.AddLast(task);
                    if (_delegatesQueuedOrRunning < _maxDegreeOfParallelism)
                    {
                        ++_delegatesQueuedOrRunning;
                        NotifyThreadPoolOfPendingWork();
                    }
                }
            }

            // Inform the ThreadPool that there's work to be executed for this scheduler. 
            private void NotifyThreadPoolOfPendingWork()
            {
                ThreadPool.UnsafeQueueUserWorkItem(_ =>
                {
                    // Note that the current thread is now processing work items.
                    // This is necessary to enable inlining of tasks into this thread.
                    _currentThreadIsProcessingItems = true;
                    try
                    {
                        // Process all available items in the queue.
                        while (true)
                        {
                            Task item;
                            lock (_tasks)
                            {
                                // When there are no more items to be processed,
                                // note that we're done processing, and get out.
                                if (_tasks.Count == 0)
                                {
                                    --_delegatesQueuedOrRunning;
                                    break;
                                }

                                // Get the next item from the queue
                                item = _tasks.First.Value;
                                _tasks.RemoveFirst();
                            }

                            // Execute the task we pulled out of the queue
                            base.TryExecuteTask(item);
                        }
                    }
                    // We're done processing items on the current thread
                    finally { _currentThreadIsProcessingItems = false; }
                }, null);
            }

            // Attempts to execute the specified task on the current thread. 
            protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                // If this thread isn't already processing a task, we don't support inlining
                if (!_currentThreadIsProcessingItems) return false;

                // If the task was previously queued, remove it from the queue
                if (taskWasPreviouslyQueued)
                    // Try to run the task. 
                    if (TryDequeue(task))
                        return base.TryExecuteTask(task);
                    else
                        return false;
                else
                    return base.TryExecuteTask(task);
            }

            // Attempt to remove a previously scheduled task from the scheduler. 
            protected sealed override bool TryDequeue(Task task)
            {
                lock (_tasks) return _tasks.Remove(task);
            }

            // Gets the maximum concurrency level supported by this scheduler. 
            public sealed override int MaximumConcurrencyLevel { get { return _maxDegreeOfParallelism; } }

            // Gets an enumerable of the tasks currently scheduled on this scheduler. 
            protected sealed override IEnumerable<Task> GetScheduledTasks()
            {
                bool lockTaken = false;
                try
                {
                    Monitor.TryEnter(_tasks, ref lockTaken);
                    if (lockTaken) return _tasks;
                    else throw new NotSupportedException();
                }
                finally
                {
                    if (lockTaken) Monitor.Exit(_tasks);
                }
            }
        }
    }
}
