# IQueryable vs IEnumerable in Entity Framework Core

## Understanding IQueryable vs IEnumerable

When working with Entity Framework Core, understanding the difference between `IQueryable<T>` and `IEnumerable<T>` is crucial for writing efficient database queries.

### IQueryable<T>

`IQueryable<T>` represents a query that hasn't been executed yet. It's the foundation of EF Core's deferred execution model.

```csharp
// IQueryable example - query is built up in stages
IQueryable<Book> booksQuery = context.Books;

// Each of these operations modifies the query but doesn't execute it
booksQuery = booksQuery.Where(b => b.Title.Contains("Entity"));
booksQuery = booksQuery.OrderBy(b => b.Title);

// Query is only executed when we call ToListAsync()
var books = await booksQuery.ToListAsync();

// This generates a single SQL query:
// SELECT * FROM Books WHERE Title LIKE '%Entity%' ORDER BY Title
```

**Key characteristics of IQueryable:**
- **Deferred execution**: The query only runs when data is actually needed
- **Expression trees**: Converts LINQ operations into SQL
- **Server-side evaluation**: Filtering, sorting, and paging happen on the database server
- **Efficient**: Only retrieves the data you actually need

### IEnumerable<T>

`IEnumerable<T>` represents a collection that has already been loaded into memory.

```csharp
// IEnumerable example - data is loaded immediately
IEnumerable<Book> booksEnumerable = await context.Books.ToListAsync();

// These operations happen in memory, not in the database
booksEnumerable = booksEnumerable.Where(b => b.Title.Contains("Entity"));
booksEnumerable = booksEnumerable.OrderBy(b => b.Title);

// Convert to list for the result
var books = booksEnumerable.ToList();
```

**Key characteristics of IEnumerable:**
- **Immediate execution**: Data is loaded when ToList() is called
- **Client-side evaluation**: Filtering, sorting, and paging happen in application memory
- **Less efficient for large datasets**: All data must be loaded before filtering
- **Multiple iterations**: Each operation creates a new enumeration

### When to Use Each

- **Use IQueryable when:**
  - Working directly with database queries
  - You need to build complex queries with multiple conditions
  - Performance is critical, especially with large datasets
  - You want to take advantage of database indexes and optimizations

- **Use IEnumerable when:**
  - The data is already in memory
  - You need to use methods that can't be translated to SQL
  - Working with small datasets where performance isn't critical
  - You're performing complex in-memory operations

## Split Queries in EF Core

Split queries are a feature in EF Core that allows you to split a single query with multiple includes into separate database queries.

### The Problem: Cartesian Explosion

When you include multiple related entities in a single query, EF Core typically generates a SQL query with multiple JOINs. This can lead to a "cartesian explosion" where the result set contains duplicate data and becomes very large.

```csharp
// Single query with multiple includes can cause cartesian explosion
var books = await context.Books
    .Include(b => b.Author)
    .Include(b => b.Publisher)
    .Include(b => b.Details)
    .Include(b => b.Categories) // Many-to-many relationship
    .ToListAsync();
```

### The Solution: Split Queries

Split queries tell EF Core to execute separate SQL queries for each included relationship, avoiding the cartesian explosion problem.

```csharp
// Using AsSplitQuery() to split into multiple database queries
var books = await context.Books
    .Include(b => b.Author)
    .Include(b => b.Publisher)
    .Include(b => b.Details)
    .Include(b => b.Categories)
    .AsSplitQuery() // This is the key difference
    .ToListAsync();
```

**This generates multiple SQL queries:**
1. `SELECT * FROM Books`
2. `SELECT * FROM Authors WHERE Id IN (...)`
3. `SELECT * FROM Publishers WHERE Id IN (...)`
4. `SELECT * FROM BookDetails WHERE BookId IN (...)`
5. `SELECT * FROM BookCategories bc JOIN Categories c ON bc.CategoryId = c.Id WHERE bc.BookId IN (...)`

### When to Use Split Queries

- **Use split queries when:**
  - Loading entities with multiple levels of relationships
  - Working with many-to-many relationships
  - Experiencing performance issues with large result sets
  - Seeing timeout errors with complex queries

- **Use single queries when:**
  - Working with simple relationships
  - Performance testing shows better results with a single query
  - Network latency is a bigger concern than query complexity

### Configuring Split Queries

You can configure split queries at different levels:

**Per-query configuration:**
```csharp
var books = await context.Books
    .Include(b => b.Author)
    .AsSplitQuery() // Apply to this query only
    .ToListAsync();
```

**Global configuration:**
```csharp
// In your DbContext configuration
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer(
        connectionString,
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
}
```

## Best Practices

1. **Profile and measure**: Always test both approaches to see which performs better for your specific scenario
2. **Consider data volume**: Split queries generally perform better with large datasets and complex relationships
3. **Watch for N+1 problems**: Be careful not to create N+1 query problems when manually loading related data
4. **Use AsNoTracking**: For read-only queries, combine with `AsNoTracking()` for better performance
5. **Be mindful of transaction boundaries**: Split queries execute multiple SQL statements, which may affect transaction behavior 