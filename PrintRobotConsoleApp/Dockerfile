ARG ARG_APM_VENDOR=datadog

# to jerry, we are using .net 5, you probably need modify them
# FROM enterprisenonpacr.azurecr.io/dotnet/aspnet:6.0-alpine AS base
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

#FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet restore "PrintRobotConsoleApp/PrintRobotApp.csproj"
RUN dotnet build "PrintRobotConsoleApp/PrintRobotApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "PrintRobotConsoleApp/PrintRobotApp.csproj" -c Release -o /app

FROM base AS apm-vendor-newrelic

RUN apk --no-cache add wget
ARG NewRelicHome=/usr/local/newrelic-netcore20-agent

ENV CORECLR_ENABLE_PROFILING=1 \
  CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
  CORECLR_NEWRELIC_HOME=$NewRelicHome \
  CORECLR_PROFILER_PATH=$NewRelicHome/libNewRelicProfiler.so \
  NEW_RELIC_DISTRIBUTED_TRACING_ENABLED=true
RUN mkdir -p ${NewRelicHome} && cd /usr/local && wget https://download.newrelic.com/dot_net_agent/previous_releases/8.30.0/newrelic-netcore20-agent_8.30.0.0_amd64.tar.gz && tar xvzf newrelic-netcore20-agent_8.30.0.0_amd64.tar.gz

FROM base AS apm-vendor-datadog

RUN apk --no-cache add curl
# Download and install the Tracer
RUN mkdir -p /opt/datadog \
  && mkdir -p /var/log/datadog \
  && TRACER_VERSION=$(curl -s https://api.github.com/repos/DataDog/dd-trace-dotnet/releases/latest | grep tag_name | cut -d '"' -f 4 | cut -c2-) \
  && curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm-${TRACER_VERSION}-musl.tar.gz \
  && tar -xzf datadog-dotnet-apm-${TRACER_VERSION}-musl.tar.gz -C /opt/datadog \
  && sh /opt/datadog/createLogPath.sh \
  && rm ./datadog-dotnet-apm-${TRACER_VERSION}-musl.tar.gz

# Enable the tracer
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
ENV DD_DOTNET_TRACER_HOME=/opt/datadog
ENV DD_INTEGRATIONS=/opt/datadog/integrations.json

FROM apm-vendor-${ARG_APM_VENDOR} as with-apm

FROM with-apm as final

#fix: sql server accessing: Globalization Invariant Mode is not supported
RUN apk --no-cache add icu-libs tzdata
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# fix the "The type initializer for 'Gdip' threw an exception."
RUN apk add libgdiplus-dev fontconfig ttf-dejavu msttcorefonts-installer --update-cache --repository http://dl-3.alpinelinux.org/alpine/edge/testing/ --allow-untrusted \
  && update-ms-fonts \
  && fc-cache -f

# custom build of libgdiplus wiht REGION_MAX_BITMAP_SIZE  (2 * 1024 * 102400 >> 3)
COPY --from=build /src/lib/libgdiplus.so.0.0.0 /usr/lib/libgdiplus.so.0.0.0
RUN chown root:root /usr/lib/libgdiplus.so.0.0.0

WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PrintRobotApp.dll"]
