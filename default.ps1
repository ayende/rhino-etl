properties { 
  $base_dir  = resolve-path .
  $lib_dir = "$base_dir\SharedLibs"
  $sln_file = "$base_dir\Rhino.Etl.sln" 
  $version = "1.3.1.0"
  $humanReadableversion = "1.3"
  $tools_dir = "$base_dir\Tools"
  $release_dir = "$base_dir\Release"
  $uploadCategory = "Rhino-ETL"
  $uploader = "..\Uploader\S3Uploader.exe"
} 

task default -depends Release

task Clean { 
  remove-item -force -recurse $release_dir -ErrorAction SilentlyContinue 
} 

task Init -depends Clean { 
	. .\psake_ext.ps1
	
	$infos = (
    "$base_dir\Rhino.Etl.Core\Properties\AssemblyInfo.cs",
    "$base_dir\Rhino.Etl.Dsl\Properties\AssemblyInfo.cs",
    "$base_dir\Rhino.Etl.Tests\Properties\AssemblyInfo.cs",
    "$base_dir\Rhino.Etl.Cmd\Properties\AssemblyInfo.cs"
	);
	
	$infos | foreach { Generate-Assembly-Info `
		-file $_ `
		-title "Rhino ETL $version" `
		-description "Developer freindly ETL Library for .NET" `
		-company "Hibernating Rhinos" `
		-product "Developer freindly ETL Library for .NET $version" `
		-version $version `
		-clsCompliant "false" `
		-copyright "Hibernating Rhinos, Ayende Rahien & Nathan Palmer 2007 - 2015"
	}		
		
	new-item $release_dir -itemType directory 
} 

task Compile -depends Init { 
  & msbuild "$sln_file" /p:Configuration=Release /v:Minimal
  if ($lastExitCode -ne 0) {
        throw "Error: Failed to execute msbuild"
  }
} 

task Test -depends Compile {
  $old = pwd
  cd $tools_dir\XUnit\
  &.\xunit.console.exe "$base_dir\Rhino.Etl.Tests\bin\Release\Rhino.Etl.Tests.dll"
  if ($lastExitCode -ne 0) {
        throw "Error: Failed to execute tests"
    }
  cd $old
}

task Release -depends Test,DoRelease {

}

