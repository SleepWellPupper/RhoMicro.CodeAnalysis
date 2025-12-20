#!/bin/bash

# Default configuration
CONFIGURATION="Debug"

# Get the directory of the script
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
PROPS_PATH="$SCRIPT_DIR/../Directory.Build.props"

# Increment assembly revision
REVISION=$(grep -oP '(?<=<AutoIncrementedAssemblyRevision>)\d+' "$PROPS_PATH")
NEW_REVISION=$((REVISION + 1))
sed -i "s/<AutoIncrementedAssemblyRevision>$REVISION</<AutoIncrementedAssemblyRevision>$NEW_REVISION</" "$PROPS_PATH"

echo -e "\033[32mIncremented assembly revision from $REVISION to $NEW_REVISION.\033[0m"

# Build UtilityGenerators.csproj
echo -e "\033[33mBuilding UtilityGenerators.csproj\033[0m"
dotnet build -c "$CONFIGURATION" -v q -p:SolutionName="RhoMicro.CodeAnalysis" > /dev/null 2>&1
if [ $? -ne 0 ]; then
    echo -e "\033[31mError while building UtilityGenerators.csproj\033[0m"
    exit 1
else
    echo -e "\033[32mBuilt UtilityGenerators.csproj\033[0m"
fi

# Build UtilityGenerators.Dev.csproj
echo -e "\033[33mBuilding UtilityGenerators.Dev.csproj\033[0m"
dotnet build "$SCRIPT_DIR/../UtilityGenerators.Dev/UtilityGenerators.Dev.csproj" -c "$CONFIGURATION" -v q -p:SolutionName="RhoMicro.CodeAnalysis" > /dev/null 2>&1
if [ $? -ne 0 ]; then
    echo -e "\033[31mError while building UtilityGenerators.Dev.csproj\033[0m"
    exit 1
else
    echo -e "\033[32mBuilt UtilityGenerators.Dev.csproj\033[0m"
fi

# Restore UtilityGenerators.csproj
echo -e "\033[33mRestoring UtilityGenerators.csproj\033[0m"
dotnet restore --force > /dev/null 2>&1
if [ $? -ne 0 ]; then
    echo -e "\033[31mError while restoring UtilityGenerators.csproj\033[0m"
    exit 1
else
    echo -e "\033[32mRestored UtilityGenerators.csproj\033[0m"
fi
