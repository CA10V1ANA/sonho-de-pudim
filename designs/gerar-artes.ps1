$ErrorActionPreference = 'Stop'
$csSource = [System.IO.File]::ReadAllText("c:\Users\viana\sonho-de-pudim\designs\PosterEngine.cs", [System.Text.Encoding]::UTF8)
Add-Type -TypeDefinition $csSource -ReferencedAssemblies System.Drawing
$engine = New-Object PosterEngine("c:\Users\viana\sonho-de-pudim\public\assets", "c:\Users\viana\sonho-de-pudim\designs\prontas-para-postar")
$engine.GenerateAll()
Write-Output "Todas as 10 artes foram geradas com sucesso em designs/prontas-para-postar!"
