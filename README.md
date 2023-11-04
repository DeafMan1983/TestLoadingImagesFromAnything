# TestLoadingImageFromAnything
Example for loading image from path, from native library (shared) or from data (embedded resource)
![image](https://github.com/DeafMan1983/TestLoadingImageFromAnything/assets/57066679/05cd62dc-a822-48f3-8d00-e55d03af2696)

1. Compile with **TestEmbedLibrary**:
   Change directory `cd TestEmbedLibrary`
   
   Type with dotnet program: `dotnet publish -c Release -r linux-x64 -p:NativeLib=Shared --self-contained`
   
   Find and get nativelibrary: `cd bin/Release/net8.0/linux-x64/native`
   
   List: `ls *.so`
   
   Result:
   
   ![image](https://github.com/DeafMan1983/TestLoadingImageFromAnything/assets/57066679/363b520d-c37d-4d09-aadc-5947d27379e2)
   
   Copy `TestEmbedLibrary.so` to `data` directory:
   
   `copy TestEmbedLibrary.so ../../../../../../TestEmbedResources/data`


2. Compile with **TestEmbedResources**:
3. 
   Change directory to `TestEmbedResources`:
   ```
   cd ../../../../../../TestEmbedResources/data
   dotnet build -c Release
   dotnet run -c Release
   ```
   
   Result:
   
   ![image](https://github.com/DeafMan1983/TestLoadingImageFromAnything/assets/57066679/bae83a7f-2ecc-4453-b614-ca5849ab0f46)
   
   If it is okay then you can publish as native executable file:
   
   `dotnet publish -c Release -r linux-x64 --self-contained`


That is all for C# and loading from native library ( shared )

I would like to say thank you for understanding and caring my idea!

If you want to spend any money - Sorry I use only Crypto! Don't worry about bad memory! It is seriously! Please note me if you already send me via crypto ( TRX or MATIC ) then you send your proof me email jenspetereckervogt[at]gmail[dot}com - Thank you!

MATIC ( Polygon ): `0x017a8d27d8c3cb7f8b7bc901f74d10fc2eb59e57`

TRX ( Tron ): `TLoCs2APXMfvRc3fbq52vquVtUc52pu1bG`

Thanks for supporting my work!
