%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild src/kinesisnet/kinesisnet.csproj /target:Clean
%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild src/kinesisnet/kinesisnet.csproj /P:Configuration=Release

nuget\nuget.exe pack src\kinesisnet\kinesisnet.csproj -Prop Configuration=Release -OutputDirectory nuget