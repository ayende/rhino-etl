properties { 
  $base_dir  = resolve-path .
  $lib_dir = "$base_dir\SharedLibs"
  $build_dir = "$base_dir\build" 
  $buildartifacts_dir = "$build_dir\" 
  $sln_file = "$base_dir\Rhino.Etl.sln" 
  $version = "1.0.0.0"
  $humanReadableversion = "1.0"
  $tools_dir = "$base_dir\Tools"
  $release_dir = "$base_dir\Release"
  $uploadCategory = "Rhino-ETL"
  $uploader = "..\Uploader\S3Uploader.exe"
} 

task default -depends Release

task Clean { 
  remove-item -force -recurse $buildartifacts_dir -ErrorAction SilentlyContinue 
  remove-item -force -recurse $release_dir -ErrorAction SilentlyContinue 
} 

task Init -depends Clean { 
	. .\psake_ext.ps1
	Generate-Assembly-Info `
		-file "$base_dir\Rhino.Etl.Core\Properties\AssemblyInfo.cs" `
		-title "Rhino ETL $version" `
		-description "Developer freindly ETL Library for .NET" `
		-company "Hibernating Rhinos" `
		-product "Developer freindly ETL Library for .NET $version" `
		-version $version `
		-clsCompliant "false" `
		-copyright "Hibernating Rhinos & Ayende Rahien 2007 - 2009"
	
	Generate-Assembly-Info `
		-file "$base_dir\Rhino.Etl.Dsl\Properties\AssemblyInfo.cs" `
		-title "Rhino ETL DSL $version" `
		-description "Developer freindly ETL Library for .NET" `
		-company "Hibernating Rhinos" `
		-product "Developer freindly ETL Library for .NET $version" `
		-version $version `
		-clsCompliant "false" `
		-copyright "Hibernating Rhinos & Ayende Rahien 2007 - 2009"
	
	Generate-Assembly-Info `
		-file "$base_dir\Rhino.Etl.Tests\Properties\AssemblyInfo.cs" `
		-title "Rhino ETL Tests $version" `
		-description "Developer freindly ETL Library for .NET" `
		-company "Hibernating Rhinos" `
		-product "Developer freindly ETL Library for .NET $version" `
		-version $version `
		-clsCompliant "false" `
		-copyright "Hibernating Rhinos & Ayende Rahien 2007 - 2009"
	
	Generate-Assembly-Info `
		-file "$base_dir\Rhino.Etl.Cmd\Properties\AssemblyInfo.cs" `
		-title "Rhino ETL Command Line $version" `
		-description "Developer freindly ETL Library for .NET" `
		-company "Hibernating Rhinos" `
		-product "Developer freindly ETL Library for .NET $version" `
		-version $version `
		-clsCompliant "false" `
		-copyright "Hibernating Rhinos & Ayende Rahien 2007 - 2009"
		
		
	new-item $release_dir -itemType directory 
	new-item $buildartifacts_dir -itemType directory 
	cp $tools_dir\XUnit\*.* $build_dir
} 

task Compile -depends Init { 
  & msbuild "$sln_file" "/p:OutDir=$build_dir\\" /p:Configuration=Release
  if ($lastExitCode -ne 0) {
        throw "Error: Failed to execute msbuild"
  }
} 

task Test -depends Compile {
  $old = pwd
  cd $build_dir
  &.\xunit.console.exe "$build_dir\Rhino.Etl.Tests.dll"
  if ($lastExitCode -ne 0) {
        throw "Error: Failed to execute tests"
    }
  cd $old
}

task Release -depends Test,DoRelease {

}

task DoRelease -depends Compile {
	& $tools_dir\zip.exe -9 -A -j $release_dir\Rhino.Etl-$humanReadableversion-Build-$env:ccnetnumericlabel.zip `
		$build_dir\Rhino.Etl.Core.dll `
		$build_dir\Rhino.Etl.Dsl.dll `
		$build_dir\Rhino.Etl.Cmd.exe `
		Rhino.Etl.Core\Rhino.Etl.Core.xml `
		Rhino.Etl.Dsl\Rhino.Etl.Dsl.xml `
		$build_dir\Boo.* `
		$build_dir\FileHelpers.dll `
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
