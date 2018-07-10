# build image
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR ./StatlerWaldorfCorp.TeamService

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish --output /out/ --configuration Release

# runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR ./StatlerWaldorfCorp.TeamService
COPY --from=build /out .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT [ "dotnet", "StatlerWaldorfCorp.TeamService.dll" ]