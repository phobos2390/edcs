#!/bin/sh
PROJECT_NAME=$(echo "edcs")
LIB_GUID=$(echo     "6E099EAA-E221-411F-A1C1-331ECB76FC4E")
TEST_GUID=$(echo    "E021AA06-3EA2-1DAF-B831-212ECB12FC4E")
PROJECT_GUID=$(echo "6E0BF60E-31E2-BC1F-BA3B-221EAF71EFCE")

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

