// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Types;

namespace IdentityServer4.Storage.FreeSql.Repositories
{
    internal interface IStringCollectionRepository
    {
        /// <summary>
        /// 获取指定类型的字符串值列表
        /// </summary>
        /// <param name="stringType"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        IList<string> GetValues(EStringType stringType, long entityId);

        /// <summary>
        /// 获取多个Entity的字符串列表
        /// </summary>
        /// <param name="stringType"></param>
        /// <param name="entityIds"></param>
        /// <returns></returns>
        IList<string> GetValues(EStringType stringType, long[] entityIds);

        /// <summary>
        /// 删除某个Entity的字符串值
        /// </summary>
        /// <param name="stringType"></param>
        /// <param name="entityId"></param>
        void RemoveValues(EStringType stringType, long entityId);

        /// <summary>
        /// 保存指定类型的的字符串值
        /// </summary>
        /// <param name="stringType"></param>
        /// <param name="entityId"></param>
        /// <param name="values"></param>
        void StoreValues(EStringType stringType, long entityId, IEnumerable<string> values);

        /// <summary>
        /// 读取某个entity所有的字符串值
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        IList<MStringCollectionItem> GetAllsItemsById(long entityId);

        /// <summary>
        /// 读取某个entity所有的字符串值
        /// </summary>
        /// <param name="entityIds"></param>
        /// <returns></returns>
        IList<MStringCollectionItem> GetAllsItemsById(long[] entityIds);
    }
}