﻿namespace MoviesGalleryASPNetCoreHoted.Server.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Movie> Movies => Set<Movie>();
}