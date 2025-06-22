namespace EFCore101.API.Endpoints;

public interface IEndpoint
{
	void MapEndpoint(IEndpointRouteBuilder app);
}
