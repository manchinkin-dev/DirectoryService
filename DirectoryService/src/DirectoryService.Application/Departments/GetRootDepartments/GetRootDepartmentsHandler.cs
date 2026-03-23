using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Departments;
using Shared.Fails;

namespace DirectoryService.Application.Departments.GetRootDepartments;

public class GetRootDepartmentsHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetRootDepartmentsHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<RootDepartmentsResponse, Errors>> Handle(
        GetRootDepartmentQuery query,
        CancellationToken cancellationToken)
    {
        var connection = _readDbContext.Connection;

        const string sql = """
                           WITH roots AS (
                               SELECT
                                   d.id,
                                   d.name,
                                   d.parent_id,
                                   d.identifier,
                                   d.is_active,
                                   d.created_at
                                   FROM departments d
                                   WHERE d.parent_id IS NULL
                                   ORDER BY d.created_at
                                   OFFSET @Page LIMIT @Size)
                           SELECT
                               *, 
                               (SELECT
                                    COUNT(*)
                                FROM departments
                                WHERE parent_id = roots.id) as children_count,
                               (EXISTS(
                                   SELECT
                                       1
                                   FROM departments
                                   WHERE parent_id = roots.id
                                   OFFSET @Prefetch LIMIT 1)) AS has_more_children
                           FROM roots

                           UNION ALL

                           SELECT
                               c.*,
                               (SELECT
                                    COUNT(*)
                                FROM departments
                                WHERE parent_id = c.id) as children_count,
                               (EXISTS(
                                   SELECT
                                       1
                                   FROM departments
                                   WHERE parent_id = c.id)) AS has_more_children
                           FROM roots r
                               CROSS JOIN LATERAL (
                                   SELECT
                                       d.id,
                                       d.name,
                                       d.parent_id,
                                       d.identifier,
                                       d.is_active,
                                       d.created_at
                                   FROM departments d
                                   WHERE d.parent_id = r.id AND d.is_active
                                   ORDER BY d.created_at
                                   LIMIT @Prefetch) c
                           ORDER BY
                               parent_id NULLS FIRST,
                               created_at;
                           """;

        var rootDepartments = (await connection.QueryAsync<RootDepartmentDto>(
                sql,
                param: new
                {
                    Page = (query.Request.Page - 1) * query.Request.Size,
                    query.Request.Size,
                    query.Request.Prefetch,
                }))
            .ToList();

        return new RootDepartmentsResponse(rootDepartments);
    }
}