function Get-Git-Commit
{
	$gitLog = git log --oneline -1
	return $gitLog.Split(' ')[0]
}

function Generate-Assembly-Info
{
param(
	[string]$clsCompliant = "true",
	[string]$title, 
	[string]$description, 
	[string]$company, 
	[string]$product, 
	[string]$copyright, 
	[string]$version,
	[string]$file = $(throw "file is a required parameter.")
)
  $commit = Get-Git-Commit
  $asmInfo = "using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: CLSCompliantAttribute($clsCompliant )]
[assembly: ComVisibleAttribute(false)]
[assembly: AssemblyTitleAttribute(""$title"")]
[assembly: AssemblyDescriptionAttribute(""$description"")]
[assembly: AssemblyCompanyAttribute(""$company"")]
[assembly: AssemblyProductAttribute(""$product"")]
[assembly: AssemblyCopyrightAttribute(""$copyright"")]
[assembly: AssemblyVersionAttribute(""$version"")]
[assembly: AssemblyInformationalVersionAttribute(""$version / $commit"")]
[assembly: AssemblyFileVersionAttribute(""$version"")]
[assembly: AssemblyDelaySignAttribute(false)]
"

	$dir = [System.IO.Path]::GetDirectoryName($file)
	if ([System.IO.Directory]::Exists($dir) -eq $false)
	{
		Write-Host "Creating directory $dir"
		[System.IO.Directory]::CreateDirectory($dir)
	}
	Write-Host "Generating assembly info file: $file"
	Write-Output $asmInfo > $file
}

function Generate-Nuget-Spec
{
param(
  [string]$title,
  [string]$version,
  [string]$authors,
  [string]$description,
  [string]$language,
  [string]$projectUrl,
  [string]$licenceUrl,
  [string]$iconUrl,
  [array]$dependencies,
  [array]$files,
  [string]$file = $(throw "file is a required parameter.")
)
  $out = "<?xml version=""1.0"" encoding=""utf-8""?>" + [Environment]::NewLine
  $out = $out + "<package xmlns=""http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"">"  + [Environment]::NewLine
  $out = $out + "    <metadata>
        <id>$title</id>
        <version>$version</version>
        <authors>$authors</authors>
        <description>$description</description>
        <language>$language</language>
        <projectUrl>$projectUrl</projectUrl>
        <licenseUrl>$licenceUrl</licenseUrl>
        <iconUrl>$iconUrl</iconUrl>
"
  $out = $out + "        <dependencies>
"
  foreach($dependency in $dependencies) {
    $out = $out + "            <dependency id=""" + $dependency[0] + """ 
                        version=""" + $dependency[1] + """ />
"
  }
  $out = $out + "        </dependencies>
"  
  $out = $out + "    </metadata>
"
  
  $out = $out + "    <files>
"  
  foreach($includedFile in $files) {
    $out = $out + "        <file src=""" + $includedFile[0] + """
              target=""" + $includedFile[1] + """ />
"
  }
  $out = $out + "    </files>
"  
  $out = $out + "</package>"
  
  $out | out-file $file -Encoding utf8
}