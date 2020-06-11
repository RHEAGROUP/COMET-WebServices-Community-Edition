#!/bin/bash
set -e
echo "CDP 4 ENTRYPOINT RUNNING"

echo "Waiting for db to Bootup"

./wait-for/wait-for.sh cdp4-database-community-edition:5432 -t 30 -- echo "Database Ready"

run_cmd="mono CDP4WebServer.exe"

>&2 echo "Executing CDP4 Webservice Startup Command"
exec $run_cmd