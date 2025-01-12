# orm-bench
Performance benchmarks on various .NET ORMs and Micro ORMs
# orm-bench

Performance benchmarks on various .NET ORMs and Micro ORMs.

## Table of Contents

- [Introduction](#introduction)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Running Benchmarks](#running-benchmarks)
- [Database Seeders](#database-seeders)
- [Repositories](#repositories)
- [Tests](#tests)
- [Contributing](#contributing)
- [License](#license)

## Introduction

This project aims to benchmark the performance of various .NET ORMs and Micro ORMs. It includes implementations using Entity Framework Core and Dapper, among others.


## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Docker (optional, for containerized execution)

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/orm-bench.git
    cd orm-bench
    ```

2. Restore the dependencies:
    ```sh
    dotnet restore
    ```

## Configuration

The configuration file [appsettings.json](http://_vscodecontentref_/12) is located in the [OrmBenchmarker](http://_vscodecontentref_/13) directory. It contains the connection strings for different databases and benchmark settings.

```json
{
  "Databases": {
    "SQLite": "Data Source=benchmark.db",
    "PostgreSQL": "Host=localhost;Port=5432;Username=postgres;Password=1234;Database=benchmark",
    "MSSQL": "Server=localhost,14330;Database=benchmarker;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=True;"
  },
  "Benchmarks": {
    "CustomerRecords": 1000,
    "OrderPerCustomer": 10
  }
}
```
### Running Benchmarks
To run the benchmarks, execute the following command in the OrmBenchmarker directory:

This will seed the database and run the benchmarks defined in the Benchmarks class.

### Database Seeders
The DatabaseSeeder class is responsible for seeding the database with test data. It uses the DataGenerator class to generate fake data for customers and orders.

- DataGenerator
- Repositories

License
This project is licensed under the MIT License. See the LICENSE file for details.