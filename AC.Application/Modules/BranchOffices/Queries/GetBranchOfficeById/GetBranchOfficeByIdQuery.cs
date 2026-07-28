using AC.Application.Abstractions.Messaging.Queries;

namespace AC.Application.Modules.BranchOffices.Queries.GetBranchOfficeById;

public class GetBranchOfficeByIdQuery : IQuery<GetBranchOfficeByIdQueryResult>
{
    public Guid Id { get; set; }
}