task Nuget {
  . .\psake_ext.ps1
  
  Generate-Nuget-Spec `
    -title "Rhino-Etl" `
    -version $version `
    -authors "Ayende Rahien, Nathan Palmer" `
    -description "Rhino Etl is a developer friendly Extract, transform and load (ETL) library for .NET" `
    -language "en-GB" `
    -projectURL "https://github.com/hibernating-rhinos/rhino-etl" `
    -licenceUrl "https://github.com/hibernating-rhinos/rhino-etl/blob/master/license.txt" `
    -iconUrl "https://raw.github.com/wiki/hibernating-rhinos/rhino-esb/images/rhino-icon.jpg" `
    -dependencies @( `
      @("Boo", "0.9.4"), `
      @("RhinoDSL", "1.0.0"), `
      @("Common.Logging", "3.0.0"), `
      @("Common.Logging.Core", "3.0.0"), `
      @("FileHelpers", "2.0.0.0") `
     ) `
    -files @( `
      @("$base_dir\Rhino.Etl.Dsl\bin\Release\Rhino.Etl.Core.dll","lib\net35"), `
      @("$base_dir\Rhino.Etl.Dsl\bin\Release\Rhino.Etl.Core.xml","lib\net35"), `
      @("$base_dir\Rhino.Etl.Dsl\bin\Release\Rhino.Etl.Core.pdb","lib\net35"), `
      @("$base_dir\Rhino.Etl.Dsl\bin\Release\Rhino.Etl.Dsl.dll","lib\net35"), `
      @("$base_dir\Rhino.Etl.Dsl\bin\Release\Rhino.Etl.Dsl.xml","lib\net35"), `
      @("$base_dir\Rhino.Etl.Dsl\bin\Release\Rhino.Etl.Dsl.pdb","lib\net35"), `
      @("$base_dir\Rhino.Etl.Dsl\**\*.cs","src\Rhino.Etl.Dsl"), `
      @("$base_dir\Rhino.Etl.Core\**\*.cs","src\Rhino.Etl.Core"), `
      @("license.txt",""), `
      @("acknowledgements.txt","") `
    ) `
    -file "$base_dir\Rhino.Etl.nuspec"
    .\Tools\NuGet.exe pack .\Rhino.Etl.nuspec -Symbols
    
    # Regular
    if (test-path ".\Release\Rhino-Etl.$version.nupkg") {
      del ".\Release\Rhino-Etl.$version.nupkg"
    }
    move "Rhino-Etl.$version.nupkg" .\Release
    
    # Symbol
    if (test-path ".\Release\Rhino-Etl.$version.symbols.nupkg") {
      del ".\Release\Rhino-Etl.$version.symbols.nupkg"
    }
    move "Rhino-Etl.$version.symbols.nupkg" .\Release

  Generate-Nuget-Spec `
    -title "Rhino-Etl-Cmd" `
    -version $version `
    -authors "Ayende Rahien, Nathan Palmer" `
    -description "Rhino Etl is a developer friendly Extract, transform and load (ETL) library for .NET" `
    -language "en-GB" `
    -projectURL "https://github.com/hibernating-rhinos/rhino-etl" `
    -licenceUrl "https://github.com/hibernating-rhinos/rhino-etl/blob/master/license.txt" `
    -iconUrl "https://raw.github.com/wiki/hibernating-rhinos/rhino-esb/images/rhino-icon.jpg" `
    -dependencies @( `
      @("Boo", "0.9.4"), `
      @("RhinoDSL", "1.0.0"), `
      @("Common.Logging", "3.0.0"), `
      @("Common.Logging.Core", "3.0.0"), `
      @("Common.Logging.Log4Net1210", "3.0.0"), `
      @("log4net", "1.2.10"), `
      @("FileHelpers", "2.0.0.0") `
     ) `
    -files @( `
      @("$base_dir\Rhino.Etl.Cmd\bin\Release\Rhino.Etl.Core.dll","lib\net35"), `
      @("$base_dir\Rhino.Etl.Cmd\bin\Release\Rhino.Etl.Core.xml","lib\net35"), `
      @("$base_dir\Rhino.Etl.Cmd\bin\Release\Rhino.Etl.Core.pdb","lib\net35"), `
      @("$base_dir\Rhino.Etl.Cmd\bin\Release\Rhino.Etl.Dsl.dll","lib\net35"), `
      @("$base_dir\Rhino.Etl.Cmd\bin\Release\Rhino.Etl.Dsl.xml","lib\net35"), `
      @("$base_dir\Rhino.Etl.Cmd\bin\Release\Rhino.Etl.Dsl.pdb","lib\net35"), `
      @("$base_dir\Rhino.Etl.Cmd\bin\Release\Rhino.Etl.Cmd.exe","lib\net35"), `
      @("$base_dir\Rhino.Etl.Cmd\bin\Release\Rhino.Etl.Cmd.pdb","lib\net35"), `
      @("$base_dir\Rhino.Etl.Dsl\**\*.cs","src\Rhino.Etl.Dsl"), `
      @("$base_dir\Rhino.Etl.Core\**\*.cs","src\Rhino.Etl.Core"), `
      @("$base_dir\Rhino.Etl.Cmd\**\*.cs","src\Rhino.Etl.Cmd"), `
      @("license.txt",""), `
      @("acknowledgements.txt","") `
    ) `
    -file "$base_dir\Rhino-Etl-Cmd.nuspec"
    .\Tools\NuGet.exe pack .\Rhino-Etl-Cmd.nuspec -Symbols
    
    # Regular
    if (test-path ".\Release\Rhino-Etl-Cmd.$version.nupkg") {
      del ".\Release\Rhino-Etl-Cmd.$version.nupkg"
    }
    move "Rhino-Etl-Cmd.$version.nupkg" .\Release
    
    # Symbol
    if (test-path ".\Release\Rhino-Etl-Cmd.$version.symbols.nupkg") {
      del ".\Release\Rhino-Etl-Cmd.$version.symbols.nupkg"
    }
    move "Rhino-Etl-Cmd.$version.symbols.nupkg" .\Release
}

task DoRelease -depends Compile,NuGet {
	& $tools_dir\zip.exe -9 -A -j $release_dir\Rhino.Etl-$humanReadableversion-Build-$env:ccnetnumericlabel.zip `
		$base_dir\Rhino.Etl.Cmd\bin\Release\Rhino.Etl.Core.dll `
		$base_dir\Rhino.Etl.Cmd\bin\Release\Rhino.Etl.Core.xml `
		$base_dir\Rhino.Etl.Cmd\bin\Release\Rhino.Etl.Dsl.dll `
		$base_dir\Rhino.Etl.Cmd\bin\Release\Rhino.Etl.Dsl.xml `
		$base_dir\Rhino.Etl.Cmd\bin\Release\Rhino.Etl.Cmd.exe `
		$base_dir\Rhino.Etl.Cmd\bin\Release\Rhino.DSL.dll `
		$base_dir\Rhino.Etl.Cmd\bin\Release\log4net.dll `
		$base_dir\Rhino.Etl.Cmd\bin\Release\Common.Logging.dll `
		$base_dir\Rhino.Etl.Cmd\bin\Release\Common.Logging.Core.dll `
		$base_dir\Rhino.Etl.Cmd\bin\Release\Common.Logging.Log4Net1210.dll `
		$base_dir\Rhino.Etl.Cmd\bin\Release\Boo.* `
		$base_dir\Rhino.Etl.Cmd\bin\Release\FileHelpers.dll `
		license.txt `
		acknowledgements.txt
	if ($lastExitCode -ne 0) {
        throw "Error: Failed to execute ZIP command"
    }
}

task Upload -depends DoRelease {
	Write-Host "Starting upload"
	if (Test-Path $uploader) {
		$log = $env:push_msg 
    if($log -eq $null -or $log.Length -eq 0) {
      $log = git log -n 1 --oneline		
    }
		&$uploader "$uploadCategory" "$release_dir\Rhino.Etl-$humanReadableversion-Build-$env:ccnetnumericlabel.zip" "$log"
		
		if ($lastExitCode -ne 0) {
      write-host "Failed to upload to S3: $lastExitCode"
			throw "Error: Failed to publish build"
		}
	}
	else {
		Write-Host "could not find upload script $uploadScript, skipping upload"
	}
}
