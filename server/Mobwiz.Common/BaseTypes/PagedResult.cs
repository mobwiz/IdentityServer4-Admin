using System.Collections.Generic;

namespace Mobwiz.Common.BaseTypes
{
    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// 记录
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// 记录总数
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public PagedResult()
        {

        }

        /// <summary>
        /// 实例化分页结果
        /// </summary>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="totalCount">记录总数</param>
        /// <param name="items">结果集</param>
        public PagedResult(long totalCount, IEnumerable<T> items)
        {
            Items = items;
            TotalCount = totalCount;
        }
    }
}
