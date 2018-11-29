# build image
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out
#RUN dotnet publish --output /out/ --configuration Release

# runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=build /app/out .
#EXPOSE 5000
#ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT [ "dotnet", "StatlerWaldorfCorp.TeamService.dll" ]