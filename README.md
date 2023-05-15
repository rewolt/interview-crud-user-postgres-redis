## Run Locally

Clone the project

```bash
  git clone https://github.com/rewolt/interview-crud-user-postgres-redis.git
```

Go to the project directory

```bash
  cd interview-crud-user-postgres-redis
```

Install & start docker containers

```bash
  docker run --name my-postgres -e POSTGRES_PASSWORD=password -p 5432:5432 -d postgres
  docker run --name my-redis -p 6379:6379 -d redis
```