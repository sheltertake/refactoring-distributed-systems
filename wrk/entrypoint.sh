#!/bin/sh

# redirect stdout and stderr to files
exec >/wrk/results.txt
exec 2>/wrk/results.txt

# now run the requested CMD without forking a subprocess
exec "$@"