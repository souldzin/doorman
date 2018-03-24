ROOM_ID=${1}
OCCUPANCY_COUNT=${2:-5}
HOST=${3:-http://localhost:51681/api/doorman}

DATA='{"occupancyCount": '"${OCCUPANCY_COUNT}"'}'

echo $(curl -i -v --silent --raw \
    -H "Content-Type: application/json" \
    -H "Authorization: ${AUTH}" \
    -X POST \
    -d "${DATA}" \
    "${HOST}/room/${ROOM_ID}/snapshot" )

