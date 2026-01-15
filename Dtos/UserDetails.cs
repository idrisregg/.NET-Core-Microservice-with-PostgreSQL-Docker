namespace Wajeb.API.Dtos;

public record UserDetails
{
    public required int Id { get; init; }
    public required string Username { get; init; }

    public required string Email { get; init; }

}