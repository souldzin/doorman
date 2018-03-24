ROOM_NAME=${1:-New Room}
HOST=${2:-http://localhost:51681/api/doorman}
CLIENT_ID=${3:-123}
CLIENT_SECRET=${4:-youououo}

DATA='{"client_id": "'"${CLIENT_ID}"'", "client_secret": "'"${CLIENT_SECRET}"'"}'

echo $(curl -i -v --silent --raw -H "Content-Type: application/json" -X POST -d "${DATA}" "${HOST}/room?name=${ROOM_NAME}")
