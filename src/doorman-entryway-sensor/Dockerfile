FROM resin/raspberry-pi-python:3-slim

ENV INITSYSTEM on

# -- Install Pipenv:
RUN set -ex && pip install pipenv --upgrade

# -- Install Application into container:
RUN set -ex && mkdir /app

WORKDIR /app

# -- Adding Pipfiles
COPY ./ /app/

# -- Install dependencies:
RUN set -ex && pipenv install --deploy --system

ENTRYPOINT ["python3", "/app/src/doorman/entryway/app.py"]
