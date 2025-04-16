#!/bin/sh

#Wait for postgres before running the dll. We are doing this for hangfire. (It must start after postgres)
until nc -z postgres 5432; do
  echo "Waiting for postgres..."
  sleep 1
done

exec dotnet DataCenter.Api.dll