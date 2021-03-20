FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

ENV NODE_VERSION=10.15.1
ENV NODE_DOWNLOAD_SHA=ca1dfa9790876409c8d9ecab7b4cdb93e3276cedfc64d56ef1a4ff1778a40214

RUN curl -SL "https://nodejs.org/dist/v${NODE_VERSION}/node-v${NODE_VERSION}-linux-x64.tar.gz" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs

# install yarn
RUN npm install -g yarn
# set environment variable
ENV ASPNETCORE_ENVIRONMENT="Production"
# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
# install parcel
#RUN yarn global add parcel-bundler
# restore node mod
RUN yarn install
# build vue app
RUN yarn build
#RUN parcel build ./ClientComponents/bundle.js --out-dir ./wwwroot/dist
# build net core app
#RUN dotnet publish -c Release -o out

# Build runtime image
#FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine3.9
FROM mcr.microsoft.com/dotnet/aspnet:5.0.4-alpine3.13-amd64

WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "VueCore.dll"]
# Heroku
# CMD ASPNETCORE_URLS=http://*:$PORT dotnet ASPNETReact.dll
