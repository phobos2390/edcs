#!/bin/sh
PROJECT_NAME=$(echo "Example")
LIB_GUID=$(echo "6E099E06-32E2-411F-B831-331ECB76FC4E")
TEST_GUID=$(echo "E0219E06-32E2-411F-B831-212ECB76FC4E")
PROJECT_GUID=$(echo "6E09960E-31E2-411F-B831-221EAF71EFCE")

find SOLUTION* -exec sed -i -e "s/\[LIB_GUID\]/\{${LIB_GUID}\}/g" {} \;
find SOLUTION* -exec sed -i -e "s/@LIB_GUID@/${LIB_GUID}/g" {} \;
find SOLUTION* -exec sed -i -e "s/\[TEST_GUID\]/\{${TEST_GUID}\}/g" {} \;
find SOLUTION* -exec sed -i -e "s/@TEST_GUID@/${TEST_GUID}/g" {} \;
find SOLUTION* -exec sed -i -e "s/\[PROJECT_GUID\]/\{${PROJECT_GUID}\}/g" {} \;
find SOLUTION* -exec sed -i -e "s/@PROJECT_GUID@/${PROJECT_GUID}/g" {} \;

find build.cake -exec sed -i -e "s/SOLUTION/${PROJECT_NAME}/g" {} \;
find SOLUTION* -exec sed -i -e "s/SOLUTION/${PROJECT_NAME}/g" {} \;

mv SOLUTION/SOLUTION.csproj "SOLUTION/${PROJECT_NAME}.csproj"
mv SOLUTION/Tests/SOLUTION.Tests.csproj "SOLUTION/Tests/${PROJECT_NAME}.Tests.csproj"
mv SOLUTION/Tests/SOLUTION.Tests.cs "SOLUTION/Tests/${PROJECT_NAME}.Tests.cs"
mv SOLUTION.sln "${PROJECT_NAME}.sln"
mv -T SOLUTION "${PROJECT_NAME}"

echo "rm -rf packages tools ${PROJECT_NAME} ${PROJECT_NAME}.sln" > ./clean.sh
sudo chmod 775 ./clean.sh

