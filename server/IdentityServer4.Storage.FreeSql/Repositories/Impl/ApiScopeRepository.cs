using IdentityServer4.Storage.FreeSql.Entities;
using IdentityServer4.Storage.FreeSql.Repositories.Impl;
using IdentityServer4.Storage.FreeSql.Types;
using Laiye.SaasUC.Service.UserModule.Repositories;
using Mobwiz.Common.BaseTypes;
using static FreeSql.Internal.GlobalFilter;

namespace IdentityServer4.Storage.FreeSql.Repositories
{
    public class ApiScopeRepositoryImpl : InnerBaseRepository, IApiScopeRepository
    {

        public ApiScopeRepositoryImpl(DatabaseConnectionManager manager)
            : base(manager)
        {

        }

        public async Task<long> CreateApiScopeAsync(MApiScope apiScope)
        {
            // the apiScope must not be null
            if (apiScope == null) throw new ArgumentNullException(nameof(apiScope));

            using var conn = GetConnection()!;
            var idInserted = 0L;
            conn.Transaction(() =>
            {
                var apiScopeRepos = conn.GetRepository<MApiScope>();

                apiScopeRepos.Insert(apiScope);
                if (apiScope.Claims.Any())
                {
                    var strRepos = new StringCollectionRepositoryImpl(conn!);
                    strRepos.StoreValues(EStringType.ApiScopeUserClaim, apiScope.Id, apiScope.Claims);
                }

                idInserted = apiScope.Id;
            });

            return idInserted;
        }

        public async Task<IList<MApiScope>> FindApiScopesByNameAsync(IList<string> names)
        {
            // the parmater names must not be null and must contains at least one item
            if (names == null || names.Count == 0) throw new ArgumentNullException(nameof(names));

            using var conn = GetConnection()!;

            var list = await conn.Select<MApiScope>()
                    .Where(p => names.Contains(p.Name))
                    .ToListAsync();

            var strRepos = new StringCollectionRepositoryImpl(conn);
            foreach (var item in list)
            {
                item.Claims = strRepos.GetValues(EStringType.ApiScopeUserClaim, item.Id);
            }

            return list;
        }

        public async Task<IList<MApiScope>> GetAllApiScopesAsync()
        {
            using var conn = GetConnection()!;

            var apiScopeRepos = conn.GetRepository<MApiScope>();
            var list = await apiScopeRepos.Select.Where(p => p.Enabled == 1).ToListAsync(); // 只需要读取 enabled 
            var strRepos = new StringCollectionRepositoryImpl(conn);

            foreach (var item in list)
            {
                item.Claims = strRepos.GetValues(EStringType.ApiScopeUserClaim, item.Id);
            }

            return list;
        }

        public Task RemoveApiScopeAsync(long id)
        {
            // the id must be greater than 0
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            using var conn = GetConnection()!;

            conn.Transaction(() =>
            {
                var apiScopeRepos = conn.GetRepository<MApiScope>();
                var scope = apiScopeRepos.Select.Where(p => p.Id == id).First();
                var strRepos = new StringCollectionRepositoryImpl(conn);

                strRepos.RemoveValues(EStringType.ApiScopeUserClaim, scope.Id);
                apiScopeRepos.Delete(scope);
            });

            return Task.CompletedTask;
        }

        public Task UpdateApiScopeAsync(MApiScope scope)
        {
            // the scope must not be null
            if (scope == null) throw new ArgumentNullException(nameof(scope));

            using var conn = GetConnection()!;

            conn.Transaction(() =>
            {
                var apiScopeRepos = conn.GetRepository<MApiScope>();

                var entity = apiScopeRepos.Select.Where(p => p.Id == scope.Id).First();
                if (entity != null)
                {
                    entity.Name = scope.Name;
                    entity.DisplayName = scope.DisplayName;
                    entity.Emphasize = scope.Emphasize;
                    entity.Required = scope.Required;
                    entity.Claims = scope.Claims;
                    entity.Enabled = scope.Enabled;
                    entity.UpdateBy = scope.UpdateBy;
                    entity.UpdateTime = scope.UpdateTime;

                    apiScopeRepos.Update(entity);

                    if (scope.Claims.Any())
                    {
                        var strRepos = new StringCollectionRepositoryImpl(conn);
                        strRepos.StoreValues(EStringType.ApiScopeUserClaim, entity.Id, entity.Claims);
                    }
                }
            });

            return Task.CompletedTask;
        }

        public async Task<PagedResult<MApiScope>> QueryApiScopes(string keyword, BoolCondition enabled, int pageIndex, int pageSize)
        {
            // the pageindex must be greater than 0
            if (pageIndex <= 0) throw new ArgumentOutOfRangeException(nameof(pageIndex));
            // the page size must be greater thant 0
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            var query = GetConnection().GetRepository<MApiScope>().Select;

            query = query.WhereIf(enabled != BoolCondition.All, p => p.Enabled == (byte)enabled);
            query = query.WhereIf(!string.IsNullOrWhiteSpace(keyword), p => p.Name.Contains(keyword) || p.DisplayName.Contains(keyword));

            var total = await query.CountAsync();
            var list = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<MApiScope>(total, list);
        }

        public async Task<MApiScope> GetApiScopeByIdAsync(long id)
        {
            // the id must be greater thant 0
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            using var conn = GetConnection()!;

            var repos = conn.GetRepository<MApiScope>();
            var item = await repos.Select.Where(p => p.Id == id).FirstAsync();

            if (item != null)
            {
                var strRepos = new StringCollectionRepositoryImpl(conn);
                item.Claims = strRepos.GetValues(EStringType.ApiScopeUserClaim, item.Id);
            }

            return item;
        }

        public async Task<bool> SameNameExistedAsync(string name, long id)
        {
            // the name must not be null or empty
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            var conn = GetConnection();
            var repos = conn.GetRepository<MApiScope>();

            if (id > 0)
            {
                var existed = await repos.Select.Where(p => p.Name == name && p.Id != id).FirstAsync();
                return existed != null;
            }
            else
            {
                var existed = await repos.Select.Where(p => p.Name == name).FirstAsync();
                return existed != null;
            }
        }
    }
}