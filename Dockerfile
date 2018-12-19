FROM microsoft/dotnet:sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Colin.Lottery.WebApp/*.csproj ./Colin.Lottery.WebApp/
COPY Colin.Lottery.DataService/*.csproj ./Colin.Lottery.DataService/
COPY Colin.Lottery.Analyzers/*.csproj ./Colin.Lottery.Analyzers/
COPY Colin.Lottery.Collectors/*.csproj ./Colin.Lottery.Collectors/
COPY Colin.Lottery.Models/*.csproj ./Colin.Lottery.Models/
COPY Colin.Lottery.Common/*.csproj ./Colin.Lottery.Common/
COPY Colin.Lottery.Utils/*.csproj ./Colin.Lottery.Utils/
WORKDIR /app/Colin.Lottery.WebApp/
RUN dotnet restore

# copy and publish app and libraries
WORKDIR /app/
COPY Colin.Lottery.WebApp/. ./Colin.Lottery.WebApp/
COPY Colin.Lottery.DataService/. ./Colin.Lottery.DataService/
COPY Colin.Lottery.Analyzers/. ./Colin.Lottery.Analyzers/
COPY Colin.Lottery.Collectors/. ./Colin.Lottery.Collectors/
COPY Colin.Lottery.Models/. ./Colin.Lottery.Models/
COPY Colin.Lottery.Common/. ./Colin.Lottery.Common/
COPY Colin.Lottery.Utils/. ./Colin.Lottery.Utils/
WORKDIR /app/Colin.Lottery.WebApp/
RUN dotnet publish -c Release -o out

FROM build AS testcollector
WORKDIR /app/Colin.Lottery.Collectors.Test
COPY Colin.Lottery.Collectors.Test/. .
ENTRYPOINT ["dotnet", "test", "--logger:trx"]

FROM build AS testanalyzer
WORKDIR /app/Colin.Lottery.Analyzers.Test
COPY Colin.Lottery.Analyzers.Test/. .
ENTRYPOINT ["dotnet", "test", "--logger:trx"]

FROM microsoft/dotnet:runtime AS runtime
WORKDIR /app
COPY --from=build /app/Colin.Lottery.WebApp/out ./
ENTRYPOINT ["dotnet", "Colin.Lottery.WebApp.dll"]
