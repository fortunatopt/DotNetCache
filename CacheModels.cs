using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheUtility
{

    /// <summary>
    /// Class to pull data from the Cache
    /// </summary>
    public class CacheObject
    {
        /// <summary>
        /// Name of the Cache item
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Cache data
        /// </summary>
        public object Value { get; set; }
        private string _area;
        /// <summary>
        /// The area for the Cache item
        /// </summary>
        public string Area
        {
            get
            {
                if (Key != null && Key != string.Empty)
                {
                    _area = Key.Substring(0, Key.LastIndexOf('_'));
                }
                return _area;
            }
            set { _area = value; }
        }
        private string _owner;
        /// <summary>
        /// Owner of the Cache item
        /// </summary>
        public string Owner
        {
            get
            {
                if (Key != null && Key != string.Empty)
                {
                    int lio = Key.LastIndexOf('_') + 1;
                    _owner = Key.Substring(lio, Key.Length - lio);
                }
                return _owner;
            }
            set { _owner = value; }
        }
    }
}
