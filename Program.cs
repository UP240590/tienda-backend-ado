var builder = WebApplication.CreateBuilder(args);

// Registrar el repositorio en el contenedor de dependencias
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Activar Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoints CRUD
app.MapGet("/productos", (IProductoRepository repo) => repo.ObtenerTodos());

app.MapGet("/productos/{id}", (int id, IProductoRepository repo) => 
    repo.ObtenerPorId(id) is Producto p ? Results.Ok(p) : Results.NotFound());

app.MapPost("/productos", (Producto producto, IProductoRepository repo) => {
    int nuevoId = repo.Insertar(producto);
    return nuevoId > 0 ? Results.Created($"/productos/{nuevoId}", producto) : Results.BadRequest();
});

app.MapPut("/productos/{id}", (int id, Producto producto, IProductoRepository repo) => {
    producto.Id = id;
    bool actualizado = repo.Actualizar(producto);
    return actualizado ? Results.NoContent() : Results.NotFound();
});

app.MapDelete("/productos/{id}", (int id, IProductoRepository repo) => {
    bool eliminado = repo.Eliminar(id);
    return eliminado ? Results.NoContent() : Results.NotFound();
});

app.Run();