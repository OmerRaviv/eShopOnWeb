# RUN ALL CONTAINERS FROM ROOT (folder with .sln file):
# docker-compose build
# docker-compose up
#
# RUN JUST THIS CONTAINER FROM ROOT (folder with .sln file):
# docker build --pull -t web -f src/Web/Dockerfile .
#
# RUN COMMAND
#  docker run --name eshopweb --rm -it -p 5106:5106 web
FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /app

COPY *.sln .
COPY . .
WORKDIR /app/src/Web
RUN dotnet restore

RUN dotnet publish -c Debug -o out

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/src/Web/out ./

# Optional: Set this here if not setting it from docker-compose.yml
# ENV ASPNETCORE_ENVIRONMENT Development

ENTRYPOINT ["dotnet", "Web.dll"]

RUN apt-get -qq -y update && apt-get -qq -y -f install && apt-get -qq -y install wget
RUN mkdir -m 0755 -p /ozcode-productiondebugger && \
	cd /ozcode-productiondebugger && \
	wget "https://clouddbgprodstorage.blob.core.windows.net/install/linux/ozcode-productiondebugger-linuxinstaller.tar.gz" && \
	tar -zxvf ozcode-productiondebugger-linuxinstaller.tar.gz && \
	rm ozcode-productiondebugger-linuxinstaller.tar.gz

RUN apt-get install -qq -y libc6-dev
RUN ln -s /usr/lib/x86_64-linux-gnu/libdl.so /ozcode-productiondebugger/agent/netstandard2.0/libdl.so

ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={09992526-3e32-4995-8d3d-a97c839393e1}
ENV CORECLR_PROFILER_PATH="/ozcode-productiondebugger/agent/SkyApm.ClrProfiler.so"
ENV CORECLR_PROFILER_HOME="/ozcode-productiondebugger/agent/netstandard2.0"
ENV OzCode:Agent:ForceLoad="True"
ENV OzCode:Agent:ServerAddress="https://ozcodeclouddebugger-staging.azurewebsites.net"
ENV OzCode:Agent:Token="79cbe084-eb5d-4efe-a925-16733a3a1421|909123bf-7d2d-4efe-8c69-16440e393cb7"