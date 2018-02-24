FROM resin/raspberry-pi-node:8-slim

# -- Install Application into container:
RUN set -ex && mkdir /app

COPY ./package.json /app/
COPY ./yarn.lock /app/
COPY ./src /app/

WORKDIR /app
RUN set -ex && yarn install --production

# -- Runtime configuration
EXPOSE 9080
ENTRYPOINT ["yarn", "run", "start"]
