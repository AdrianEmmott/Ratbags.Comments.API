# build with .net 8.0 sdk image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# github pat
ARG GITHUB_TOKEN
ENV GITHUB_TOKEN=${GITHUB_TOKEN}

ARG DB_CONNECTION_STRING
ENV DB_CONNECTION_STRING=${DB_CONNECTION_STRING}

# make sure vars get picked up...
RUN echo "COMMENTS GITHUB_TOKEN: $GITHUB_TOKEN"
RUN echo "COMMENTS DB_CONNECTION_STRING: $DB_CONNECTION_STRING"

# copy nuget.config and replace 'myPAT' with actual pat (using sed insead of powershell for image size)
COPY nuget.config .
RUN sed -i "s/myPAT/$GITHUB_TOKEN/g" nuget.config && cat nuget.config

# copy project file and restore dependencies using modified nuget.config file
COPY ["Ratbags.Comments.API.csproj", "./"]
RUN dotnet restore "Ratbags.Comments.API.csproj" --configfile nuget.config

# copy and build project
COPY . .
WORKDIR "/src/."
RUN dotnet build "Ratbags.Comments.API.csproj" -c Release -o /app/build

# publish
FROM build AS publish
RUN dotnet publish "Ratbags.Comments.API.csproj" -c Release -o /app/publish

# run app with .net 8.0 runtime image 
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5079
EXPOSE 7160

# install certs to sim ssl locally
RUN apt-get update && apt-get install -y ca-certificates

# copy published app into image
COPY --from=publish /app/publish .
COPY aspnetapp.pfx /app/aspnetapp.pfx

# run!
ENTRYPOINT ["dotnet", "Ratbags.Comments.API.dll"]
