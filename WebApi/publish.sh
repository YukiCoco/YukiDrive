version="1.1.0"

dotnet publish --runtime linux-x64 --self-contained true -o /Users/yukino/Desktop/Projects/YukiDrive/Release/WebApi/${version}/linux-x64 -c Release
dotnet publish --runtime linux-arm --self-contained true -o /Users/yukino/Desktop/Projects/YukiDrive/Release/WebApi/${version}/linux-arm -c Release
dotnet publish --runtime osx-x64 --self-contained true -o /Users/yukino/Desktop/Projects/YukiDrive/Release/WebApi/${version}/macOS -c Release
dotnet publish --runtime win-x64 --self-contained true -o /Users/yukino/Desktop/Projects/YukiDrive/Release/WebApi/${version}/win -c Release