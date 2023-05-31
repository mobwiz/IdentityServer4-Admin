// Copyright (c) Luther R.D. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Types;

namespace IdentityServer4.Storage.FreeSql.Repositories.Impl
{
    public class StringCollectionRepositoryImpl : IStringCollectionRepository
    {
        private IFreeSql _conn;

        public StringCollectionRepositoryImpl(IFreeSql conn)
        {
            this._conn = conn;
        }

        /// <summary>
        /// Save string values
        /// </summary>
        /// <param name="stringType">String type</param>
        /// <param name="entityId">entitity id</param>
        /// <param name="values"> values</param>
        public void StoreValues(EStringType stringType, long entityId, IEnumerable<string> values)
        {
            // check input
            if (entityId <= 0) throw new ArgumentOutOfRangeException(nameof(entityId));
            if (values == null) throw new ArgumentNullException(nameof(values));

            var repos = _conn.GetRepository<MStringCollectionItem>();

            var itemsToInsert = new List<MStringCollectionItem>();

            foreach (var str in values)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    itemsToInsert.Add(new MStringCollectionItem
                    {
                        ParentId = entityId,
                        StringType = stringType,
                        StringValue = str.Trim()
                    });
                }
            }

            repos.Select.Where(p => p.ParentId == entityId && p.StringType == stringType)
                .ToDelete().ExecuteAffrows();
            repos.Insert(itemsToInsert);

        }

        // 读取字符串
        public IList<string> GetValues(EStringType stringType, long entityId)
        {
            // the entityId must be greater thant 0
            if (entityId <= 0) throw new ArgumentOutOfRangeException(nameof(entityId));

            {
                var repos = _conn.GetRepository<MStringCollectionItem>();
                var list = repos.Select.Where(p => p.ParentId == entityId && p.StringType == stringType)
                    .ToList();

                return list.Select(p => p.StringValue).ToList();
            }
        }

        /// <summary>
        /// Get values for entities
        /// </summary>
        /// <param name="stringType"></param>
        /// <param name="entityIds"></param>
        /// <returns></returns>
        public IList<string> GetValues(EStringType stringType, long[] entityIds)
        {
            // check input
            if (entityIds == null || entityIds.Length == 0) throw new ArgumentNullException(nameof(entityIds));

            {
                var repos = _conn.GetRepository<MStringCollectionItem>();
                var list = repos.Select.Where(p => entityIds.Contains(p.ParentId) && p.StringType == stringType)
                    .ToList();

                return list.Select(p => p.StringValue).ToList();
            }
        }

        /// <summary>
        /// Remove values by entity id
        /// </summary>
        /// <param name="stringType"></param>
        /// <param name="entityId"></param>
        public void RemoveValues(EStringType stringType, long entityId)
        {
            // check input
            if (entityId <= 0) throw new ArgumentOutOfRangeException(nameof(entityId));

            _conn.Select<MStringCollectionItem>()
                .Where(p => p.ParentId == entityId && p.StringType == stringType)
                .ToDelete().ExecuteAffrows();
        }

        public IList<MStringCollectionItem> GetAllsItemsById(long entityId)
        {
            // the entityId must be greater thant 0
            if (entityId <= 0) throw new ArgumentOutOfRangeException(nameof(entityId));

            {
                var repos = _conn.GetRepository<MStringCollectionItem>();
                var list = repos.Select.Where(p => p.ParentId == entityId)
                    .ToList();

                return list;
            }
        }


        public IList<MStringCollectionItem> GetAllsItemsById(long[] entityIds)
        {
            // the entityId must be greater thant 0
            if (entityIds.Length <= 0) throw new ArgumentOutOfRangeException(nameof(entityIds));

            {
                var repos = _conn.GetRepository<MStringCollectionItem>();
                var list = repos.Select.Where(p => entityIds.Contains(p.ParentId))
                    .ToList();

                return list;
            }
        }
    }
}
