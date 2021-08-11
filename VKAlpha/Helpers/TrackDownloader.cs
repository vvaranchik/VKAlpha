using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using RestSharp;
using VKAlpha.Extensions;
using Windows.Media.AppBroadcasting;

namespace VKAlpha.Helpers
{

    public static class TrackDownloaderEx
    {
        const byte MAX = 0x3;
        private static readonly Queue<AudioModel> _queue = new Queue<AudioModel>();
        private static readonly object _syncRoot = new object();

        private static Task<bool> Download(AudioModel item)
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);

            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                if (Directory.Exists($"Cache/download/"))
                {
                    var fileName = CacheService.GetSafeFileName(item.FullData);
                    var path = Path.Combine("Cache", $"download/{fileName}.mp3");
                    if (File.Exists(path))
                    {
                        MainViewModelLocator.MainViewModel.MessageQueue.Enqueue($"{item.FullData} already downloaded!");
                    }
                    else
                    {
                        MainViewModelLocator.MainViewModel.MessageQueue.Enqueue($"Starting download {item.FullData}");
                        var writer = File.OpenWrite(path);
                        var client = new RestClient(new Uri(item.Url));
                        var request = new RestRequest(Method.GET);
                        request.ResponseWriter = stream => { using (stream) { stream.CopyTo(writer); } };
                        client.DownloadData(request);
                        MainViewModelLocator.MainViewModel.MessageQueue.Enqueue($"{item.FullData} Download completed");
                    }
                }
                else
                {
                    MainViewModelLocator.MainViewModel.MessageQueue.Enqueue("Error! No directory for download!");
                }

            });
            return tcs.Task;
        }

        private static void Process()
        {
            while(_queue.Count > 0)
            {

                List<AudioModel> items = new List<AudioModel>();

                lock (_syncRoot)
                {
                    for (var i = 0; i < MAX; i++)
                    {
                        items.Add(_queue.Dequeue());
                    }
                }
                var tasks = new List<Task<bool>>();

                foreach(var audio in items)
                {
                    if (audio)
                    {
                        tasks.Add(Download(audio));
                    }
                }

                var runningTasks = new List<Task<bool>>();

                foreach (var queueItem in tasks)
                {
                    if (queueItem == null)
                        continue;
            //        runningTasks.Add(Download(queueItem));
                }

            //    var tracks = await Task.WhenAll(runningTasks);

            }
        }

        public static void AddToQueue(AudioModel item)
        {
            lock (_syncRoot)
            {
                if (!_queue.Contains(item))
                {
                    _queue.Enqueue(item);
                }
            }
            Task.Run(() => Process());
        }

    }
    
    public static class TrackDownloader
    {
        const byte MAX_PARALLEL_ITEMS = 0x3;
        private static readonly Dictionary<long, List<AudioModel>> _requestQueue = new Dictionary<long, List<AudioModel>>();
        private static readonly object _syncRoot = new object();

        private static Task<long> DownloadAsync(AudioModel item)
        {
            var tcs = new TaskCompletionSource<long>();

            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                if (Directory.Exists($"Cache/download/"))
                {
                    var fileName = CacheService.GetSafeFileName(item.FullData);
                    var path = Path.Combine("Cache", $"download/{fileName}.mp3");
                    if (File.Exists(path))
                    {
                        MainViewModelLocator.MainViewModel.MessageQueue.Enqueue($"{item.FullData} already downloaded!");
                        tcs.SetResult(item.Id);
                    }
                    else
                    {
                        MainViewModelLocator.MainViewModel.MessageQueue.Enqueue($"Starting download {item.FullData}");
                        var writer = File.OpenWrite(path);
                        var client = new RestClient(new Uri(item.Url));
                        var request = new RestRequest(Method.GET);
                        request.ResponseWriter = stream => { using (stream) { stream.CopyTo(writer); } };
                        client.DownloadData(request);
                        MainViewModelLocator.MainViewModel.MessageQueue.Enqueue($"{item.FullData} Download completed");
                        writer.Dispose();
                    }
                }
                else
                {
                    MainViewModelLocator.MainViewModel.MessageQueue.Enqueue("Error! No directory for download!");
                    tcs.SetResult(item.Id);
                }
                    
            });
            return tcs.Task;
        }

        private static async void StartQueueProcessing()
        {
            while (_requestQueue.Count > 0)
            {
                List<KeyValuePair<long, List<AudioModel>>> queueItems;

                lock (_syncRoot)
                {
                    queueItems = _requestQueue.Take(MAX_PARALLEL_ITEMS).ToList(); //берем несколько элементов очереди
                }

                var runningTasks = new List<Task<long>>();

                foreach (var queuItem in queueItems)
                {
                    var item = queuItem.Value.FirstOrDefault();
                    if (item == null)
                        continue;
                    runningTasks.Add(DownloadAsync(item));
                }

                var tracks = await Task.WhenAll(runningTasks);

                runningTasks.Clear();

                foreach (var keyValuePair in tracks)
                {
                    if (queueItems.All(i => i.Key != keyValuePair))
                        continue;

                    var q = queueItems.First(i => i.Key == keyValuePair);

                    lock (_syncRoot)
                    {
                        _requestQueue.Remove(q.Key);
                    }
                }
            }

            await Task.Delay(100);
        }

        public static void AddToQueue(AudioModel item)
        {
            lock (_syncRoot)
            {
                if (_requestQueue.ContainsKey(item.Id))
                {
                    if (_requestQueue[item.Id].All(t => t != item))
                        _requestQueue[item.Id].Add(item);
                }
                else
                {
                    _requestQueue.Add(item.Id, new List<AudioModel>() { item });
                }
            }
            Task.Run(() => StartQueueProcessing());
        }
    }
}
