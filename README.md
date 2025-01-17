# SteamTotp
A .NET 9 library to generate Steam TOTP codes

## NuGet
Get SteamTotp on NuGet: https://www.nuget.org/packages/SteamTotp

## Usage
```cs
using JeremyEspresso.SteamTotp;

var totpCode = SteamTotp.GetAuthCode("cnOgv/KdpLoP6Nbh0GMkXkPXALQ=");
```
