using Microsoft.Data.SqlClient;

public class ProductoRepository : IProductoRepository
{
    private readonly string _connectionString;

    public ProductoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public List<Producto> ObtenerTodos()
    {
        var lista = new List<Producto>();
        using var conexion = new SqlConnection(_connectionString);
        string query = "SELECT Id, Nombre, Precio, Stock FROM Productos";
        using var comando = new SqlCommand(query, conexion);
        
        try
        {
            conexion.Open();
            using var lector = comando.ExecuteReader();
            while (lector.Read())
            {
                lista.Add(new Producto
                {
                    Id = lector.GetInt32(0),
                    Nombre = lector.GetString(1),
                    Precio = lector.GetDecimal(2),
                    Stock = lector.GetInt32(3)
                });
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Error de base de datos: {ex.Message}");
        }
        return lista;
    }

    public Producto? ObtenerPorId(int id)
    {
        using var conexion = new SqlConnection(_connectionString);
        string query = "SELECT Id, Nombre, Precio, Stock FROM Productos WHERE Id = @Id";
        using var comando = new SqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@Id", id);

        try
        {
            conexion.Open();
            using var lector = comando.ExecuteReader();
            if (lector.Read())
            {
                return new Producto
                {
                    Id = lector.GetInt32(0),
                    Nombre = lector.GetString(1),
                    Precio = lector.GetDecimal(2),
                    Stock = lector.GetInt32(3)
                };
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Error de base de datos: {ex.Message}");
        }
        return null;
    }

    public int Insertar(Producto producto)
    {
        using var conexion = new SqlConnection(_connectionString);
        string query = "INSERT INTO Productos (Nombre, Precio, Stock) OUTPUT INSERTED.Id VALUES (@Nombre, @Precio, @Stock)";
        using var comando = new SqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@Nombre", producto.Nombre);
        comando.Parameters.AddWithValue("@Precio", producto.Precio);
        comando.Parameters.AddWithValue("@Stock", producto.Stock);

        try
        {
            conexion.Open();
            return (int)comando.ExecuteScalar();
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Error al insertar: {ex.Message}");
            return -1;
        }
    }

    public bool Actualizar(Producto producto)
    {
        using var conexion = new SqlConnection(_connectionString);
        string query = "UPDATE Productos SET Nombre = @Nombre, Precio = @Precio, Stock = @Stock WHERE Id = @Id";
        using var comando = new SqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@Id", producto.Id);
        comando.Parameters.AddWithValue("@Nombre", producto.Nombre);
        comando.Parameters.AddWithValue("@Precio", producto.Precio);
        comando.Parameters.AddWithValue("@Stock", producto.Stock);

        try
        {
            conexion.Open();
            int filas = comando.ExecuteNonQuery();
            return filas > 0;
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Error al actualizar: {ex.Message}");
            return false;
        }
    }

    public bool Eliminar(int id)
    {
        using var conexion = new SqlConnection(_connectionString);
        string query = "DELETE FROM Productos WHERE Id = @Id";
        using var comando = new SqlCommand(query, conexion);
        comando.Parameters.AddWithValue("@Id", id);

        try
        {
            conexion.Open();
            int filas = comando.ExecuteNonQuery();
            return filas > 0;
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Error al eliminar: {ex.Message}");
            return false;
        }
    }
}