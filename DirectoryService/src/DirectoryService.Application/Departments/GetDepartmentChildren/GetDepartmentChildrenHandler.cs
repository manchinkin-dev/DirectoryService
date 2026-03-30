using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts.Departments;
using FluentValidation;
using Shared.Fails;

namespace DirectoryService.Application.Departments.GetDepartmentChildren;

public class GetDepartmentChildrenHandler
{
    private readonly IReadDbContext _readDbContext;
    private readonly IValidator<GetDepartmentChildrenQuery> _validator;

    public GetDepartmentChildrenHandler(
        IReadDbContext readDbContext,
        IValidator<GetDepartmentChildrenQuery> validator)
    {
        _readDbContext = readDbContext;
        _validator = validator;
    }

    public async Task<Result<DepartmentChildrenResponse, Errors>> Handle(
        GetDepartmentChildrenQuery query,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToList();
        }

        var connection = _readDbContext.Connection;

        const string sql =
            """
            WITH children as (
                SELECT
                    d.id,
                    d.name,
                    d.parent_id,
                    d.identifier,
                    d.is_active,
                    d.created_at
                FROM departments d
                WHERE d.parent_id = @ParentId
                ORDER BY d.created_at
                OFFSET @Page LIMIT @Size)
            SELECT
                *,
                (SELECT
                     COUNT(*)
                 FROM departments
                 WHERE parent_id = children.id) as children_count,
                (EXISTS(
                    SELECT
                        1
                    FROM departments
                    WHERE parent_id = children.id)) AS has_more_children
            FROM children;
            """;

        var children = (await connection.QueryAsync<RootDepartmentDto>(
                sql,
                param: new
                {
                    query.ParentId,
                    Page = (query.Request.Page - 1) * query.Request.Size,
                    query.Request.Size,
                }))
            .ToList();

        return new DepartmentChildrenResponse(children);
    }
}