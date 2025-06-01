using MySqlConnector;
namespace YourProject.Endpoints;

public static class ColorEndpoints
{
    public static void MapColorEndpoints(this IEndpointRouteBuilder app, string connectionString)
    {
        app.MapGet("/colors", async () =>
        {
            var colors = new List<Color>();

            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand("SELECT id, name, code, image_url FROM colors;", conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                colors.Add(new Color
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Code = reader.GetString(2),
                    ImageUrl = reader.GetString(3)
                });
            }

            return Results.Ok(colors);
        });

        app.MapGet("/colors/{id:int}", async (int id) =>
        {
            using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand("SELECT id, name, code, image_url FROM colors WHERE id = @id;", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var color = new Color
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Code = reader.GetString(2),
                    ImageUrl = reader.GetString(3)
                };

                return Results.Ok(color);
            }

            return Results.NotFound();
        });
    }
}
