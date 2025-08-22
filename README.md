README for learn-postgres-fts
=============================

Learning project for using full text search with Postgres, C# Entity Framework (code-first).

# Features
## Single 'unknown' translation
At time of writing, each asset (hotel review) only has a single translation that is indexed in multiple languages

This could be rearranged to have a single vector that matches a specific translation of an asset (I think)

## Reusable indexing setup
A full-text-search can be added to any entity `Xxx` in the form of a `XxxFts` table. The same related entity structure `Fts` is used.

## Low-overhead, customizable indexes
Full-text / vector indexes are created in another `XxxFts` table which keeps the original entities lightweight

The `XxxFts` table has its own copy of the searchable text which must be maintained but also allows custom calculation of searchable text

# Setup
1. Setup a database that matches connection string in [Program.cs](./app/Program.cs)
   - Can use an entry in `/etc/hosts` for custom host name
1. Install `ef` tool with `dotnet tool install --global dotnet-ef --version ???` (see library versions)
2. Run Migrations in `app` folder. E.g. `dotnet ef database update`
3. Use [DataLoader](./tests/Pkg/Util/DataLoader.cs) to populate tables
   - The dataset used needs some massaging. See notes at top of that file
5. Run tests

# Code Tour
Visit these classes to gain a rough understanding:
- `HotelReview` is a first attempt that has vector fields in the same table
- `HotelReview2` is a second attempt with separate table (`HotelReview2Fts`) containing vector fields
- `AppDb` sets up relationships (Note that XxxFts table can be added for any table Xxx with very little code)
- `FullTextSearchTests` illustrates show to search