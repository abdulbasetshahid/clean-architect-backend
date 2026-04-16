using MediatR;

namespace EShop.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest<Unit>
{
    public Guid Id { get; }

    public DeleteProductCommand(Guid id) => Id = id;
}
