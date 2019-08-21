using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Core
{
    /// <summary>
    /// save objects to a the local file system
    /// </summary>
    public class FileSystemTimedContext : IContext
    {
        #region Data Memever
        private string _directory;
        private static Timer _timer;
        private List<object> _localTempItems = new List<object>();
        private object syncObject = new object();
        private DirectoryInfo _directoryInfo;

        #endregion

        public FileSystemTimedContext(string directory)
        {
            _directory = directory;
            _directoryInfo = new DirectoryInfo(_directory);
            if (!_directoryInfo.Exists)
                _directoryInfo.Create();
            _timer = new Timer(SaveToFileSystem, null, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// Add new item to a local cache
        /// </summary>
        /// <param name="entity">item to add</param>
        public void Add<T>(T entity)
        {
            lock (syncObject)
            {
                _localTempItems.Add(entity);
            }
        }

        /// <summary>
        /// Get all the item in list order by creation time decending
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> All<T>()
        {
            var files = _directoryInfo.EnumerateFiles("*.json").OrderByDescending(x => x.CreationTime);
            foreach (var file in files)
            {
                var listObject = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(file.FullName));
                foreach (var obj in listObject)
                {
                    yield return obj;
                }
            }
        }

        /// <summary>
        /// A background thread. tick ever 1 second after the last tick ends.
        /// </summary>
        /// <param name="state"></param>
        private void SaveToFileSystem(object state)
        {
            try
            {
                //if no items there is no need to save
                if (_localTempItems.Count == 0)
                    return;
                List<object> items;
                //a. we lock the list to make sure no thread is adding items while we get a pointer to the list
                //b. we init new instance of the list
                lock (syncObject)
                {
                    items = _localTempItems;
                    _localTempItems = new List<object>();
                }

                //split the item list to a chunk of 10 items
                List<List<object>> chunks = new List<List<object>>();
                List<object> chunk = new List<object>();
                for (int i = 0; i < items.Count; i++)
                {
                    if (i % 10 == 0)
                    {
                        chunk = new List<object>();
                        chunks.Add(chunk);
                    }
                    chunk.Add(items[i]);
                }
                //save chunks to filesystem, file name is unique by timestamp and file counter
                for (int i = 0; i < chunks.Count; i++)
                {
                    var jsonItems = JsonConvert.SerializeObject(chunks[i]);
                    var fileName = $"{_directory}\\{DateTime.Now:ddMMyyyyHHmmss}_{i}.json";
                    File.WriteAllText(fileName, jsonItems);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Failed on saving to file system", ex);
            }
            finally
            {
                //make sure the time is ticking again in 1 second
                _timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(-1));
            }

        }

    }
}