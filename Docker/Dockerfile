# Buildtime Container
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY src/Tools.DiskPurger/Tools.DiskPurger.csproj .
RUN dotnet restore Tools.DiskPurger.csproj

COPY src/Tools.DiskPurger/ .
RUN dotnet publish -c Release -o out

# Runtime Container Image
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runtime
LABEL Author="Rob Hofmann <rob.hofmann@me.com>"

ENV BASE_DIR=/data

# Add Cron
RUN apt-get -q update && \
    apt-get -qy dist-upgrade && \
    apt-get install -y --no-install-recommends cron && \
    apt-get -y autoremove && \
    apt-get -y clean && \
    rm -rf /var/lib/apt/lists/*

# Make sure we setup the entrypoint script
COPY /Docker/entrypoint.sh /entrypoint.sh
RUN chmod u+x /entrypoint.sh

# Place the app binaries
WORKDIR /app
COPY --from=build /app/out ./

# Define the Entrypoint
ENTRYPOINT ["/entrypoint.sh"]