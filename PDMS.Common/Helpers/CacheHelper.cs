/* ---------------------------------------------------------------------    
 * Copyright:
 * Jabil
 * 
 * Class Description:
 * 缓存管理类
 *
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2017-07-17  shuwei_miao@Jabil.com
 *
 * ------------------------------------------------------------------------------*/

using PDMS.Common.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace PDMS.Common.Helpers
{
    public class CacheHelper
    {
        public static readonly Cache _cache;

        /// <summary>
        /// 静态构造函数用于初始化任何静态数据，或用于执行仅需执行一次的特定操作。在创建第一个实例或引用任何静态成员之前，将自动调用静态构造函数
        /// </summary>
        static CacheHelper()
        {
            HttpContext current = HttpContext.Current;
            if (current != null)
            {
                _cache = current.Cache;
            }
            else
            {
                _cache = HttpRuntime.Cache;
            }
        }

        /// <summary>
        /// 清除所有缓存数据
        /// </summary>
        public static void Clear()
        {
            IDictionaryEnumerator enumerator = _cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                _cache.Remove(enumerator.Key.ToString());
            }
        }

        /// <summary>
        /// 根据key获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            return _cache[key];
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void Set(string key, object obj)
        {
            Set(key, obj, null);
        }

        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="dep"></param>
        public static void Set(string key, object obj, CacheDependency dep)
        {
            if ((obj != null))
            {
                _cache.Insert(key, obj, dep, DateTime.MaxValue, TimeSpan.FromHours(10), CacheItemPriority.AboveNormal, null);
            }
        }

        /// <summary>
        /// 根据key移除缓存
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            _cache.Remove(key);
        }

        public static bool HasException()
        {
            bool? isException = CacheHelper.Get(ConstConstants.Has_Exception) as bool?;
            if (isException == null)//如果为空，则说明执行成功
            {
                return false;
            }
            else //isException只有null或true，null说明没问题，true说明有异常发生
            {
                return isException.Value;
            }
        }
    }
}
