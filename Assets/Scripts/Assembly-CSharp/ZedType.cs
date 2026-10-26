using System;

[Flags]
public enum ZedType
{
	Clot = 1,
	Gorefast = 2,
	Bloat = 4,
	Crawler = 8,
	Siren = 0x10,
	Stalker = 0x20,
	Husk = 0x40,
	Scrake = 0x80,
	Fleshpound = 0x100,
	Patriarch = 0x200
}
